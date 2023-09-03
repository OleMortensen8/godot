using Godot;
using System;

public partial class player : CharacterBody3D
{
	private Vector3 velocity = new Vector3();

	private const float SPEED = 5.0f;
	private const float JUMP_VELOCITY = 4.5f;

	private float lookSensitivity = 0.01f;
	private float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

	[Export]
	private Camera3D camera;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera3D");
	}

	public void _PhysicsProcess(float delta)
	{
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y -= gravity * delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JUMP_VELOCITY;
		}

		// Get the input direction and handle the movement/deceleration.
		var inputDir = new Vector3(
			Input.GetActionStrength("right") - Input.GetActionStrength("left"),
			0,
			Input.GetActionStrength("down") - Input.GetActionStrength("up")
		);
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Z)).Normalized();
		if (direction.Length() > 0)
		{
			velocity.X = direction.X * SPEED;
			velocity.Z = direction.Z * SPEED;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, SPEED);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, SPEED);
		}

		MoveAndSlide();

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible ? 
				  Input.MouseModeEnum.Captured : 
				  Input.MouseModeEnum.Visible;


		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(-eventMouseMotion.Relative.X * lookSensitivity);
			camera.RotateX(-eventMouseMotion.Relative.Y * lookSensitivity);
			camera.Rotation = new Vector3(
				Mathf.Clamp(camera.Rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2),
				camera.Rotation.Y,
				camera.Rotation.Z
			);
		}
	}
}
