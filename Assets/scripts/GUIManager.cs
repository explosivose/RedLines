using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GUIWindow
{
	/* GUIWindow info
	* This class is serializable for the designer to choose window settings
	* from within the unity inspector. Settings include position and size.
	*/
	public enum DimensionMode
	{
		PercentageOfScreen,
		Absolute
	}

	public enum Alignment
	{
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight
	}

	public int DesignHeight;
	public DimensionMode HeightIs = DimensionMode.Absolute;

	public int Height 
	{
		get
		{
			if (HeightIs == DimensionMode.Absolute)
				return DesignHeight;
			else
				return Mathf.RoundToInt(Screen.height * DesignHeight / 100);
		}
		set
		{
			DesignHeight = value;
		}
	}

	public int DesignWidth;
	public DimensionMode WidthIs = DimensionMode.Absolute;

	public int Width
	{
		get
		{
			if (WidthIs == DimensionMode.Absolute)
				return DesignWidth;
			else
				return Mathf.RoundToInt(Screen.width * DesignWidth / 100);
		}
		set
		{
			DesignWidth = value;
		}
	}


	public Alignment Align = Alignment.UpperLeft;

	// Top side of the window in screen pixels
	public int verticalOffset;
	public int Top
	{
		get
		{
			// depends on the alignment mode 
			switch (Align)
			{
			case Alignment.UpperCenter: 
			case Alignment.UpperLeft: 
			case Alignment.UpperRight:
			default:
				return verticalOffset;

			case Alignment.MiddleCenter: 
			case Alignment.MiddleLeft: 
			case Alignment.MiddleRight:
				return Mathf.RoundToInt(Screen.height/2 - Height/2) + verticalOffset;
	
			case Alignment.LowerCenter: 
			case Alignment.LowerLeft: 
			case Alignment.LowerRight:
				return Screen.height - Height - verticalOffset;

			}
		}
		set
		{
			verticalOffset = value;
		}
	}

	// Left side of the window in screen pixels
	public int horizontalOffset;
	public int Left
	{
		get
		{
			// depends on the alignment mode
			switch (Align)
			{
			case Alignment.LowerLeft: 
			case Alignment.MiddleLeft: 
			case Alignment.UpperLeft:
			default:
				return horizontalOffset;
	
			case Alignment.LowerCenter: 
			case Alignment.MiddleCenter: 
			case Alignment.UpperCenter:
				return Mathf.RoundToInt(Screen.width/2 - Width/2) + horizontalOffset;
			
			case Alignment.LowerRight: 
			case Alignment.MiddleRight: 
			case Alignment.UpperRight:
				return Screen.width - Width - horizontalOffset;
		
			}
		}
		set
		{
			horizontalOffset = value;
		}
	}


}


/// <summary>
/// All the GUI code exists in this class.
/// </summary>
public class GUIManager : MonoBehaviour
{
	public int buttonHeight = 25;
	// these are window properties for the designer to edit 
	// using the unity inspector
	public GUIWindow mainMenu = new GUIWindow();
	public GUIWindow scoreBoard = new GUIWindow();
	public GUIWindow options = new GUIWindow();
	public GUIWindow credits = new GUIWindow();
	public GUIWindow dialogue = new GUIWindow();

	public float playerSpeed;


	public void ShowMainMenu()
	{
		state = GUIState.MainMenu;
	}

	public void ShowOptions()
	{
		state = GUIState.Options;
	}

	public void ShowScoreBoard()
	{
		state = GUIState.ScoreBoard;
	}

	public void ShowCredits()
	{
		state = GUIState.Credits;
	}

	public void ShowNoWindows()
	{
		if (showDialogue)
						state = GUIState.ShowDialogue;
				else
						state = GUIState.NoWindows;
	}


	private enum GUIState
	{
		MainMenu,
		ScoreBoard,
		Options,
		Credits,
		ShowDialogue,
		NoWindows
	}

	private GUIState state = GUIState.MainMenu;
	private string playerName = "Player 1";
	private List<Score> scores;
	private bool scoresAreUpdated = false;

	private bool showDialogue = false;
	private string dialogueCharacter = "";
	private string dialogueMessage = "";
	private float dialogueTime = 0f;

	private Rect windowSize = new Rect();
	private GUISkin menuSkin;
	private GUISkin transparentWindow;

	void Awake()
	{
		menuSkin = (GUISkin)Resources.Load ("Menus", typeof(GUISkin));
		transparentWindow = (GUISkin)Resources.Load ("TransparentWindow",typeof(GUISkin));
	}

	/// <summary>
	/// Main Menu GUI function.
	/// </summary>
	void wMainMenu(int windowID)
	{
		// Gap for the header
		GUILayout.Space(15);


		// Game first launched
		if (GameManager.Instance.State == GameManager.GameState.MainMenu) 
		{
			if (GUILayout.Button ("New Game", menuSkin.button, GUILayout.Height (buttonHeight))) 
			{
				GameManager.Instance.NewGame("scene1");
			}
		}


		// Player has died
		if (GameManager.Instance.State == GameManager.GameState.GameOver ) 
		{
			if (GUILayout.Button ("Retry", menuSkin.button, GUILayout.Height (buttonHeight))) 
			{
				GameManager.Instance.NewGame("scene1");
			}
			GUILayout.Space(20);
		}



		// Resume game
		if (GameManager.Instance.State == GameManager.GameState.Pause )
		{
			if (GUILayout.Button ("Resume Game", menuSkin.button, GUILayout.Height (buttonHeight)))
			{
				GameManager.Instance.State = GameManager.GameState.Play;
			}
			GUILayout.Space(20);
			if (GUILayout.Button ("Retry", menuSkin.button, GUILayout.Height (buttonHeight))) 
			{
				GameManager.Instance.NewGame("scene1");
			}
			GUILayout.Space(5);
		}
		
		// Scoreboard
		GUILayout.Space(5);
		if (GUILayout.Button("High Scores", menuSkin.button, GUILayout.Height(buttonHeight)))
			state = GUIState.ScoreBoard;
			
		// Options
		GUILayout.Space(5);
		if ( GUILayout.Button ("Options", menuSkin.button, GUILayout.Height (buttonHeight)) )
			state = GUIState.Options;

		// Credits
		GUILayout.Space(5);
		if (GUILayout.Button ("Credits", menuSkin.button, GUILayout.Height (buttonHeight)))
			state = GUIState.Credits;

		// Quit
		GUILayout.Space(5);
		if ( GUILayout.Button ("Quit", menuSkin.button, GUILayout.Height (buttonHeight)) )
			Application.Quit();
	}

	void wScoreBoard(int windowID)
	{
		GUILayout.Space(15);
		
		if (!scoresAreUpdated)
		{
			scores = ScoreManager.Instance.GetScores();
			scoresAreUpdated = true;
		}
		
		
		GUILayout.Label ("There are " + scores.Count + " scores stored.", menuSkin.label);
		// score list
		for(int i = 0; i < scores.Count; i++)
		{
			GUILayout.Label(scores[i].PlayerName + ": " + scores[i].Distance, menuSkin.label);
		}
		
		// delete high scores button
		GUILayout.Space(20);
		if ( GUILayout.Button ("Wipe Scores", menuSkin.button, GUILayout.Height(buttonHeight)) )
		{
			ScoreManager.Instance.DeleteHighScores();
			scoresAreUpdated = false;
		}
		
		// back to main menu
		GUILayout.Space(20);
		if ( GUILayout.Button ("Main Menu", menuSkin.button, GUILayout.Height(buttonHeight)) )
		{
			state = GUIState.MainMenu;
			scoresAreUpdated = false;
		}
			
	}

	void wOptions(int windowID)
	{
		// Gap for header
		GUILayout.Space(15);

		GUILayout.Label("Player Name:");
		playerName = GUILayout.TextField(playerName);

		// Back to main menu button
		GUILayout.Space(5);
		if ( GUILayout.Button ("Main Menu", menuSkin.button, GUILayout.Height(buttonHeight)) )
		{
			PlayerPrefs.SetString("playerName", playerName);
			state = GUIState.MainMenu;
		}
	}

	void wCredits(int windowID)
	{
		GUILayout.Space (15);

		GUILayout.Label ("Primary author: Matt 'explosivose' Blickem");
		GUILayout.Space (5);
		GUILayout.Label ("Music: 'The Life and Death of a Certain K. Zabriskie, Patriarch'" +
						" by Chris Zabriskie");
		GUILayout.Space (5);
		GUILayout.Label ("SFX: Bfxr, explosivose");
		GUILayout.Space (5);
		GUILayout.Label ("Ship models and flame effects: Tanbouz");
		GUILayout.Space (5);
		GUILayout.Label ("This game was originally created for Ludum Dare 28 (December 2013). " + 
						"The author would like to thank the organisers of Ludum Dare, " + 
						"the developers of the Unity Engine, and all those helpful people " +
						"that answer questions / cries for help on the internet. Special " +
		                "thanks to friends that gave feedback and inspiration!"); 
		GUILayout.Space (5);
		GUILayout.Label ("No barrel rolls were done during the making of this game."); 
		// Back to main menu button
		GUILayout.Space(20);
		if ( GUILayout.Button ("Main Menu", menuSkin.button, GUILayout.Height(buttonHeight)) )
			state = GUIState.MainMenu;

	}


	public void ShowDialogue (string character, string message, float time)
	{
		dialogueCharacter = character;
		dialogueMessage = message;
		dialogueTime = time;
		StartCoroutine ("ShowDialogueMessage");
	}

	IEnumerator ShowDialogueMessage()
	{
		GUIState previousState = state;
		showDialogue = true;
		state = GUIState.ShowDialogue;
		yield return new WaitForSeconds (dialogueTime);
		state = previousState;
		showDialogue = false;
	}

	void wDialogue(int windowID)
	{
		//GUI.skin = transparentWindow;
		//Rect textRect = new Rect (0f, 0f, windowSize.width, windowSize.height);
		//GUI.TextArea (textRect, dialogueMessage);
		GUILayout.Label (dialogueMessage,transparentWindow.window);
	}




	/// <summary>
	/// Called every fixed update.
	/// Decide which window to draw.
	/// </summary>
	void FixedUpdate()
	{
		
		if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.State == GameManager.GameState.Pause) 
		{
			GameManager.Instance.State = GameManager.GameState.Play;
		}
		else if ( Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.State == GameManager.GameState.Play)
		{
			GameManager.Instance.State = GameManager.GameState.Pause;
		}
		
		if (state == GUIState.NoWindows || state == GUIState.ShowDialogue)
		{
			Screen.lockCursor = true;	// hide cursor whilst no GUI is shown
		}
		else
		{
			Screen.lockCursor = false;
		}
	}

	public Texture commander;

	/// <summary>
	/// Draws the window.
	/// </summary>
	void OnGUI()
	{
		GUIWindow thisWindow = new GUIWindow();

		// Copy GUIWindow settings to thisWindow
		switch ( state )
		{
		case GUIState.MainMenu:
			thisWindow = mainMenu;
			break;
		case GUIState.ScoreBoard:
			thisWindow = scoreBoard;
			break;
		case GUIState.Options:
			thisWindow = options;
			break;
		case GUIState.Credits:
			thisWindow = credits;
			break;
		case GUIState.ShowDialogue:
			thisWindow = dialogue;
			break;
		default:
			break;
		}

		windowSize = new Rect(thisWindow.Left, thisWindow.Top, thisWindow.Width, thisWindow.Height);

		// Draw thisWindow (GUILayout.Window)
		switch ( state )
		{
		case GUIState.MainMenu:

			GUILayout.Window (1, windowSize, wMainMenu, "RedLines", menuSkin.window);
			break;
		case GUIState.ScoreBoard:
			GUILayout.Window (1, windowSize, wScoreBoard, "High Scores", menuSkin.window);
			break;
		case GUIState.Options:
			GUILayout.Window (1, windowSize, wOptions, "Options", menuSkin.window);
			break;
		case GUIState.Credits:
			GUILayout.Window (1, windowSize, wCredits, "Credits", menuSkin.window);
			break;
		case GUIState.ShowDialogue:
			GUILayout.Window (1, windowSize, wDialogue, commander, transparentWindow.window);
			break;
		case GUIState.NoWindows:
			break;
		default:
			break;
		}
	}
	
}
