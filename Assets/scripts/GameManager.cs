using UnityEngine;
using System.Collections;

public static class HyperSpaceMaker
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
	
	private static HyperSpace currentHyperSpace;
	
	
	public static HyperSpace CurrentHyperSpace
	{
		get
		{
			if ( currentHyperSpace == null )
				currentHyperSpace = new HyperSpace();
			return currentHyperSpace;
		}
		private set 
		{
			currentHyperSpace = value;
		}
	}
	
	public static HyperSpace FlatSpace
	{
		get 
		{
			HyperSpace temp = new HyperSpace();
			temp.name = "";
			temp.maxMidChange = CurrentHyperSpace.maxMidChange;
			temp.midSampleRate = 0f;
			temp.maxTop = 5f;
			temp.minTop = 5f;
			temp.topSampleRate = CurrentHyperSpace.topSampleRate;
			temp.maxBot = 5f;
			temp.minBot = 5f;
			temp.botSampleRate = CurrentHyperSpace.botSampleRate;
			return temp;
		}
	}
	
	public static HyperSpace NewHyperSpace
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
			
			temp.maxMidChange = Random.Range(50f,500f);
			temp.midSampleRate = Random.value/5000f;
			temp.minTop = Random.Range(2f,10f);
			temp.maxTop = Random.Range(temp.minTop, 40f);
			temp.topSampleRate = Random.value/50f;
			temp.minBot = Random.Range(2f,10f);
			temp.maxBot = Random.Range(temp.minBot, 40f);
			temp.botSampleRate = Random.value/50f;
			
			temp.RandomSeeds();
			
			currentHyperSpace = temp;
			return temp;
		}
	}
	
	
}

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
	
	private Color colour1 = Color.red;
	private Color colour2 = Color.green;
	private bool changingHyperSpace = false;
	
	void Awake()
	{
		DontDestroyOnLoad (this);

		GameObject GUIManagerPrefab = (GameObject)Resources.Load ("GUIManager", typeof(GameObject));
		GUIManagerPrefab = Instantiate(GUIManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		DontDestroyOnLoad(GUIManagerPrefab);
		GUIMan = GUIManagerPrefab.GetComponent<GUIManager> ();


		if (PlayerPrefs.HasKey("audioVolume"))
			AudioListener.volume = PlayerPrefs.GetFloat("audioVolume");
	}
	
	void Start()
	{

	}

	void OnLevelWasLoaded()
	{
		level = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelGenerator>();
		//level.SetHyperSpace(HyperSpaceMaker.NewHyperSpace);
		
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Sun>();
	}
	
	void Update()
	{
		if (state == GameState.Play)
		{
			
			if (player != null)
				GUIMan.playerSpeed = player.currentSpeed;
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
	
	public Color ColourPrimary
	{
		get { return colour1; }
	}	
	
	public Color ColourSecondary
	{
		get { return colour2; }
	}
	
	public void HyperSpaceIncrement()
	{
		if (!changingHyperSpace)
			StartCoroutine("HyperSpaceTransition");
	}
	
	private IEnumerator HyperSpaceTransition()
	{
		changingHyperSpace = true;
		Debug.Log("Current HyperSpace: " + HyperSpaceMaker.CurrentHyperSpace.name);
		
		float thrustIncrease = 10f;
			
		// flatten out the level
		Debug.Log ("Flattening level...");
		level.SetHyperSpace(HyperSpaceMaker.FlatSpace);
		yield return new WaitForSeconds(10f);
		
		// explosive accel!
		Debug.Log("Explosive accel!");
		player.rigidbody.AddForce (Vector3.right * player.maxSpeed, ForceMode.Impulse);
		
		// change colours!
		StartCoroutine("ChangeColours",5f);
		
		// increase player thrust (removing the temporary slow, also)
		StartCoroutine( ChangeThrust(4f, player.thrust + thrustIncrease) );
		
		// wait for speed to settle in flat-mode
		yield return new WaitForSeconds(5f);
		
		// new level geometry
		level.SetHyperSpace(HyperSpaceMaker.NewHyperSpace);
		Debug.Log ("New HyperSpace: " + HyperSpaceMaker.CurrentHyperSpace.name);
		yield return new WaitForSeconds(1f);
		changingHyperSpace = false;
	}
	
	private IEnumerator ChangeThrust(float t, float newThrust)
	{
		float start = Time.time;
		float oldThrust = player.thrust;
		while(Time.time < start + t)
		{
			player.thrust = Mathf.Lerp (oldThrust, newThrust,  (Time.time - start)/t );
			yield return new WaitForFixedUpdate();
		}
	}
	
	private IEnumerator ChangeColours(float t)
	{
		float start = Time.time;
		Color P_old = colour1;
		Color S_old = colour2;
		Color P_new = new Color(Random.value, Random.value, Random.value);
		Color S_new = new Color(Random.value, Random.value, Random.value);
		while(Time.time < start + t)
		{
			colour1 = Color.Lerp(P_old, P_new, (Time.time - start)/t );
			colour2 = Color.Lerp(S_old, S_new, (Time.time - start)/t );
			yield return new WaitForFixedUpdate();
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
	
}
