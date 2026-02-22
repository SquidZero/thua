using System;
using Godot;

public partial class CameraLook : Camera3D
{
	Vector2 mouseDelta = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
