using Godot;

public partial class VHSShaderController : CanvasLayer
{
    [Export] public float DefaultIntensity = 0.3f;
    [Export] public float HorrorIntensity = 0.8f;
    [Export] public float TransitionSpeed = 2.0f;

    private ColorRect _vhsRect;
    private ShaderMaterial _material;
    private float _currentIntensity;
    private float _targetIntensity;

    public override void _Ready()
    {
        _vhsRect = GetNodeOrNull<ColorRect>("VHSRect");

        if (_vhsRect == null)
        {
            _vhsRect = new ColorRect();
            _vhsRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            _vhsRect.MouseFilter = Control.MouseFilterEnum.Ignore;
            AddChild(_vhsRect);
        }

        var shader = GD.Load<Shader>("res://Shaders/vhs_postprocess.gdshader");
        if (shader != null)
        {
            _material = new ShaderMaterial();
            _material.Shader = shader;
            _vhsRect.Material = _material;
        }

        _currentIntensity = DefaultIntensity;
        _targetIntensity = DefaultIntensity;
        ApplyIntensity(_currentIntensity);

        EventManager.Instance.PanicAttackStarted += OnHorrorStart;
        EventManager.Instance.HorrorTransitionTriggered += OnHorrorStart;
        EventManager.Instance.PanicAttackEnded += OnHorrorEnd;
    }

    public override void _Process(double delta)
    {
        _currentIntensity = Mathf.Lerp(_currentIntensity, _targetIntensity, TransitionSpeed * (float)delta);
        ApplyIntensity(_currentIntensity);
    }

    private void ApplyIntensity(float intensity)
    {
        if (_material == null) return;

        _material.SetShaderParameter("scanline_intensity", intensity * 0.5f);
        _material.SetShaderParameter("noise_intensity", intensity * 0.3f);
        _material.SetShaderParameter("color_bleed", intensity * 0.2f);
        _material.SetShaderParameter("vignette_strength", intensity * 0.4f);
    }

    public void SetIntensity(float intensity)
    {
        _targetIntensity = intensity;
    }

    public void SetHorrorMode()
    {
        _targetIntensity = HorrorIntensity;
    }

    public void SetNormalMode()
    {
        _targetIntensity = DefaultIntensity;
    }

    private void OnHorrorStart()
    {
        SetHorrorMode();
    }

    private void OnHorrorEnd()
    {
        SetNormalMode();
    }
}
