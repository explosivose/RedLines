using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>  
{
	/* GameManager info
	
	This class is in charge of:
		*Pausing the game (State = GameState.Pause)
		*Passing messages from the GUI to the game
			load level via NewGame()
			setting audio volume (NOT YET IMPLEMENTED)
			
		*Passing messages from the game to the GUI
			counts number of hypermatter collected
			keeps track of hyperstate
			coordinates hyperspace transitions
	
	This class is a singleton (can only be instantiated once) 	
	*/
	
	
	private static class HyperSpaceMaker
	{
		/* HyperSpaceMaker info
		this class generates data for hyperspace transitions
		*/
		private static string[] hyperSpacePrefix = new string[5]
		{"sub","super","hyper","ultra","extra"};
		
		private static string[] hyperSpaceSuffix = new string[5]
		{"sonic","focus","core","flow","zone"};
		
		private static int prefix = 0;
		private static int suffix = 0;
		
	
		public static HyperSpace nextHyperSpace
		{
			get
			{
				HyperSpace temp = new HyperSpace();
				temp.name = hyperSpacePrefix[prefix++] + hyperSpaceSuffix[suffix];
				
				if (prefix >= hyperSpacePrefix.Length)
				{
					prefix = 0;
					suffix++;
					if (suffix >= hyperSpaceSuffix.Length)
						suffix = 0;
				}
				
				temp.maxMidChange = Random.Range(0f, 100f);
				temp.midSampleRate = Random.value;
				temp.minTop = Random.Range(0f,10f);
				temp.maxTop = Random.Range(temp.minTop, 50f);
				temp.topSampleRate = Random.value;
				temp.minBot = Random.Range(0f,10f);
				temp.maxBot = Random.Range(temp.minBot, 50f);
				temp.botSampleRate = Random.value;
				
				return temp;
			}
		}
	}
	
	public enum GameState
	{
		MainMenu,
		Play,
		Pause,
		GameOver
	}
	
	private GameState state = GameState.MainMenu;
	private GUIManager GUIMan;
	private Player player;
	private Sun sun;
	private LevelGenerator level;

	
	void Awake()
	{
		DontDestroyOnLoad (this);

		GameObject GUIManagerPrefab = (GameObject)Resources.Load ("GUIManager", typeof(GameObject));
		GUIManagerPrefab = Instantiate(GUIManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		DontDestroyOnLoad(GUIManagerPrefab);
		GUIMan = GUIManagerPrefab.GetComponent<GUIManager> ();
		level = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelGenerator>();
	}

	
	void Update()
	{
		if (state == GameState.Play)
		{
			if (player == null)
				player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
			
			if (sun == null)
				sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Sun>();
			
			if (player != null)
				GUIMan.playerSpeed = player.currentSpeed;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			Debug.Log("penis");
			level.hyperSpace = HyperSpaceMaker.nextHyperSpace;
		}
	}
	
	/* Public GameManager interfaces
	 * 
	 * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	 */
	public GameState State 
	{
		get 
		{ 
			return state; 
		} 
		set
		{
			switch (value)
			{
			case GameState.MainMenu:
				Application.LoadLevel ("mainmenu");
				state = GameState.MainMenu;
				GUIMan.ShowMainMenu();
				break;
			case GameState.Play:
				state = GameState.Play;
				Time.timeScale = 1f;
				GUIMan.ShowNoWindows();
				break;
			case GameState.Pause:
				state = GameState.Pause;
				Time.timeScale = 0f;
				GUIMan.ShowMainMenu();
				break;
			case GameState.GameOver:
				state = GameState.GameOver;
				GUIMan.ShowMainMenu();
				break;
			default:
				Debug.LogError("Tried to set GameManager.State to invalid value.");
				break;
			}
		}
	}

	public float HyperLerp
	{
		get
		{
			if (player != null)
				return player.currentSpeed / player.maxSpeed;
			else
				return 0f;
		}
	}	
	
	public void NewGame(int level)
	{
		Application.LoadLevel (level);
		State = GameState.Play;
	}

	public void NewGame(string level)
	{
		Application.LoadLevel (level);
		State = GameState.Play;
	}

	public void StartDialogue(string character, string message, float showTime)
	{
		GUIMan.ShowDialogue (character, message, showTime);
	}
	
	public void LevelUp(Color fastColour)
	{
		
		sun.slowColor = sun.fastColor;
		sun.fastColor = fastColour;
	}
}
