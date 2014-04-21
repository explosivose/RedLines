using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> 
{
	private Rect windowSize = new Rect();
	private GUISkin menuSkin;
	private string buildDate = "unknown";
	
	private enum GameState
	{
		PreGame,
		Paused,
		Playing,
		GameOver
	}
	private GameState state = GameState.PreGame;
	
	public bool IsPlaying
	{
		get 
		{
			return (state == GameState.Playing);
		}
	}
	
	private string playerName;
	private int hints;
	
	void LoadSettings()
	{
		playerName = PlayerPrefs.GetString("playerName", "mingebag");
		AudioListener.volume = PlayerPrefs.GetFloat("audioVolume", 0.75f);
		hints = PlayerPrefs.GetInt("hints", 1);
	}
	void SaveSettings()
	{
		PlayerPrefs.SetString("playerName", playerName);
		PlayerPrefs.SetFloat("audioVolume", AudioListener.volume);
		PlayerPrefs.SetInt("hints", hints);
	}
	
	public bool ShowHints
	{
		get { return (hints != 0); }
	}
	
	void StartGame()
	{
		LevelGenerator.Reset();
		Application.LoadLevel("RedLines");
		UnPause();
	}
	
	public void Pause()
	{
		Screen.lockCursor = false;
		state = GameState.Paused;
		gui = GUIState.PauseMenu;
		Time.timeScale = 0f;
	}
	
	public void UnPause()
	{
		Time.timeScale = 1f;
		Screen.lockCursor = true;
		state = GameState.Playing;
		gui = GUIState.NoWindows;
	}
	
	private int score;
	public void GameOver(int gameScore)
	{
		Screen.lockCursor = false;
		state = GameState.GameOver;
		gui = GUIState.DeathMenu;
		score = gameScore;
		CubeMaster.Instance.cubeAccel = 0f;
		CubeMaster.Instance.cubeSpeed = 0.5f;
	}
	
	void Awake()
	{
		DontDestroyOnLoad(this);
		menuSkin = (GUISkin)Resources.Load("Menus", typeof(GUISkin));
		LoadSettings();
		// unfortunately this code doesnt work or even fail safely with a Unity Webplayer build :-((
		//DateTime buildTime = RetrieveLinkerTimestamp();
		//buildDate = String.Format("{0:d/M/yyyy HH:mm:ss}", buildTime);
	}
	
	// This function automatically retreives build date
	// Found it here: http://stackoverflow.com/questions/1600962/displaying-the-build-date
	private DateTime RetrieveLinkerTimestamp()
	{
		string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
		const int c_PeHeaderOffset = 60;
		const int c_LinkerTimestampOffset = 8;
		byte[] b = new byte[2048];
		System.IO.Stream s = null;
		
		try
		{
			s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			s.Read(b, 0, 2048);
		}
		finally
		{
			if (s != null)
			{
				s.Close();
			}
		}
		
		int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
		int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
		DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
		dt = dt.AddSeconds(secondsSince1970);
		dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
		return dt;
	}
	
	void Update()
	{
		// grab the mouse cursor while playing
		if (!Screen.lockCursor && state == GameState.Playing)
		{
			Pause();
		}
		if (Input.GetKeyUp(KeyCode.Escape) && state == GameState.Playing)
			Pause();
	}
	
	private enum GUIState
	{
		NoWindows,
		MainMenu,
		PauseMenu,
		DeathMenu,
		Scores,
		Options,
		Credits
	}
	private GUIState gui = GUIState.MainMenu;
	
	public GUIWindow mainMenu = new GUIWindow();
	void wMainMenu(int windowID)
	{
		GUILayout.Space(menuSkin.window.fontSize);
		GUILayout.Space(Screen.height/8f);
		
		if (state == GameState.Paused)
		{
			if (GUILayout.Button("RESUME", menuSkin.button))
				UnPause();
			GUILayout.Space(20);
		}
		
		if (GUILayout.Button ("START", menuSkin.button))
			StartGame();
		
		if (GUILayout.Button("SCOREBOARD", menuSkin.button))
			gui = GUIState.Scores;
			
		if (GUILayout.Button("OPTIONS", menuSkin.button))
			gui = GUIState.Options;
			
		if (GUILayout.Button("CREDITS", menuSkin.button))
			gui = GUIState.Credits;
			
		if (!Application.isWebPlayer)
		{
			if (GUILayout.Button("QUIT", menuSkin.button))
				Application.Quit();
		}
	}
	
	public GUIWindow deathMenu = new GUIWindow();
	void wDeathMenu(int windowID)
	{
		GUILayout.Space(menuSkin.window.fontSize);
		GUILayout.Space(Screen.height/8f);
		
		if (GUILayout.Button("MAIN MENU", menuSkin.button))
		{
			SaveSettings();
			ScoreBoard.NewScore(score);
			gui = GUIState.MainMenu;
		}
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("PILOTNAME:", menuSkin.label);
		playerName = GUILayout.TextField(playerName, 16, menuSkin.textField);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("SCORE:", menuSkin.label);
		GUILayout.Label(score.ToString(), menuSkin.label);
		GUILayout.EndHorizontal();
		
		if (GUILayout.Button("AGAIN", menuSkin.button))
		{
			SaveSettings();
			ScoreBoard.NewScore(score);
			StartGame();
		}
		GUILayout.Space (10);

	}
	
	public GUIWindow pauseMenu = new GUIWindow();
	void wPauseMenu(int windowID)
	{
		GUILayout.Space(menuSkin.window.fontSize);
		GUILayout.Space(Screen.height/8f);
		
		if (GUILayout.Button("RESUME", menuSkin.button))
			UnPause();
			
		GUILayout.Space (10);
		
		if (GUILayout.Button("MAIN MENU", menuSkin.button))
			gui = GUIState.MainMenu;
		
		if (!Application.isWebPlayer)
		{
			if (GUILayout.Button("QUIT", menuSkin.button))
				Application.Quit();
		}
	}
	
	public GUIWindow scoreBoard = new GUIWindow();
	void wScoreBoard(int windowID)
	{
		GUILayout.Space(menuSkin.window.fontSize);
		GUILayout.Space(Screen.height/8f);
		
		if (GUILayout.Button("MAIN MENU", menuSkin.button))
			gui = GUIState.MainMenu;
		
		List<Score> scores = ScoreBoard.GetScores();
		
		for (int i = 0; i < scores.Count; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label((i+1).ToString() + ". " + scores[i].player, menuSkin.label);
			GUILayout.Label(scores[i].score.ToString(), menuSkin.label);
			GUILayout.EndHorizontal();
		}
		
		if (GUILayout.Button("WIPE SCORES", menuSkin.button))
			ScoreBoard.DeleteSavedScores();
	}
	
	public GUIWindow options = new GUIWindow();
	void wOptions(int windowID)
	{
		bool h = (hints == 0);
		
		GUILayout.Space(menuSkin.window.fontSize);
		GUILayout.Space(Screen.height/8f);
		
		
		if (GUILayout.Button("MAIN MENU", menuSkin.button))
		{
			SaveSettings();
			gui = GUIState.MainMenu;
		}
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("PILOTNAME:", menuSkin.label);
		playerName = GUILayout.TextField(playerName, 16, menuSkin.textField);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("AUDIOVOLUME:", menuSkin.label);
		AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume, 0f, 1f, menuSkin.horizontalSlider, menuSkin.horizontalSliderThumb);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("GAME HINTS:", menuSkin.label);
		h = GUILayout.Toggle(h, "", menuSkin.toggle);
		GUILayout.EndHorizontal();
		
		if (h) hints = 1;
		else hints = 0;

	}
	
	public GUIWindow credits = new GUIWindow();
	void wCredits(int windowID)
	{
		GUILayout.Space(menuSkin.window.fontSize);
		GUILayout.Label("Build date: " + buildDate, menuSkin.label);
		GUILayout.Space(Screen.height/8);
		if (GUILayout.Button("MAIN MENU", menuSkin.button))
			gui = GUIState.MainMenu;
		GUILayout.Space (20f);
		GUILayout.Label("HTTP://SUPERCORE.CO.UK", menuSkin.label);

	}

	void OnGUI()
	{
		GUIWindow currentWindow = new GUIWindow();
		
		// Copy GUIWindow settings to thisWindow
		switch ( gui )
		{
		case GUIState.MainMenu:
			currentWindow = mainMenu;
			break;
		case GUIState.DeathMenu:
			currentWindow = deathMenu;
			break;
		case GUIState.PauseMenu:
			currentWindow = pauseMenu;
			break;
		case GUIState.Scores:
			currentWindow = scoreBoard;
			break;
		case GUIState.Options:
			currentWindow = options;
			break;
		case GUIState.Credits:
			currentWindow = credits;
			break;
		default:
			break;
		}
		
		windowSize = new Rect(currentWindow.Left, currentWindow.Top, currentWindow.Width, currentWindow.Height);
		
		// Draw thisWindow (GUILayout.Window)
		switch ( gui )
		{
		case GUIState.MainMenu:
			GUILayout.Window (1, windowSize, wMainMenu, "REDLINES", menuSkin.window);
			break;
		case GUIState.DeathMenu:
			GUILayout.Window (1, windowSize, wDeathMenu, "HYPERDUMP!", menuSkin.window);
			break;
		case GUIState.PauseMenu:
			GUILayout.Window (1, windowSize, wPauseMenu, "PAUSED", menuSkin.window);
			break;
		case GUIState.Scores:
			GUILayout.Window (1, windowSize, wScoreBoard, "SCORES", menuSkin.window);
			break;
		case GUIState.Credits:
			GUILayout.Window (1, windowSize, wCredits, "CREDITS", menuSkin.window);
			break;
		case GUIState.Options:
			GUILayout.Window (1, windowSize, wOptions, "OPTIONS", menuSkin.window);
			break;
		case GUIState.NoWindows:
			break;
		default:
			break;
		}
		
	}
}






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
