using Godot;

public partial class PhoneUI : Control
{
    [Export] public string FirstMessage = "hey, wanna go for a walk tomorrow?";
    [Export] public string SecondMessage = "are you okay?";

    private Label _messageLabel;
    private VBoxContainer _choicesContainer;
    private Button _positiveBtn;
    private Button _negativeBtn;
    private Button _ignoreBtn;
    private Control _phonePanel;
    private RichTextLabel _chatHistory;

    private bool _isFirstMessage = true;
    private bool _isOpen = false;
    private int _selectedIndex = 0;
    private Button[] _choiceButtons;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;

        _phonePanel = GetNode<Control>("PhonePanel");
        _messageLabel = GetNode<Label>("PhonePanel/VBoxContainer/MessageLabel");
        _chatHistory = GetNode<RichTextLabel>("PhonePanel/VBoxContainer/ChatHistory");
        _choicesContainer = GetNode<VBoxContainer>("PhonePanel/VBoxContainer/ChoicesContainer");
        _positiveBtn = GetNode<Button>("PhonePanel/VBoxContainer/ChoicesContainer/PositiveBtn");
        _negativeBtn = GetNode<Button>("PhonePanel/VBoxContainer/ChoicesContainer/NegativeBtn");
        _ignoreBtn = GetNode<Button>("PhonePanel/VBoxContainer/ChoicesContainer/IgnoreBtn");

        _positiveBtn.Pressed += OnPositiveReply;
        _negativeBtn.Pressed += OnNegativeReply;
        _ignoreBtn.Pressed += OnIgnore;

        _choiceButtons = new Button[] { _positiveBtn, _negativeBtn, _ignoreBtn };

        EventManager.Instance.PhoneMessageReceived += OnPhoneMessageReceived;
    }

    public void ShowPhone(string message)
    {
        _isOpen = true;
        Visible = true;
        _messageLabel.Text = message;
        _choicesContainer.Visible = true;
        _selectedIndex = 0;
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        UpdateSelection();
    }

    private void OnPhoneMessageReceived(string message)
    {
        ShowPhone(message);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_isOpen || !_choicesContainer.Visible) return;

        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (key.Keycode == Key.Up || key.Keycode == Key.W)
            {
                _selectedIndex = (_selectedIndex - 1 + _choiceButtons.Length) % _choiceButtons.Length;
                UpdateSelection();
            }
            else if (key.Keycode == Key.Down || key.Keycode == Key.S)
            {
                _selectedIndex = (_selectedIndex + 1) % _choiceButtons.Length;
                UpdateSelection();
            }
            else if (key.Keycode == Key.Enter || key.Keycode == Key.KpEnter || key.Keycode == Key.Space)
            {
                _choiceButtons[_selectedIndex].EmitSignal("pressed");
            }
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < _choiceButtons.Length; i++)
        {
            _choiceButtons[i].ReleaseFocus();
        }
        _choiceButtons[_selectedIndex].GrabFocus();
    }

    private void OnPositiveReply()
    {
        AddToChatHistory("You", "sure, why not. maybe it'll help.");
        _choicesContainer.Visible = false;
        _messageLabel.Text = "";
        EventManager.Instance?.EmitSignal(EventManager.SignalName.PhoneReplied, 0);
        ClosePhone();
    }

    private void OnNegativeReply()
    {
        AddToChatHistory("You", "not really. maybe some other time.");
        _choicesContainer.Visible = false;
        _messageLabel.Text = "";
        EventManager.Instance?.EmitSignal(EventManager.SignalName.PhoneReplied, 1);
        ClosePhone();
    }

    private void OnIgnore()
    {
        AddToChatHistory("You", "...");
        _choicesContainer.Visible = false;
        _messageLabel.Text = "";
        EventManager.Instance?.EmitSignal(EventManager.SignalName.PhoneReplied, 2);
        ClosePhone();
    }

    private void AddToChatHistory(string sender, string text)
    {
        var color = sender == "You" ? "7ec8e3" : "e37c7c";
        _chatHistory.AppendText($"[color={color}][b]{sender}:[/b][/color] {text}\n");
    }

    private void ClosePhone()
    {
        _isOpen = false;
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;

        GetTree().CreateTimer(1.0).Timeout += () =>
        {
            Visible = false;
        };
    }

    public void ShowFakeMessages()
    {
        _chatHistory.AppendText("[color=e37c7c][b]???:[/b][/color] please answer\n");
        _chatHistory.AppendText("[color=e37c7c][b]???:[/b][/color] don't leave me\n");
        _chatHistory.AppendText("[color=e37c7c][b]???:[/b][/color] where are you\n");
    }

    public bool IsOpen()
    {
        return _isOpen;
    }
}
