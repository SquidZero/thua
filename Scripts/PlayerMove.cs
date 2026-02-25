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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	[Export]
	float walkSpeed = 620;
	[Export]
	float runSpeed = 1400;
	
	[Export]
	float floorFriction = 0.89f;
	[Export]
	float airFriction = 0.95f;
	byte wallJumps = 0;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 movementInput = Input.GetVector("left", "right", "forward", "backward");
		float frictionFactor = IsOnFloor() ? floorFriction : airFriction;
		
        if (movementInput != Vector2.Zero) {
			sprinting = Input.IsActionPressed("sprint");
			float moveStep = (sprinting ? runSpeed : walkSpeed) * (float)delta;
			Vector3 stupid = Velocity;
			stupid.Y = 0;
			movementAxis = new Vector2(
				movementAxis.X+(movementInput.X*moveStep*(1f/(stupid.Length()+5f))),
				movementAxis.Y+(movementInput.Y*moveStep*(1f/(stupid.Length()+5f)))
			);
		}
		else
		{
			sprinting = false;
		}
		movementAxis *= frictionFactor;
		if (IsOnFloor())
		{	
			boingVec = Vector3.Zero;
			wallJumps = 0;
			if (Input.IsActionJustPressed("slide")) {
				sliding = true;
				slideVec = movementInput != Vector2.Zero ? new Vector3(movementInput.X, 0.0f, movementInput.Y) : new Vector3(0.0f, 0.0f, -1.0f);
				slideVec *= (float)delta * 1000.0f;
				slideAng = Rotation.Y;
			}
			if (Input.IsActionJustPressed("jump"))
			{
				boingVec.Y = 10.0f;
			}
		}
		else
		{
			if (IsOnWallOnly() && Input.IsActionJustPressed("jump") && wallJumps < 3)
			{
				wallJumps++;
				
				boingVec = GetWallNormal() * (float)delta * (sprinting ? 1000.0f : 700.0f);
				boingVec.Y = 1.0f;
			}
			else
			{
				boingVec.X *= 0.95f;
				boingVec.Z *= 0.95f;
				boingVec.Y += GetGravity().Y;
			}
		}
		

		if (GlobalPosition.Y < -75.0f) {
			GD.Print("You fell off the map, idiot");
			GlobalPosition = Vector3.Zero;
		}
		if (Input.IsActionPressed("fly")) {
			boingVec.Y = 16.0f;	
		}
        if (sliding && IsOnFloor() && Input.IsActionPressed("slide") && !Input.IsActionJustPressed("jump"))
        {
            Velocity = new Vector3(slideVec.X, boingVec.Y, slideVec.Z).Rotated(Vector3.Up, slideAng);
        }
        else
        {
			if (sliding && Input.IsActionJustPressed("jump")) {
				boingVec.Y = 13.0f;
				slideVec *= 3.0f;
			}
            sliding = false;
			slideVec *= frictionFactor;
            Velocity = new Vector3(movementAxis.X, 0.0f, movementAxis.Y).Rotated(Vector3.Up, Rotation.Y);
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
