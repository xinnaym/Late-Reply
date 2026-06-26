using Godot;

public partial class GameManager : Node3D
{
	private PlayerController _player;
	private PhoneUI _phoneUI;
	private ComputerUI _computerUI;
	private DialogueUI _dialogueUI;
	private PanicAttackEffect _panicAttack;
	private VHSShaderController _vhsEffect;
	private Interactable _pcInteractable;
	private Interactable _phoneInteractable;
	private bool _introPlayed = false;
	private bool _firstMessageShown = false;
	private bool _secondMessageShown = false;
	private bool _panicAttackTriggered = false;
	private bool _horrorTransitionTriggered = false;

	public override void _Ready()
	{
		var root = GetParent();
		_player = root.GetNode<PlayerController>("Player");
		_phoneUI = root.GetNode<PhoneUI>("PhoneUI");
		_computerUI = root.GetNode<ComputerUI>("ComputerUI");
		_dialogueUI = root.GetNode<DialogueUI>("DialogueUI");
		_panicAttack = root.GetNode<PanicAttackEffect>("PanicAttackEffect");
		_vhsEffect = root.GetNode<VHSShaderController>("VHSEffect");

		_pcInteractable = root.GetNode<Interactable>("PCInteractable");
		_phoneInteractable = root.GetNode<Interactable>("PhoneInteractable");

		if (_pcInteractable != null)
		{
			_pcInteractable.InteractionPrompt = "Use Computer";
			_pcInteractable.SetMeta("type", "computer");
		}

		if (_phoneInteractable != null)
		{
			_phoneInteractable.InteractionPrompt = "Check Phone";
			_phoneInteractable.SetMeta("type", "phone");
		}

		EventManager.Instance.PlayerInteracted += OnPlayerInteracted;
		EventManager.Instance.PhoneReplied += OnPhoneReplied;
		EventManager.Instance.ComputerClosed += OnComputerClosed;
		EventManager.Instance.TriggerEntered += OnTriggerEntered;

		CallDeferred(nameof(StartIntro));
	}

	private void StartIntro()
	{
		if (_introPlayed) return;
		_introPlayed = true;

		GetTree().CreateTimer(2.0).Timeout += () =>
		{
			SafeShowDialogue("???", "where... am I?");
		};

		GetTree().CreateTimer(5.0).Timeout += () =>
		{
			SafeShowDialogue("You", "my room... it's night.");
		};

		GetTree().CreateTimer(9.0).Timeout += () =>
		{
			SafeShowDialogue("You", "how long was I asleep?");
			GetTree().CreateTimer(4.0).Timeout += () =>
			{
				if (!_firstMessageShown)
				{
					ShowFirstPhoneMessage();
				}
			};
		};
	}

	private void SafeShowDialogue(string speaker, string text, System.Action onComplete = null)
	{
		if (_phoneUI != null && _phoneUI.IsOpen()) return;
		if (_computerUI != null && _computerUI.IsOpen()) return;
		_dialogueUI?.ShowDialogue(speaker, text, onComplete);
	}

	private void ShowFirstPhoneMessage()
	{
		if (_firstMessageShown) return;
		_firstMessageShown = true;
		_phoneUI?.ShowPhone(_phoneUI.FirstMessage);
	}

	private void OnPlayerInteracted(Node interactable)
	{
		if (interactable == null) return;

		if (interactable.HasMeta("type"))
		{
			string type = (string)interactable.GetMeta("type");

			if (type == "computer")
			{
				_computerUI?.OpenComputer();
				_player?.SetCanMove(false);
			}
			else if (type == "phone")
			{
				if (!_firstMessageShown)
				{
					ShowFirstPhoneMessage();
				}
				else if (_firstMessageShown && !_secondMessageShown)
				{
					ShowSecondPhoneMessage();
				}
			}
		}
	}

	private void OnPhoneReplied(int choiceIndex)
	{
		_player?.SetCanMove(true);

		string[] responses = new string[]
		{
			"She typed a reply. Maybe tomorrow will be different.",
			"She put the phone down. Some things are easier to ignore.",
            "She stared at the screen, then looked away."
		};

		if (choiceIndex >= 0 && choiceIndex < responses.Length)
		{
			GetTree().CreateTimer(2.0).Timeout += () =>
			{
				SafeShowDialogue("Narrator", responses[choiceIndex]);
			};
		}

		if (!_secondMessageShown)
		{
			GetTree().CreateTimer(8.0).Timeout += () =>
			{
				ShowSecondPhoneMessage();
			};
		}
	}

	private void ShowSecondPhoneMessage()
	{
		if (_secondMessageShown) return;
		_secondMessageShown = true;

		SafeShowDialogue("???", "are you okay?", OnSecondMessageDismissed);
	}

	private void OnSecondMessageDismissed()
	{
		_phoneUI?.ShowFakeMessages();
		_computerUI?.AddFakeChatMessages();

		GetTree().CreateTimer(3.0).Timeout += () =>
		{
			if (!_panicAttackTriggered)
			{
				TriggerPanicAttack();
			}
		};
	}

	private void TriggerPanicAttack()
	{
		if (_panicAttackTriggered) return;
		_panicAttackTriggered = true;

		_panicAttack?.StartPanicAttack();
		_player?.SetCanMove(false);

		GetTree().CreateTimer(3.0).Timeout += () =>
		{
			SafeShowDialogue("You", "what's happening to me...");
		};

		GetTree().CreateTimer(_panicAttack?.Duration ?? 15f).Timeout += () =>
		{
			_player?.SetCanMove(true);
			TriggerHorrorTransition();
		};
	}

	private void TriggerHorrorTransition()
	{
		if (_horrorTransitionTriggered) return;
		_horrorTransitionTriggered = true;

		_computerUI?.FlickerScreen();
		EventManager.Instance?.EmitSignal(EventManager.SignalName.HorrorTransitionTriggered);

		GetTree().CreateTimer(5.0).Timeout += () =>
		{
			TurnOffLights();
		};
	}

	private void TurnOffLights()
	{
		var lights = GetTree().GetNodesInGroup("Lights");
		foreach (var light in lights)
		{
			if (light is Light3D light3D)
			{
				var tween = CreateTween();
				tween.TweenProperty(light3D, "light_energy", 0f, 2.0f);
			}
		}

		EventManager.Instance?.EmitSignal(EventManager.SignalName.LightsOut);

		GetTree().CreateTimer(3.0).Timeout += () =>
		{
			SafeShowDialogue("System", "PROTOTYPE END\n\nThank you for playing.\n\nThis was Late Reply.");
		};
	}

	private void OnComputerClosed()
	{
		_player?.SetCanMove(true);
	}

	private void OnTriggerEntered(string triggerId)
	{
		GD.Print($"Trigger entered: {triggerId}");
	}
}
