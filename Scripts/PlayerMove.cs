using Godot;

// testicles
public partial class PlayerMove : CharacterBody3D
{
    [Export]
    public Camera3D playerCamera;

    [Export]
    public RayCast3D rayCast;
    Vector2 movementAxis = Vector2.Zero;
    float velocityY = 0f;

    [Export]
    float walkSpeed = 250f;

    [Export]
    float sprintingSpeed = 750f;
    bool sprinting = false;
    Vector3 from = Vector3.Zero;
    Vector3 to = Vector3.Zero;
    Vector3 flooredVel = Vector3.Zero;

    [Export]
    float friction = 2f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        movementAxis = Input.GetVector("left", "right", "forward", "backward");
        sprinting = Input.IsActionPressed("sprint") && IsOnFloor();
        movementAxis *= sprinting ? (float)delta * sprintingSpeed : (float)delta * walkSpeed;
        if (!IsOnFloor())
        {
            velocityY += GetGravity().Y;
            //Velocity = new Vector3(Velocity.X, Velocity.Y, Velocity.Z);
            Velocity =
                flooredVel
                + new Vector3(movementAxis.X, velocityY - Velocity.Y, movementAxis.Y).Rotated(
                    Vector3.Up,
                    Rotation.Y
                );
            Velocity /= friction;
        }
        else
        {
            //if (Velocity > flooredVel && IsOnFloor() && sprinting)
            flooredVel = Velocity;
            if (Input.GetActionStrength("jump") > 0)
            {
                velocityY = 6f;
            }
            Velocity += new Vector3(movementAxis.X, velocityY - Velocity.Y, movementAxis.Y).Rotated(
                Vector3.Up,
                Rotation.Y
            );
            Velocity /= friction;
        }

        //GD.Print("movementVec: " + movementVec);

        MoveAndSlide();
        //GD.Print("input: " + movementAxis);
        //GD.Print("position: " + GlobalPosition);

        if (Input.IsActionJustReleased("shoot"))
        {
            var col = rayCast.GetCollider();
            col?.EmitSignal("take_damage", 0.1f);
        }
    }
}
