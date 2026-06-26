using Godot;
using System.Collections.Generic;

public partial class InteractionSystem : Node3D
{
    [Export] public float InteractionRange = 4.0f;

    private Camera3D _camera;
    private Interactable _currentInteractable;

    public override void _Ready()
    {
        _camera = GetViewport().GetCamera3D();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_camera == null)
        {
            _camera = GetViewport().GetCamera3D();
            if (_camera == null) return;
        }

        Interactable closest = null;
        float closestDist = InteractionRange;

        var interactables = GetTree().GetNodesInGroup("Interactable");
        foreach (var node in interactables)
        {
            if (node is not Interactable interactable) continue;

            var screenPos = _camera.UnprojectPosition(interactable.GlobalPosition);
            var viewportSize = GetViewport().GetVisibleRect().Size;
            if (screenPos.X < 0 || screenPos.X > viewportSize.X || screenPos.Y < 0 || screenPos.Y > viewportSize.Y)
                continue;

            var toObj = interactable.GlobalPosition - _camera.GlobalPosition;
            var forward = -_camera.GlobalTransform.Basis.Z;
            float dot = forward.Dot(toObj.Normalized());
            if (dot < 0.5f) continue;

            float dist = _camera.GlobalPosition.DistanceTo(interactable.GlobalPosition);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        if (closest != _currentInteractable)
        {
            if (_currentInteractable != null)
                _currentInteractable.SetHighlighted(false);

            _currentInteractable = closest;

            if (_currentInteractable != null)
                _currentInteractable.SetHighlighted(true);
        }

        if (Input.IsActionJustPressed("interact") && _currentInteractable != null)
        {
            _currentInteractable.Interact();
            EventManager.Instance?.EmitSignal(EventManager.SignalName.PlayerInteracted, _currentInteractable);
        }
    }

    public Interactable GetCurrentInteractable()
    {
        return _currentInteractable;
    }
}
