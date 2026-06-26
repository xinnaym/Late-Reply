using Godot;

[GlobalClass]
public partial class ComputerNote : Resource
{
    [Export] public string NoteId = "";
    [Export] public string Title = "";
    [Export] public string Content = "";
    [Export] public bool RevealsMentalState = true;
}
