using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float maxSpeed  = 10f;
	public float turnSpeed = 100f;
	public Transform deathsplosion;
	public bool isDead = false;
	public int maxHyperMatter = 16;
	
	public AudioClip[] audioGameStart;
	public AudioClip[] audioDeath;
	public AudioClip[] audioHyperDustPickup;
	public AudioClip[] audioHyperJumpReady;
	public AudioClip[] audioHyperJumpEnter;
	public AudioClip[] audioHyperJumpExit;
	
	private float currentScore;
	private Vector3 direction = Vector3.zero;
	private Vector3 targetPosition = Vector3.zero;
	private bool hyperJump = false;
	private int hyperMatter = 0;
	private int newHyperMatter = 0;
	private float hyperJumpCount = 0;
	private ParticleSystem hyperSpaceEffect;
	private TextMesh guiHyperMatter;
	private TextMesh guiSpeed;
	private TextMesh guiScore;
	private Transform guiHyperSpaceHint;
	
	public int Score
	{
		get { return Mathf.RoundToInt(currentScore * 100); }
	}
	
	// Use this for initialization
	void Start () 
	{
		targetPosition = transform.position;
		hyperSpaceEffect = transform.FindChild("HyperSpaceEffect").particleSystem;
		guiHyperMatter = transform.FindChild("guiHyperValue").GetComponent<TextMesh>();
		guiSpeed =       transform.FindChild("guiSpeedValue").GetComponent<TextMesh>();
		guiScore		=transform.FindChild("guiScoreValue").GetComponent<TextMesh>();
		guiHyperSpaceHint = transform.FindChild("guiHyperSpaceHint");
		PlayRandomSound(audioGameStart, transform.position);
		StartCoroutine( HyperDustPickupQueue() );
	}
	

	void Update () 
	{
		GUIUpdate();
		if (isDead || hyperJump)
			return;
		
		currentScore += (Time.deltaTime * CubeMaster.Instance.cubeSpeed)+(10*hyperJumpCount*Time.deltaTime);
		
		MovementUpdate();
		WeaponUpdate();
	}
	
	void MovementUpdate()
	{
		// decide which input to take
		// read mouse axis and other axis inputs
		// use the one with the larger magnitude
		float mx = Input.GetAxis ("Mouse X");	// mouse
		float ha = Input.GetAxis("Horizontal"); // other axis
		float h = ha;
		if (Mathf.Abs(mx) > Mathf.Abs(ha)) h = mx;
		float my = -Input.GetAxis ("Mouse Y");
		float va = -Input.GetAxis("Vertical");
		float v = va;
		if (Mathf.Abs(my) > Mathf.Abs(va)) v = my;
		
		// calculate rotation based on input
		
		float maxrot = 60f;
		Vector3 vrot = transform.rotation.eulerAngles;
		
		// left/right rotation
		vrot.y += h * turnSpeed * Time.deltaTime;
		if (vrot.y > maxrot		  && vrot.y <  90f) vrot.y =  maxrot; // clamp between 0+max and 0-max
		if (vrot.y <  360f-maxrot && vrot.y > 270f) vrot.y =  360f-maxrot;
		
		// up/down rotation
		vrot.x += v * turnSpeed * Time.deltaTime;
		if (vrot.x > maxrot      && vrot.x <  90f) vrot.x = maxrot;
		if (vrot.x < 360f-maxrot && vrot.x > 270f) vrot.x = 360f-maxrot;
		
		// calculate movement based on rotation
		
		// need to avoid the wrap around from 0deg to 359deg for movement
		// i.e. 359deg becomes -1deg
		if (vrot.y > 270f) vrot.y -= 360f;
		if (vrot.x > 270f) vrot.x -= 360f;
		
		float x = Rescale(vrot.y, maxrot, -maxrot, 1f, -1f);
		float y = Rescale(vrot.x, maxrot, -maxrot, 1f, -1f);
		// move direction relative to camera orientation
		direction = Camera.main.transform.TransformDirection(new Vector3(x, -y));
		direction.z = 0f;	// remove any Z component added by TransformDirection()
		targetPosition += direction * maxSpeed * Time.deltaTime;
		
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 16f);
		transform.rotation = Quaternion.Euler(vrot);
		
	}
	
	void WeaponUpdate()
	{
		if (Input.GetButton("Fire1"))
		{
			BroadcastMessage("Fire");
		}
		if (Input.GetKey(KeyCode.Space))
		{
			if (!hyperJump && hyperMatter == maxHyperMatter) 
				StartCoroutine( HyperJump() );
		}
	}
	
	void GUIUpdate()
	{
		if (hyperJump || isDead)
		{
			guiSpeed.text = "----";
			guiHyperMatter.text = "----";
		}
		else
		{
			guiSpeed.text = (Mathf.RoundToInt(100*CubeMaster.Instance.cubeSpeed)).ToString();
			guiHyperMatter.text = (100 * hyperMatter / maxHyperMatter).ToString() + "%";
		}
		
		
		guiScore.text = Score.ToString();
		
		guiHyperSpaceHint.renderer.enabled = (hyperMatter == maxHyperMatter && GameManager.Instance.ShowHints);
	}
	
	IEnumerator HyperJump()
	{
		hyperJump = true;
		PlayRandomSound(audioHyperJumpEnter, transform.position);
		hyperMatter = 0;
		LevelGenerator.Reset(transform.position);
		LevelGenerator.LockPosition();
		LevelGenerator.Obstacles = false;
		CubeMaster.Instance.HyperJump = true;
		CubeMaster.Instance.SpeedChange(1.75f);
		ScreenShake.Instance.Shake(0.5f,1f/CubeMaster.Instance.CubeTravelTime);
		hyperSpaceEffect.time = 0f;
		hyperSpaceEffect.playbackSpeed = hyperSpaceEffect.duration/CubeMaster.Instance.CubeTravelTime;
		hyperSpaceEffect.Play();
		yield return new WaitForSeconds(CubeMaster.Instance.CubeTravelTime);
		CubeMaster.Instance.HyperJump = false;
		PlayRandomSound(audioHyperJumpExit, transform.position);
		ScreenShake.Instance.Shake(0.5f,0.5f);
		hyperJumpCount++;
		yield return new WaitForSeconds(0.5f);
		LevelGenerator.Unlock();
		LevelGenerator.Obstacles = true;
		hyperJump = false;
	}	
	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Death" && !isDead)
		{
			if (!isDead) StartCoroutine( Death() );
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (isDead) return;
		if (col.gameObject.tag == "HyperDust")
		{
			if (hyperMatter + newHyperMatter < maxHyperMatter) 
			{
				newHyperMatter++;
			}
		}
	}
	
	IEnumerator Death()
	{
		isDead = true;
		float t = PlayRandomSound(audioDeath, transform.position);
		yield return new WaitForSeconds(t);
		Instantiate(deathsplosion, transform.position, transform.rotation);
		maxSpeed = 0f;
		GameManager.Instance.GameOver(Score);
	}
	
	IEnumerator HyperDustPickupQueue()
	{
		while (true)
		{
			if ( newHyperMatter > 0)
			{
				newHyperMatter--;
				hyperMatter++;
				PlayRandomSound(audioHyperDustPickup, transform.position);
				yield return new WaitForSeconds(0.125f);
				if (hyperMatter == maxHyperMatter)
					PlayRandomSound(audioHyperJumpReady, transform.position);
			}
			yield return new WaitForFixedUpdate();
		}
	}
	
	// Rescale a value from old range to new range
	// i.e. 5 is between 1 (min) and 10 (max)
	// Rescale(ref 5, 1, 10, 30, 50) --> 40
	private float Rescale(float value, float oldMax, float oldMin, float newMax, float newMin)
	{
		float oldRange = oldMax - oldMin;
		float newRange = newMax - newMin;
		return (((value - oldMin)*newRange)/oldRange)+newMin;
	}
	
	private float PlayRandomSound(AudioClip[] library, Vector3 position)
	{
		int i = Random.Range(0, library.Length);
		AudioSource.PlayClipAtPoint(library[i], position);
		return library[i].length;
	}
}
