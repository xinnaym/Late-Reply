using Godot;

public partial class DialogueUI : Control
{
    [Export] public float TypingSpeed = 0.03f;

    private Label _speakerLabel;
    private Label _textLabel;
    private Control _dialoguePanel;
    private Timer _typeTimer;
    private string _fullText = "";
    private int _charIndex = 0;
    private bool _isTyping = false;
    private bool _isOpen = false;
    private System.Action _onComplete;
    private Timer _autoCloseTimer;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;

        _dialoguePanel = GetNode<Control>("DialoguePanel");
        _speakerLabel = GetNode<Label>("DialoguePanel/VBoxContainer/SpeakerLabel");
        _textLabel = GetNode<Label>("DialoguePanel/VBoxContainer/TextLabel");

        _typeTimer = new Timer();
        _typeTimer.WaitTime = TypingSpeed;
        _typeTimer.OneShot = false;
        _typeTimer.Timeout += OnTypeTimerTick;
        AddChild(_typeTimer);

        _autoCloseTimer = new Timer();
        _autoCloseTimer.OneShot = true;
        _autoCloseTimer.Timeout += OnAutoClose;
        AddChild(_autoCloseTimer);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isOpen) return;

        if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
        {
            GetViewport().SetInputAsHandled();
            if (_isTyping)
                SkipTyping();
            else
                CloseDialogue();
        }
        else if (@event.IsActionPressed("interact"))
        {
            GetViewport().SetInputAsHandled();
            if (_isTyping)
                SkipTyping();
            else
                CloseDialogue();
        }
    }

    public void ShowDialogue(string speaker, string text, System.Action onComplete = null, float autoCloseDelay = 4.0f)
    {
        _isOpen = true;
        Visible = true;
        _speakerLabel.Text = speaker;
        _fullText = text;
        _charIndex = 0;
        _textLabel.Text = "";
        _isTyping = true;
        _onComplete = onComplete;
        _typeTimer.Start();
        GetTree().Paused = true;

        _autoCloseTimer.Stop();
        _autoCloseTimer.WaitTime = _fullText.Length * TypingSpeed + autoCloseDelay;
        _autoCloseTimer.Start();
    }

    private void OnTypeTimerTick()
    {
        if (_charIndex < _fullText.Length)
        {
            _textLabel.Text += _fullText[_charIndex];
            _charIndex++;
        }
        else
        {
            _typeTimer.Stop();
            _isTyping = false;
        }
    }

    private void SkipTyping()
    {
        _typeTimer.Stop();
        _textLabel.Text = _fullText;
        _isTyping = false;
        _autoCloseTimer.Stop();
        _autoCloseTimer.WaitTime = 2.0f;
        _autoCloseTimer.Start();
    }

    private void OnAutoClose()
    {
        if (_isOpen) CloseDialogue();
    }

    private void CloseDialogue()
    {
        _isOpen = false;
        Visible = false;
        _typeTimer.Stop();
        _autoCloseTimer.Stop();
        GetTree().Paused = false;
        _onComplete?.Invoke();
        _onComplete = null;
    }

    public bool IsOpen()
    {
        return _isOpen;
    }
}
