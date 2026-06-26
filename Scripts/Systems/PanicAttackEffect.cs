using Godot;

public partial class PanicAttackEffect : Node
{
    [Export] public float Duration = 15.0f;
    [Export] public float MaxTunnelVision = 0.5f;
    [Export] public float MaxCameraShake = 0.3f;
    [Export] public float MaxBreathingVolume = 0.8f;
    [Export] public float MaxRingVolume = 0.6f;

    private float _elapsed = 0f;
    private bool _isActive = false;
    private Camera3D _camera;
    private AudioStreamPlayer _breathingAudio;
    private AudioStreamPlayer _ringingAudio;
    private ColorRect _tunnelVisionRect;
    private float _shakeIntensity;
    private float _originalFov;

    public override void _Ready()
    {
        EventManager.Instance.PanicAttackStarted += StartPanicAttack;
    }

    public void StartPanicAttack()
    {
        _isActive = true;
        _elapsed = 0f;
        _camera = GetViewport().GetCamera3D();

        if (_camera != null)
        {
            _originalFov = _camera.Fov;
        }

        SetupTunnelVision();
        SetupAudio();

        var tween = CreateTween();
        tween.TweenProperty(this, nameof(_shakeIntensity), MaxCameraShake, Duration * 0.5f);
    }

    private void SetupTunnelVision()
    {
        _tunnelVisionRect = new ColorRect();
        _tunnelVisionRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _tunnelVisionRect.Color = new Color(0, 0, 0, 0);
        _tunnelVisionRect.MouseFilter = Control.MouseFilterEnum.Ignore;
        _tunnelVisionRect.ZIndex = 10;
        GetTree().Root.AddChild(_tunnelVisionRect);

        var shader = GD.Load<Shader>("res://Shaders/vhs_postprocess.gdshader");
        if (shader != null)
        {
            var material = new ShaderMaterial();
            material.Shader = shader;
            _tunnelVisionRect.Material = material;
        }
    }

    private void SetupAudio()
    {
        _breathingAudio = new AudioStreamPlayer();
        _breathingAudio.Name = "BreathingAudio";
        _breathingAudio.VolumeDb = -80;
        AddChild(_breathingAudio);

        _ringingAudio = new AudioStreamPlayer();
        _ringingAudio.Name = "RingingAudio";
        _ringingAudio.VolumeDb = -80;
        AddChild(_ringingAudio);
    }

    public override void _Process(double delta)
    {
        if (!_isActive) return;

        _elapsed += (float)delta;
        float progress = _elapsed / Duration;

        if (progress >= 1.0f)
        {
            EndPanicAttack();
            return;
        }

        ApplyTunnelVision(progress);
        ApplyCameraShake(delta);
        ApplyAudioEffects(progress);
        ApplyEnvironmentChanges(progress);
    }

    private void ApplyTunnelVision(float progress)
    {
        if (_tunnelVisionRect?.Material is ShaderMaterial mat)
        {
            float intensity = Mathf.Lerp(0, MaxTunnelVision, progress);
            mat.SetShaderParameter("vignette_strength", intensity);
        }
    }

    private void ApplyCameraShake(double delta)
    {
        if (_camera == null || _shakeIntensity <= 0) return;

        var offset = new Vector3(
            (float)GD.RandRange(-_shakeIntensity, _shakeIntensity),
            (float)GD.RandRange(-_shakeIntensity * 0.5f, _shakeIntensity * 0.5f),
            0
        );
        _camera.Position += offset;

        float fovOffset = (float)GD.RandRange(-2f, 2f) * _shakeIntensity;
        _camera.Fov = _originalFov + fovOffset;
    }

    private void ApplyAudioEffects(float progress)
    {
        if (_breathingAudio != null)
        {
            float breathingVol = Mathf.Lerp(-80, VolumeToDb(MaxBreathingVolume), progress);
            _breathingAudio.VolumeDb = breathingVol;
        }

        if (_ringingAudio != null)
        {
            float ringVol = Mathf.Lerp(-80, VolumeToDb(MaxRingVolume), progress * 0.5f);
            _ringingAudio.VolumeDb = ringVol;
        }
    }

    private void ApplyEnvironmentChanges(float progress)
    {
        var worldEnv = GetTree().CurrentScene?.GetNodeOrNull<WorldEnvironment>("WorldEnvironment");
        if (worldEnv?.Environment == null) return;

        var env = worldEnv.Environment;
        float darkness = Mathf.Lerp(1.0f, 0.3f, progress);
        env.AmbientLightColor = new Color(darkness, darkness, darkness);
    }

    private void EndPanicAttack()
    {
        _isActive = false;
        _shakeIntensity = 0;

        if (_camera != null)
        {
            _camera.Fov = _originalFov;
        }

        if (_tunnelVisionRect != null)
        {
            var tween = CreateTween();
            tween.TweenProperty(_tunnelVisionRect, "color:a", 0f, 2.0f);
            tween.TweenCallback(Callable.From(() => _tunnelVisionRect.QueueFree()));
        }

        if (_breathingAudio != null)
        {
            var tween = CreateTween();
            tween.TweenProperty(_breathingAudio, "volumeDb", -80f, 2.0f);
        }

        if (_ringingAudio != null)
        {
            var tween = CreateTween();
            tween.TweenProperty(_ringingAudio, "volumeDb", -80f, 2.0f);
        }

        var worldEnv = GetTree().CurrentScene?.GetNodeOrNull<WorldEnvironment>("WorldEnvironment");
        if (worldEnv?.Environment != null)
        {
            worldEnv.Environment.AmbientLightColor = Colors.White;
        }

        EventManager.Instance?.EmitSignal(EventManager.SignalName.PanicAttackEnded);
    }

    public bool IsActive()
    {
        return _isActive;
    }

    private float VolumeToDb(float linear)
    {
        return linear > 0 ? 20 * Mathf.Log(linear) / Mathf.Log(10) : -80;
    }
}
