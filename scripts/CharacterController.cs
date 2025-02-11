using Godot;
using System;

public partial class CharacterController : CharacterBody3D // Added the partial modifier here
{
    [Export] public float LookSensitivity = 0.006f;
    [Export] public float JumpVelocity = 6.0f;
    [Export] public bool AutoBhop = true;
    [Export] public float WalkSpeed = 7.0f;
    [Export] public float SprintSpeed = 8.5f;
    [Export] public float GroundAccel = 14.0f;
    [Export] public float GroundDecel = 10.0f;
    [Export] public float GroundFriction = 6.0f;

    [Export] public float AirCap = 0.85f;
    [Export] public float AirAccel = 800.0f;
    [Export] public float AirMoveSpeed = 500.0f;

    private Vector3 _wishDir = Vector3.Zero;

    private float GetMoveSpeed()
    {
        return Input.IsActionPressed("sprint") ? SprintSpeed : WalkSpeed;
    }

    public override void _Ready()
    {
        Console.WriteLine("Hi there this works???");
        foreach (var child in GetNode("WorldModel").GetChildren())
        {
            if (child is VisualInstance3D visualInstance)
            {
                visualInstance.SetLayerMaskValue(1, false);
                visualInstance.SetLayerMaskValue(2, true);
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            if (@event is InputEventMouseMotion mouseMotion)
            {
                RotateY(-mouseMotion.Relative.X * LookSensitivity);

                var head = GetNode<Node3D>("Head");

                // Use quaternion rotation instead of RotateX
                var rotation = head.Rotation;
                rotation.X = Mathf.Clamp(rotation.X + mouseMotion.Relative.Y * LookSensitivity, Mathf.DegToRad(-90), Mathf.DegToRad(90));
                head.Rotation = rotation;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float deltaTime = (float)delta; // Convert to float since PhysicsProcess takes double

        Vector2 inputDir = Input.GetVector("left", "right", "up", "down").Normalized();
        // Replaced Xform() with Basis * Vector3 multiplication
        _wishDir = GlobalTransform.Basis * new Vector3(-inputDir.X, 0.0f, -inputDir.Y);

        if (IsOnFloor())
        {
            if (Input.IsActionJustPressed("jump"))
            {
                Velocity = new Vector3(Velocity.X, JumpVelocity, Velocity.Z);
            }
            HandleGroundPhysics(deltaTime);
        }
        else
        {
            HandleAirPhysics(deltaTime);
        }

        MoveAndSlide();
    }

    private void HandleGroundPhysics(float delta)
    {
        float curSpeedInWishDir = Velocity.Dot(_wishDir);
        float addSpeedTillCap = GetMoveSpeed() - curSpeedInWishDir;
        if (addSpeedTillCap > 0)
        {
            float accelSpeed = GroundAccel * delta * GetMoveSpeed();
            accelSpeed = Mathf.Min(accelSpeed, addSpeedTillCap);
            Velocity += accelSpeed * _wishDir;
        }

        float control = Mathf.Max(Velocity.Length(), GroundDecel);
        float drop = control * GroundFriction * delta;
        float newSpeed = Mathf.Max(Velocity.Length() - drop, 0.0f);

        if (Velocity.Length() > 0)
        {
            newSpeed /= Velocity.Length();
        }

        Velocity *= newSpeed;
    }

    private void HandleAirPhysics(float delta)
    {
        Velocity = new Vector3(Velocity.X, Velocity.Y - ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * delta, Velocity.Z);

        float curSpeedInWishDir = Velocity.Dot(_wishDir);
        float cappedSpeed = Mathf.Min((AirMoveSpeed * _wishDir).Length(), AirCap);
        float addSpeedTillCap = cappedSpeed - curSpeedInWishDir;

        if (addSpeedTillCap > 0)
        {
            float accelSpeed = AirAccel * AirMoveSpeed * delta;
            accelSpeed = Mathf.Min(accelSpeed, addSpeedTillCap);
            Velocity += accelSpeed * _wishDir;
        }
    }

    public override void _Process(double delta)
    {
        // Placeholder for any update logic if needed.
    }
}
