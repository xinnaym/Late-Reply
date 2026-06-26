using Godot;

public partial class TriggerZone : Area3D
{
    [Export] public string TriggerId = "";
    [Export] public bool OneShot = true;
    [Export] public bool TriggerOnEnter = true;
    [Export] public bool TriggerOnExit = false;

    private bool _hasTriggered = false;

    public override void _Ready()
    {
        Monitoring = true;
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!TriggerOnEnter) return;
        if (OneShot && _hasTriggered) return;

        if (body.IsInGroup("Player"))
        {
            _hasTriggered = true;
            OnTriggered();
            EventManager.Instance?.EmitSignal(EventManager.SignalName.TriggerEntered, TriggerId);
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (!TriggerOnExit) return;

        if (body.IsInGroup("Player"))
        {
            EventManager.Instance?.EmitSignal(EventManager.SignalName.TriggerExited, TriggerId);
        }
    }

    protected virtual void OnTriggered()
    {
        GD.Print($"Trigger fired: {TriggerId}");
    }

    public void Reset()
    {
        _hasTriggered = false;
    }
}
