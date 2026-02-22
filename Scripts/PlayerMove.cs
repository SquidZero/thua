using System;
using System.Linq;
using Godot;

public partial class PlayerMove : CharacterBody3D
{
    [Export]
    public Camera3D playerCamera;

    [Export]
    public RayCast3D rayCast;

    [Export]
    public float jumpHeight = 4f;
    Vector2 movementAxis = Vector2.Zero;
    float velocityY = 0f;
    bool sprinting = false;
    Vector3 from = Vector3.Zero;
    Vector3 to = Vector3.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        movementAxis = Input.GetVector("left", "right", "forward", "backward");
        sprinting = Input.IsActionPressed("sprint");
        movementAxis *= sprinting ? (float)delta * 1500 : (float)delta * 800;
        if (IsOnFloor())
        {
            if (Input.GetActionStrength("jump") > 0)
            {
                velocityY = jumpHeight;
            }
        }
        else
        {
            velocityY += GetGravity().Y;
        }
        //GD.Print("movementVec: " + movementVec);
        Velocity = new Vector3(movementAxis.X, velocityY, movementAxis.Y).Rotated(
            Vector3.Up,
            Rotation.Y
        );
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
