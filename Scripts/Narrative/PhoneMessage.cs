using Godot;

[GlobalClass]
public partial class PhoneMessage : Resource
{
    [Export] public string Sender = "friend";
    [Export] public string MessageText = "";
    [Export] public bool IsFromPlayer = false;
    [Export] public float DelayBeforeShow = 0f;
}
