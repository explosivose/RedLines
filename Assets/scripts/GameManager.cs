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
		StopAllCoroutines();
		changingHyperSpace = false;
		level = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelGenerator>();
		
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
	
	public bool ChangingHyperSpace
	{
		get { return changingHyperSpace; }
	}
	
	public void HyperSpaceIncrement()
	{
		if (!changingHyperSpace)
			StartCoroutine("HyperSpaceTransition");
		GameObject[] hyperm = GameObject.FindGameObjectsWithTag("HyperMatter");
		foreach (GameObject h in hyperm)
		{
			Destroy (h);
		}
		//AudioSource.PlayClipAtPoint();
	}
	
	private IEnumerator HyperSpaceTransition()
	{
		float thrustIncrease = 7.5f;
		
		changingHyperSpace = true;
		Debug.Log("Current HyperSpace: " + HyperSpaceMaker.CurrentHyperSpace.name);
		StartDialogue ("Commander", "Preparing to jump" + new string('.', Random.Range (3,20)), 5f);
		
			
		// flatten out the level
		Debug.Log ("Flattening level...");
		level.SetHyperSpace(HyperSpaceMaker.FlatSpace);
		float wait = 0.25f + level.hyperSpaceLerpTime + (level.levelLength/player.currentSpeed);
		Debug.Log ("waiting for " + wait + " seconds");
		yield return new WaitForSeconds(wait);
		
		// explosive accel!
		Debug.Log("Explosive accel!");
		player.rigidbody.AddForce (Vector3.right * player.maxSpeed * 0.5f, ForceMode.Impulse);
		StartDialogue ("Commander", HyperSpaceMaker.CurrentHyperSpace.name + new string('!', Random.Range (1, 10)), 5f);
		
		// change colours!
		StartCoroutine("ChangeColours",0.5f);
		yield return new WaitForSeconds(0.5f);
		StartCoroutine("ChangeColours",0.5f);
		yield return new WaitForSeconds(0.5f);
		StartCoroutine("ChangeColours",0.5f);
		yield return new WaitForSeconds(0.5f);
		StartCoroutine("ChangeColours",5f);
		// increase player thrust (removing the temporary slow, also)
		StartCoroutine( ChangeThrust(4f, player.thrust + thrustIncrease) );
		
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
		Color P_new = new Color(Random.Range(0.5f,1f), Random.Range(0.5f,1f), Random.Range(0.5f,1f));
		Color S_new = new Color(Random.Range(0.5f,1f), Random.Range(0.5f,1f), Random.Range(0.5f,1f));
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
