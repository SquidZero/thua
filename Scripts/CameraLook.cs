using System;
using Godot;

public partial class CameraLook : Camera3D
{
	Vector2 mouseDelta = Vector2.Zero;
	Vector3 startPos = Vector3.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startPos = Position;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if ((bool)GetParent().Get("sliding")) {
			Position = new Vector3(Position.X, Math.Max(Position.Y-(float)delta*60,-0.6f), Position.Z);
		}
		else
		{
			Position = new Vector3(Position.X, Math.Min(Position.Y+(float)delta*60,startPos.Y), Position.Z);
		}
		if (
			(RotationDegrees.X + mouseDelta.Y >= 90 && mouseDelta.Sign().Y >= 0)
			|| (RotationDegrees.X + mouseDelta.Y <= -90 && mouseDelta.Sign().Y <= 0)
		)
			mouseDelta[1] = 0;
		RotateX(mouseDelta.Y);
		((Node3D)GetParent()).RotateY(mouseDelta.X);
		mouseDelta = Vector2.Zero;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			mouseDelta =
				(RotationDegrees.X + mouseDelta.Y >= 90 && mouseDelta.Sign().Y >= 0)
				|| (RotationDegrees.X + mouseDelta.Y <= -90 && mouseDelta.Sign().Y <= 0)
					? -motion.Relative / 300
					: -motion.Relative / 300;
		}
	}
}
