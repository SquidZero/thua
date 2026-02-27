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
	Vector3 boingVec = Vector3.Zero;
	Vector3 slideVec = Vector3.Zero;
	float slideAng = 0;
	bool sprinting = false;
	public bool sliding = false;
	Vector3 from = Vector3.Zero;
	Vector3 to = Vector3.Zero;
	Vector3 resetyvec = new Vector3(0,1,0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	[Export]
	float walkSpeed = 768.00f;
	[Export]
	float runSpeed = 1280.00f;
	[Export]
	float jumpHeight = 10.00f;
	[Export]
	float frictionFactor = 0.90f;
	[Export]
	float slideFrictionFactor = 0.98f;
	byte wallJumps = 0;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 movementInput = Input.GetVector("left", "right", "forward", "backward");
		float moveStep = (sprinting ? runSpeed : walkSpeed) * (float)delta;
		
		float friction = frictionFactor * (IsOnFloor() && movementInput == Vector2.Zero ? 0.80f : 1.00f);
		float slideFriction = slideFrictionFactor * (IsOnFloor() ? 0.80f : 1.00f);

        if (movementInput != Vector2.Zero) {
			sprinting = Input.IsActionPressed("sprint");
			Vector3 velocity2D = Velocity;
			velocity2D.Y = 0.00f;
			movementAxis.X += movementInput.X*moveStep/(velocity2D.Length()+5f);
			movementAxis.Y += movementInput.Y*moveStep/(velocity2D.Length()+5f);
		}
		else
		{
			sprinting = false;
		}
		movementAxis *= friction;

		if (movementAxis.LengthSquared() < 0.25f ) { movementAxis = Vector2.Zero; }

		if (IsOnFloor())
		{	
			boingVec = Vector3.Zero;
			wallJumps = 0;
			if (Input.IsActionJustPressed("slide")) {
				sliding = true;
				slideVec = movementInput != Vector2.Zero ? new Vector3(movementInput.X, 0.00f, movementInput.Y) : new Vector3(0.00f, 0.00f, -1.00f);
				slideVec *= (float)delta * 1000.0f;
				slideAng = Rotation.Y;
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
				
				boingVec = GetWallNormal().Normalized() * moveStep * 1.60f;
				boingVec.Y = jumpHeight * 1.40f;
			}
			else
			{
				boingVec.X *= 0.95f;
				boingVec.Z *= 0.95f;
				boingVec.Y += GetGravity().Y;
			}
		}
		

		if (GlobalPosition.Y < -75.00f) {
			GD.Print("You fell off the map, idiot");
			GlobalPosition = Vector3.Zero;
		}
		if (Input.IsActionPressed("fly")) {
			boingVec.Y = jumpHeight;	
		}
        if (sliding && IsOnFloor() && Input.IsActionPressed("slide") && !Input.IsActionJustPressed("jump"))
        {
            Velocity = new Vector3(slideVec.X, boingVec.Y, slideVec.Z).Rotated(Vector3.Up, slideAng);
			
        }
        else
        {
			if (sliding && Input.IsActionJustPressed("jump")) {
				boingVec.Y = jumpHeight * 1.50f;
				slideVec *= 2.00f;
			}
            sliding = false;
			slideVec *= slideFriction;
			if (slideVec.LengthSquared() < 0.25f) { slideVec = Vector3.Zero; }
            Velocity = new Vector3(movementAxis.X, 0.00f, movementAxis.Y).Rotated(Vector3.Up, Rotation.Y);
            Velocity += boingVec;
			Velocity += new Vector3(slideVec.X, 0.00f, slideVec.Z).Rotated(Vector3.Up, slideAng);
        }
		// if (IsOnCeiling()) {
		// 	boingVec = Vector3.Zero;
		// }
		MoveAndSlide();
		if (GetRealVelocity().LengthSquared() < 250.00f && IsOnWall() && GetChild<Camera3D>(2).Position.Y == -0.60f) { sliding = false; }

		if (Input.IsActionJustReleased("shoot"))
		{
			var col = rayCast.GetCollider();
			col?.EmitSignal("take_damage", 0.10f);
		}
	}
}
