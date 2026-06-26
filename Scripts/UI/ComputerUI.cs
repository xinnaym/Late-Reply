using Godot;

public partial class ComputerUI : Control
{
    [Export] public string[] Notes = new string[]
    {
        "can't sleep again. it's been 3 days.\n\ntried the pills but they just make me\nfeel like i'm underwater.\n\nmaybe i should up the dose.",
        "mom called. didn't pick up.\nshe'll worry. i'll call back later.\n\ni never call back.",
        "therapist says i should \"get out more\".\neasY for her to say.\n\nshe doesn't know what it's like\nto feel like the walls are closing in\nwhen you step outside.",
        "online friends are the only ones\nwho understand.\n\nthe real world is too loud.\ntoo bright.\ntoo much.",
        "forgot to eat today.\nagain.\n\nthe dishes are piling up.\ni can't even look at them."
    };

    private Control _desktopPanel;
    private Control _chatPanel;
    private Control _notesPanel;
    private Control _browserPanel;
    private Control _musicPanel;
    private Button _chatBtn;
    private Button _notesBtn;
    private Button _browserBtn;
    private Button _musicBtn;
    private Button _closeBtn;
    private RichTextLabel _chatText;
    private RichTextLabel _notesText;
    private Label _browserText;
    private bool _isOpen = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;

        _desktopPanel = GetNode<Control>("DesktopPanel");
        _chatPanel = GetNode<Control>("DesktopPanel/ChatPanel");
        _notesPanel = GetNode<Control>("DesktopPanel/NotesPanel");
        _browserPanel = GetNode<Control>("DesktopPanel/BrowserPanel");
        _musicPanel = GetNode<Control>("DesktopPanel/MusicPanel");

        _chatBtn = GetNode<Button>("DesktopPanel/Taskbar/ChatBtn");
        _notesBtn = GetNode<Button>("DesktopPanel/Taskbar/NotesBtn");
        _browserBtn = GetNode<Button>("DesktopPanel/Taskbar/BrowserBtn");
        _musicBtn = GetNode<Button>("DesktopPanel/Taskbar/MusicBtn");
        _closeBtn = GetNode<Button>("DesktopPanel/Taskbar/CloseBtn");

        _chatText = GetNode<RichTextLabel>("DesktopPanel/ChatPanel/ChatText");
        _notesText = GetNode<RichTextLabel>("DesktopPanel/NotesPanel/NotesText");
        _browserText = GetNode<Label>("DesktopPanel/BrowserPanel/BrowserText");

        _chatBtn.Pressed += () => ShowPanel(_chatPanel);
        _notesBtn.Pressed += () => ShowPanel(_notesPanel);
        _browserBtn.Pressed += () => ShowPanel(_browserPanel);
        _musicBtn.Pressed += () => ShowPanel(_musicPanel);
        _closeBtn.Pressed += CloseComputer;

        LoadChatHistory();
        LoadNotes();

        EventManager.Instance.ComputerOpened += OnComputerOpened;
    }

    public void OpenComputer()
    {
        _isOpen = true;
        Visible = true;
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        ShowPanel(_chatPanel);
        EventManager.Instance?.EmitSignal(EventManager.SignalName.ComputerOpened);
    }

    public void CloseComputer()
    {
        _isOpen = false;
        Visible = false;
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        EventManager.Instance?.EmitSignal(EventManager.SignalName.ComputerClosed);
    }

    private void ShowPanel(Control panel)
    {
        _chatPanel.Visible = false;
        _notesPanel.Visible = false;
        _browserPanel.Visible = false;
        _musicPanel.Visible = false;
        panel.Visible = true;
    }

    private void LoadChatHistory()
    {
        _chatText.AppendText("[color=7ec8e3][b]friend:[/b][/color] hey\n");
        _chatText.AppendText("[color=7ec8e3][b]friend:[/b][/color] you there?\n");
        _chatText.AppendText("[color=7ec8e3][b]friend:[/b][/color] haven't heard from you in a while\n");
    }

    private void LoadNotes()
    {
        _notesText.Clear();
        foreach (var note in Notes)
        {
            _notesText.AppendText($"---\n{note}\n\n");
        }
    }

    public void AddFakeChatMessages()
    {
        _chatText.AppendText("\n[color=e37c7c][b]???:[/b][/color] please answer\n");
        _chatText.AppendText("[color=e37c7c][b]???:[/b][/color] don't leave me\n");
        _chatText.AppendText("[color=e37c7c][b]???:[/b][/color] where are you\n");
        _chatText.AppendText("[color=e37c7c][b]???:[/b][/color] are you okay?\n");
    }

    public void FlickerScreen()
    {
        var tween = CreateTween();
        for (int i = 0; i < 8; i++)
        {
            tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.05);
            tween.TweenProperty(this, "modulate", Colors.White, 0.05);
        }
        tween.TweenCallback(Callable.From(() =>
        {
            _chatText.AppendText("\n[color=ff0000][b]SYSTEM:[/b][/color] TURN AROUND\n");
        }));
    }

    private void OnComputerOpened()
    {
    }

    public bool IsOpen()
    {
        return _isOpen;
    }
}
