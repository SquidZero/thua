using Godot;
using System;
using System.Collections.Generic;

public partial class Grapple : Node3D
{
	public PlayerMove player;
	public bool swinging {get; private set;} = false;
	[Export] Vector3 center = Vector3.Zero;
	[Export] Node3D debugBox;
	[Export] Node3D velbox;
	public float r = 1;
	Vector3 playerRotation = Vector3.Zero;
	Vector3 Vc = Vector3.Zero;
	[Export]
	ImmediateMesh line = new();
	[Export]
	MeshInstance3D mesh;
	[Export]
	Material lineMat;
	[Export] Label3D velLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<PlayerMove>();
		mesh.Mesh = line;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.GetActionStrength("swing") > 0.5f)
		{
			if (!swinging)
			{
				swinging = true;
				playerRotation = player.GlobalRotation;
				var initRotation = player.rayCast.Rotation;
				// List<Vector3> colpts = new();
				// for (double i = -0.2f; i < 0.2f; i += 0.05f)
				// {
					// player.rayCast.RotateY((float)i);
					if (player.rayCast.IsColliding() && !((Node3D)player.rayCast.GetCollider()).IsInGroup("player")) {
						// colpts.Add(player.rayCast.GetCollisionPoint());
						center = player.rayCast.GetCollisionPoint();
					// player.rayCast.Rotation = initRotation;
				// }
				} else {//if (colpts.Count == 0){
					// GD.Print("not colliding");
					swinging = false;
					return;
				}

				// center = colpts[0];
				debugBox.Position = center;
				// r = player.Position.DistanceTo(center);
				// player.playerCamera.GetNode<CameraLook>(".").dontAffectPlayerTransform = true;
			}
			r = player.Position.DistanceTo(center);

			// Vc = (center-player.GlobalPosition).Cross(Vector3.Right).Normalized();
			player.a_c = (center-player.GlobalPosition)*r;
			// player.Vc = 16 * Vc.Rotated(Vector3.Up, player.Rotation.Y);

			//GD.Print(player.Vc);
			velbox.GlobalPosition = player.GlobalPosition+Vc;
			// velbox.GlobalRotate(Vector3.Up, Mathf.Atan2(player.ToGlobal(Vector3.Forward).X,center.X));

			velLabel.Text = player.a_c.Length().ToString();
			velLabel.LookAt(player.playerCamera.GlobalPosition, Vector3.Up, true);

			line.ClearSurfaces();
			line.SurfaceBegin(Mesh.PrimitiveType.LineStrip, lineMat);
			line.SurfaceAddVertex(center);
			line.SurfaceAddVertex(player.GlobalPosition + Vc);
			line.SurfaceEnd();

		} else {
			Vc = Vector3.Zero;
			if (swinging) {
				player.GlobalRotation = new Vector3(0, player.playerCamera.GlobalRotation.Y, 0);
				player.playerCamera.Rotation *= new Vector3(1,0,0);
			}
			// player.playerCamera.GetNode<CameraLook>(".").dontAffectPlayerTransform = false;
			
			swinging = false;
		}
	}
}
