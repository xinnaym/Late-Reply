using Godot;

public partial class EventManager : Node
{
    public static EventManager Instance { get; private set; }

    [Signal] public delegate void PhoneMessageReceivedEventHandler(string message);
    [Signal] public delegate void PhoneRepliedEventHandler(int choiceIndex);
    [Signal] public delegate void ComputerOpenedEventHandler();
    [Signal] public delegate void ComputerClosedEventHandler();
    [Signal] public delegate void NoteReadEventHandler(string noteId);
    [Signal] public delegate void ChatHistoryChangedEventHandler();
    [Signal] public delegate void PanicAttackStartedEventHandler();
    [Signal] public delegate void PanicAttackEndedEventHandler();
    [Signal] public delegate void HorrorTransitionTriggeredEventHandler();
    [Signal] public delegate void LightsOutEventHandler();
    [Signal] public delegate void DialogueStartedEventHandler(string dialogueId);
    [Signal] public delegate void DialogueEndedEventHandler(string dialogueId);
    [Signal] public delegate void PlayerInteractedEventHandler(Node interactable);
    [Signal] public delegate void TriggerEnteredEventHandler(string triggerId);
    [Signal] public delegate void TriggerExitedEventHandler(string triggerId);

    public override void _Ready()
    {
        Instance = this;
    }
}
