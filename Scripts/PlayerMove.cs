using Godot;
using System;

// testicles
public partial class PlayerMove : CharacterBody3D
{
    [Export]
    public Camera3D playerCamera;

    [Export]
    public RayCast3D rayCast;
    Vector2 movementAxis = Vector2.Zero;
    Vector3 boingVec = Vector3.Zero;
    Vector3 slideVec = Vector3.Zero;
    float slideAng = 0;
    bool sprinting = false;
    public bool sliding = false;
    Vector3 from = Vector3.Zero;
    Vector3 to = Vector3.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    [Export]
    float walkSpeed = 340;

    [Export]
    float floorFriction = 0.67f;

    [Export]
    float airFriction = 0.98f;

    [Export] float jumpHeight = 6.0f;
    [Export]
    float runSpeed = 620;
    byte wallJumps = 0;

    [Export]
    Vector2 movementInput = Vector2.Zero;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        float frictionFactor = IsOnFloor() ? floorFriction : airFriction;
        if (Math.MaxMagnitude(Math.Abs(movementInput.X), Math.Abs(movementInput.Y)) < 0.25d)
            movementInput = Vector2.Zero;
        movementInput *= 0.85f;
        movementInput += Input.GetVector("left", "right", "forward", "backward");
        if (movementInput != Vector2.Zero)
        {
            sprinting = Input.IsActionPressed("sprint");
            var moveStep = (sprinting ? runSpeed : walkSpeed) * (float)delta;
            movementAxis = movementInput * moveStep;

            /*
              movementAxis = new Vector2(
                (float)
                    Math.MinMagnitude(
                        movementAxis.X + (movementInput.X * moveStep / (Velocity.Length() + 5f)),
                        movementInput.X * moveStep
                    ),
                (float)
                    Math.MinMagnitude(
                        movementAxis.Y + (movementInput.Y * moveStep / (Velocity.Length() + 5f)),
                        movementInput.Y * moveStep
                    )
            );
            */
        }
        else
        {
            movementAxis *= frictionFactor;
            if (movementAxis.Length() < 1)
                movementAxis = Vector2.Zero;
            sprinting = false;
        }
        if (IsOnFloor())
        {
            boingVec = Vector3.Zero;
            wallJumps = 0;
            if (Input.IsActionJustPressed("slide"))
            {
                sliding = true;
                slideVec =
                    movementInput != Vector2.Zero
                        ? new Vector3(movementInput.X, 0.0f, movementInput.Y)
                        : new Vector3(0.0f, 0.0f, -1.0f);
                slideVec *= (float)delta * 860.0f;
                slideAng = Rotation.Y;
            }
            if (Input.IsActionPressed("jump"))
            {
                boingVec.Y = jumpHeight;
            }
        }
        else
        {
            if (IsOnWallOnly() && Input.IsActionPressed("jump") && wallJumps < 3)
            {
                wallJumps++;

                boingVec = GetWallNormal() * (float)delta * (sprinting ? 1000.0f : 700.0f);
                boingVec.Y = 1.4f*jumpHeight;
            }
            else
            {
                boingVec.X *= 0.95f;
                boingVec.Z *= 0.95f;
                boingVec.Y += GetGravity().Y;
            }
        }

        if (GlobalPosition[1] < -75.0f)
        {
            GD.Print("You fell off the map, idiot");
            GlobalPosition = Vector3.Zero;
        }

        if (
            sliding
            && IsOnFloor()
            && Input.IsActionPressed("slide")
            && !Input.IsActionPressed("jump")
        )
        {
            Velocity = new Vector3(slideVec.X, boingVec.Y, slideVec.Z).Rotated(
                Vector3.Up,
                slideAng
            );
        }
        else
        {
            if (sliding && Input.IsActionPressed("jump"))
            {
                boingVec.Y = jumpHeight*1.5f;
                slideVec *= 1.8f;
            }
            sliding = false;
            slideVec *= frictionFactor;
            Velocity = new Vector3(movementAxis.X, 0.0f, movementAxis.Y).Rotated(
                Vector3.Up,
                Rotation.Y
            );
            Velocity += boingVec;
            Velocity += new Vector3(slideVec.X, 0.0f, slideVec.Z).Rotated(Vector3.Up, slideAng);
        }
        MoveAndSlide();

        if (Input.IsActionJustReleased("shoot"))
        {
            var col = rayCast.GetCollider();
            col?.EmitSignal("take_damage", 0.1f);
        }
    }
}
