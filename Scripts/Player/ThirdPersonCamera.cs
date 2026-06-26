using Godot;

public partial class ThirdPersonCamera : Camera3D
{
    [Export] public NodePath TargetPath;
    [Export] public Vector3 Offset = new Vector3(0, 1.5f, 2.0f);
    [Export] public float FollowSpeed = 8.0f;
    [Export] public float MouseSensitivity = 0.003f;
    [Export] public float MinVerticalAngle = -30.0f;
    [Export] public float MaxVerticalAngle = 60.0f;

    private Node3D _target;
    private float _yaw;
    private float _pitch = 15.0f;

    public float Yaw => _yaw;

    public override void _Ready()
    {
        if (TargetPath != null)
        {
            _target = GetNode<Node3D>(TargetPath);
        }

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            _yaw -= mouseMotion.Relative.X * MouseSensitivity;
            _pitch -= mouseMotion.Relative.Y * MouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, Mathf.DegToRad(MinVerticalAngle), Mathf.DegToRad(MaxVerticalAngle));
        }

        if (@event.IsActionPressed("pause_menu"))
        {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null) return;

        var targetPos = _target.GlobalPosition;
        var rotation = new Basis(Vector3.Up, _yaw) * new Basis(Vector3.Right, _pitch);
        var cameraOffset = rotation * new Vector3(0, 0, Offset.Z);

        var desiredPosition = targetPos + new Vector3(0, Offset.Y, 0) + cameraOffset;
        GlobalPosition = GlobalPosition.Lerp(desiredPosition, FollowSpeed * (float)delta);

        LookAt(targetPos + new Vector3(0, 1.0f, 0), Vector3.Up);
    }

    public void SetTarget(Node3D target)
    {
        _target = target;
    }
}
