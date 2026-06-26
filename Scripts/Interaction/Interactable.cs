using Godot;

public partial class Interactable : StaticBody3D
{
    [Export] public string InteractionPrompt = "Interact";
    [Export] public bool HighlightEnabled = true;

    private MeshInstance3D _mesh;
    private StandardMaterial3D _originalMaterial;
    private StandardMaterial3D _highlightMaterial;

    public override void _Ready()
    {
        AddToGroup("Interactable");

        _mesh = GetNodeOrNull<MeshInstance3D>("Mesh");

        if (_mesh != null && _mesh.MaterialOverride is StandardMaterial3D mat)
        {
            _originalMaterial = mat;
        }

        _highlightMaterial = new StandardMaterial3D();
        _highlightMaterial.AlbedoColor = new Color(1, 1, 1, 0.15f);
        _highlightMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        _highlightMaterial.EmissionEnabled = true;
        _highlightMaterial.Emission = new Color(0.5f, 0.7f, 1.0f);
        _highlightMaterial.EmissionEnergyMultiplier = 0.3f;

        CollisionLayer = 4;
        CollisionMask = 1;
    }

    public virtual void Interact()
    {
        GD.Print($"Interacted with: {Name}");
    }

    public void SetHighlighted(bool highlighted)
    {
        if (_mesh == null || !HighlightEnabled) return;

        if (highlighted)
        {
            _mesh.MaterialOverride = _highlightMaterial;
        }
        else
        {
            _mesh.MaterialOverride = _originalMaterial;
        }
    }

    public string GetPrompt()
    {
        return InteractionPrompt;
    }
}
