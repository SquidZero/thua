using System;
using Godot;

// testicles
public partial class PlayerMove : CharacterBody3D
{
	[Export]
	public Camera3D playerCamera;

	[Export]
	public RayCast3D rayCast;
	Vector2 movementAxis = Vector2.Zero;
	float velocityY = 0.0f;
	bool sprinting = false;
	Vector3 from = Vector3.Zero;
	Vector3 to = Vector3.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

		
	public float walkSpeed = 340;
	public float runSpeed = 620;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var movementInput = Input.GetVector("left", "right", "forward", "backward");

		if (movementInput != Vector2.Zero) {
			sprinting = Input.IsActionPressed("sprint");
			var moveStep = (sprinting ? runSpeed : walkSpeed) * (float)delta;
			movementAxis = new Vector2(
				(float)Math.MinMagnitude(movementAxis.X+(movementInput.X*moveStep*(1f/(Velocity.Length()+5f))),movementInput.X*moveStep),
				(float)Math.MinMagnitude(movementAxis.Y+(movementInput.Y*moveStep*(1f/(Velocity.Length()+5f))),movementInput.Y*moveStep)
			);
		}
		else
		{
			movementAxis *= IsOnFloor() ? 0.67f : 0.98f;
			sprinting = false;
		}
		if (IsOnFloor())
		{
			if (Input.IsActionPressed("jump"))
			{
				velocityY = 6.0f;
			}
		}
		else
		{
			velocityY += GetGravity().Y;
		}
		if (GlobalPosition[1] < -75.0f) {
			GD.Print("You fell off the map, idiot");
			GlobalPosition = Vector3.Zero;
		}
		Velocity = new Vector3(movementAxis.X, Math.Min(velocityY,100.0f), movementAxis.Y).Rotated(Vector3.Up, Rotation.Y);
		MoveAndSlide();

		if (Input.IsActionJustReleased("shoot"))
		{
			var col = rayCast.GetCollider();
			col?.EmitSignal("take_damage", 0.1f);
		}
	}
}
