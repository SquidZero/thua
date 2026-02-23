using System;
using System.Linq;
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
	bool sprinting = false;
	Vector3 from = Vector3.Zero;
	Vector3 to = Vector3.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }
	
	public bool sprintJump = false;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		sprinting = Input.IsActionPressed("sprint"); //&& IsOnFloor();
		var moving = Input.GetVector("left", "right", "forward", "backward");
		if (moving[0] != 0 || moving[1] != 0) {
			movementAxis = Input.GetVector("left", "right", "forward", "backward");
			movementAxis *= sprinting || sprintJump ? (float)delta * 620 : (float)delta * 340;
		}
		else
		{
			movementAxis *= IsOnFloor() ? new Vector2(0.60f,0.60f) : new Vector2(0.89f,0.89f);
		}
		
		if (IsOnFloor())
		{
			//sprintJump = sprinting;
			if (Input.IsActionPressed("jump"))
			{
				velocityY = 6f;
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
