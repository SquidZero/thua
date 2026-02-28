using System;
using Godot;

// testicles
public partial class PlayerMove : CharacterBody3D
{
	[Export]
	public Camera3D playerCamera;

	[Export]
	public RayCast3D rayCast;
	Vector3 movementAxis = Vector3.Zero;
	Vector3 boingVec = Vector3.Zero;
	Vector3 slideVec = Vector3.Zero;
	bool sprinting = false;
	public bool sliding = false;
	Vector3 from = Vector3.Zero;
	Vector3 to = Vector3.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	[Export]
	float walkSpeed = 100.00f;
	[Export]
	float runSpeed = 200.00f;
	[Export]
	float jumpHeight = 10.00f;
	[Export]
	float floorFriction = 25.00f;
	[Export]
	float airFriction = 15.00f;
	[Export]
	byte wallJumps = 0;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 movementInput = Input.GetVector("left", "right", "forward", "backward");
		float moveStep = (sprinting ? runSpeed : walkSpeed) * (float)delta;
		float frictionStep = (IsOnFloor() ? floorFriction : airFriction) * (float)delta;

		Vector3 friction = Vector3.Zero;

		friction.X = Velocity.X > 0.00f ? frictionStep : -frictionStep;
		friction.Z = Velocity.Z > 0.00f ? frictionStep : -frictionStep;
		if (Math.Abs(frictionStep) > Math.Abs(Velocity.X)) { friction.X = Velocity.X; }
		if (Math.Abs(frictionStep) > Math.Abs(Velocity.Z)) { friction.Z = Velocity.Z; }

        if (movementInput != Vector2.Zero) {
			sprinting = Input.IsActionPressed("sprint");
			Vector3 velocity2D = Velocity;
			velocity2D.Y = 0.00f;
			movementAxis.X = movementInput.X*moveStep;
			movementAxis.Z = movementInput.Y*moveStep;
			movementAxis = movementAxis.Rotated(Vector3.Up, Rotation.Y);
			if ((velocity2D+movementAxis).Length() >= (sprinting ? 20.00f : 10.00f)) { movementAxis = (velocity2D+movementAxis).LimitLength(sprinting ? 20.00f : 10.00f) - velocity2D; }
		}
		else
		{
			friction *= 1.80f;
			sprinting = false;
		}

		//if (movementAxis.LengthSquared() < 0.25f ) { movementAxis = Vector3.Zero; }

		if (IsOnFloor())
		{	
			boingVec = Vector3.Zero;
			wallJumps = 0;
			if (Input.IsActionJustPressed("slide")) {
				sliding = true;
				slideVec = (movementInput != Vector2.Zero ? new Vector3(movementInput.X, 0.00f, movementInput.Y) : new Vector3(0.00f, 0.00f, -1.00f)).Rotated(Vector3.Up, Rotation.Y);
				slideVec *= (float)delta * 1000.0f;
			}
			if (Input.IsActionJustPressed("jump"))
			{
				boingVec.Y = jumpHeight;
			}
		}
		else
		{
			boingVec.Y *= IsOnCeiling() ? 0 : 1;
			if (IsOnWallOnly() && Input.IsActionJustPressed("jump") && wallJumps < 3)
			{
				wallJumps++;
				
				boingVec = GetWallNormal().Normalized() * 500.00f * (float)delta;
				boingVec.Y = jumpHeight * 1.40f;
			}
			else
			{
				boingVec.Y += GetGravity().Y;
			}
		}
		

		if (GlobalPosition.Y < -75.00f) {
			GD.Print("You fell off the map, idiot");
			GlobalPosition = Vector3.Zero;
		}
		if (Input.IsActionPressed("fly")) {
			boingVec.Y = 1.00f;
		}
        if (sliding && IsOnFloor() && Input.IsActionPressed("slide") && !Input.IsActionJustPressed("jump"))
        {
            Velocity = slideVec;
        }
        else
        {
			if (sliding && Input.IsActionJustPressed("jump")) {
				boingVec.Y = jumpHeight * 1.50f;
				Velocity += slideVec;
				
			}
            sliding = false;
			
			Velocity += movementAxis;
            Velocity += boingVec;
			Velocity -= friction;

			movementAxis = Vector3.Zero;
			boingVec = Vector3.Zero;
			slideVec = Vector3.Zero;
        }
		MoveAndSlide();
		Velocity = GetRealVelocity();
		if (GetRealVelocity().LengthSquared() < 250.00f && IsOnWall() && GetChild<Camera3D>(2).Position.Y == -0.60f) { sliding = false; }

		if (Input.IsActionJustReleased("shoot"))
		{
			var col = rayCast.GetCollider();
			col?.EmitSignal("take_damage", 0.10f);
		}
	}
}
