using Godot;

public partial class PlayerController : CharacterBody3D
{
    [Export] public float WalkSpeed = 3.0f;
    [Export] public float SprintSpeed = 5.5f;
    [Export] public float Acceleration = 10.0f;
    [Export] public float RotationSpeed = 12.0f;
    [Export] public float Gravity = 9.8f;

    private Vector3 _direction;
    private float _currentSpeed;
    private bool _canMove = true;
    private bool _isSprinting;
    private ThirdPersonCamera _camera;

    public override void _Ready()
    {
        _currentSpeed = WalkSpeed;
        _camera = GetNodeOrNull<ThirdPersonCamera>("ThirdPersonCamera");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_canMove)
        {
            Velocity = Vector3.Zero;
            MoveAndSlide();
            return;
        }

        HandleInput();

        if (!IsOnFloor())
        {
            Velocity = new Vector3(Velocity.X, Velocity.Y - Gravity * (float)delta, Velocity.Z);
        }
        else
        {
            Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
        }

        if (_direction != Vector3.Zero)
        {
            var targetSpeed = _isSprinting ? SprintSpeed : WalkSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Acceleration * (float)delta);

            var targetVelocity = _direction * _currentSpeed;
            Velocity = new Vector3(
                Mathf.Lerp(Velocity.X, targetVelocity.X, Acceleration * (float)delta),
                Velocity.Y,
                Mathf.Lerp(Velocity.Z, targetVelocity.Z, Acceleration * (float)delta)
            );
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Acceleration * (float)delta);
            Velocity = new Vector3(
                Mathf.Lerp(Velocity.X, 0, Acceleration * (float)delta),
                Velocity.Y,
                Mathf.Lerp(Velocity.Z, 0, Acceleration * (float)delta)
            );
        }

        if (_camera != null)
        {
            var targetAngle = _camera.Yaw;
            var currentAngle = Rotation.Y;
            Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(currentAngle, targetAngle, RotationSpeed * (float)delta), Rotation.Z);
        }

        MoveAndSlide();
    }

    private void HandleInput()
    {
        _isSprinting = Input.IsActionPressed("sprint");

        var inputDir = Vector2.Zero;
        if (Input.IsActionPressed("move_forward")) inputDir.Y += 1;
        if (Input.IsActionPressed("move_backward")) inputDir.Y -= 1;
        if (Input.IsActionPressed("move_left")) inputDir.X -= 1;
        if (Input.IsActionPressed("move_right")) inputDir.X += 1;

        if (inputDir != Vector2.Zero)
        {
            inputDir = inputDir.Normalized();
        }

        var camera = GetViewport().GetCamera3D();
        if (camera != null)
        {
            var cameraForward = -camera.GlobalTransform.Basis.Z;
            var cameraRight = camera.GlobalTransform.Basis.X;
            cameraForward.Y = 0;
            cameraRight.Y = 0;
            cameraForward = cameraForward.Normalized();
            cameraRight = cameraRight.Normalized();

            _direction = (cameraRight * inputDir.X + cameraForward * inputDir.Y).Normalized();
        }
        else
        {
            _direction = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
        }
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
        if (!canMove)
        {
            _direction = Vector3.Zero;
        }
    }

    public bool IsMoving()
    {
        return _direction != Vector3.Zero && _currentSpeed > 0.1f;
    }

    public bool IsSprinting()
    {
        return _isSprinting && IsMoving();
    }
}
