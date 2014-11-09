using UnityEngine;
using System.Collections;

public class Player : Singleton<Player>
{
	public float maxSpeed  = 10f;
	public float turnSpeed = 100f;
	public float acceleration = 10f;
	public float hyperJumpSpeedChange = 2f;
	public Transform deathsplosion;
	public bool isDead = false;
	public bool controlEnabled = false;
	public int maxHyperMatter = 16;
	
	public AudioClip[] audioGameStart;
	public AudioClip[] audioDeath;
	public AudioClip[] audioHyperDustPickupFail;
	public AudioClip[] audioHyperJumpReady;
	public AudioClip[] audioHyperJumpEnter;
	public AudioClip[] audioHyperJumpExit;
	public AudioClip[] audioHyperJumpFail;
	
	private Vector3 direction = Vector3.zero;
	public float maxThrust = 400f;
	public float thrust {get; private set;}
	private Vector3 targetPosition = Vector3.zero;
	private float startZ;
	private bool hyperJump = false;
	private int hyperMatter = 0; 
	private int newHyperMatter = 0;
	private int hyperJumpCount = 0;
	private ParticleSystem hyperSpaceEffect;
	
	public bool HyperReady {
		get { return hyperMatter == maxHyperMatter;}
	}
	
	public int HyperMultiplier {
		get { return hyperJumpCount; }
	}
	
	public int HyperTankPercentage {
		get { return Mathf.RoundToInt(100f*hyperMatter/maxHyperMatter); }
	}
	
	// Use this for initialization
	IEnumerator Start () 
	{
		targetPosition = transform.position;
		startZ = transform.position.z;
		hyperSpaceEffect = transform.FindChild("HyperSpaceEffect").particleSystem;
		controlEnabled = false;
		PlayRandomSound(audioGameStart, transform.position);
		StartCoroutine( HyperDustPickupQueue() );
		yield return new WaitForSeconds(0.3f);
		StartCoroutine( HyperJump() );
		controlEnabled = true;
		// 2D: keep the ship on the track
		
		
	}
	

	void Update () 
	{
		if (isDead || hyperJump || !controlEnabled)
			return;

		WeaponUpdate();
	}
	
	void FixedUpdate() 
	{
		if (isDead || hyperJump || !controlEnabled)
			return;
		MovementUpdate();
	}
	
	void MovementUpdate()
	{
		float accFactor = CubeMaster.Instance.cubeSpeed/CubeMaster.Instance.InitialCubeSpeed;
		
		// ship thrust
		if (Input.GetButton("ThrustUp")) 
		{
			thrust += acceleration * accFactor;
			thrust = Mathf.Min(thrust, maxThrust);
		}
		else if (Input.GetButton("ThrustDown"))
		{
			thrust -= acceleration * accFactor;
			thrust = Mathf.Max(thrust, -maxThrust);
		}
		else
		{
			thrust *= 0.75f;
		}
		
		// corrective Z (forward/back) 
		float error = startZ - transform.position.z;
		Vector3 correction = Vector3.forward * error;
		
		Vector3 force = Vector3.up * thrust * rigidbody.drag * rigidbody.mass;
		rigidbody.AddForce(Vector3.up * thrust + correction);
		
		// ship rotation
		Vector3 vel = rigidbody.velocity;
		vel += Vector3.forward * CubeMaster.Instance.cubeSpeed;
		Quaternion forward = Quaternion.LookRotation(vel);
		transform.rotation = Quaternion.Lerp(transform.rotation, forward, Time.deltaTime);
	}
	
	void MovementUpdate3D()
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
		
		// maxSpeed scales with cubespeed
		float speedChangeFactor = CubeMaster.Instance.cubeSpeed/CubeMaster.Instance.InitialCubeSpeed;
		targetPosition += direction * maxSpeed * speedChangeFactor * Time.deltaTime;
		
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 16f);
		transform.rotation = Quaternion.Euler(vrot);
		
	}
	
	void WeaponUpdate()
	{
		if (Input.GetButton("Fire1"))
		{
			BroadcastMessage("Fire");
		}
		
		if (Input.GetButtonDown("Fire2"))
		{
			if (!hyperJump && hyperMatter == maxHyperMatter)
			{
				StartCoroutine( HyperJump() );
			}
			else if (!hyperJump)
			{
				PlayRandomSound(audioHyperJumpFail, transform.position);
			}
		}
	}
	
	IEnumerator HyperJump()
	{
		hyperJump = true;
		collider.enabled = false;
		bool firstJump = hyperJumpCount == 0;
		if (firstJump) Time.timeScale = 3f;
		hyperJumpCount++;
		hyperMatter = 0;
		maxHyperMatter += hyperJumpCount;
		PlayRandomSound(audioHyperJumpEnter, transform.position);
		LevelGenerator.Reset(transform.position);
		LevelGenerator.LockPosition();
		LevelGenerator.Obstacles = false;
		CubeMaster.Instance.HyperJump = true;
		CubeMaster.Instance.SpeedChange(hyperJumpSpeedChange);
		ScreenShake.Instance.Shake(0.5f,2f/CubeMaster.Instance.CubeTravelTime);
		hyperSpaceEffect.time = 0f;
		hyperSpaceEffect.playbackSpeed = hyperSpaceEffect.duration/CubeMaster.Instance.CubeTravelTime;
		hyperSpaceEffect.Play();
		transform.rotation = Quaternion.identity;
		yield return new WaitForSeconds(CubeMaster.Instance.CubeTravelTime * 1.5f);
		ScoreBoard.CurrentScore += Mathf.RoundToInt(100000f/CubeMaster.Instance.CubeTravelTime);
		CubeMaster.Instance.HyperJump = false;
		PlayRandomSound(audioHyperJumpExit, transform.position);
		ScreenShake.Instance.Shake(0.5f,0.5f);
		if (firstJump) Time.timeScale = 1f;
		yield return new WaitForSeconds(0.5f);
		LevelGenerator.Unlock();
		LevelGenerator.Obstacles = true;
		collider.enabled = true;
		direction = Vector3.zero;
		hyperJump = false;
	}	
	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Death" && !isDead)
		{
			if (!isDead) StartCoroutine( Death() );
		}
		ScreenShake.Instance.Shake(0.75f,3f);
		Vector3 force = col.relativeVelocity * 2f;
		Vector3 point = col.contacts[0].point;
		Instantiate(deathsplosion, point, transform.rotation);
		rigidbody.AddForceAtPosition(force, point, ForceMode.Impulse);
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
			else 
			{
				PlayRandomSound(audioHyperDustPickupFail, col.transform.position);
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
		GameManager.Instance.GameOver();
	}
	
	IEnumerator HyperDustPickupQueue()
	{
		while (true)
		{
			if ( newHyperMatter > 0)
			{
				newHyperMatter--;
				hyperMatter++;
				ScoreBoard.CurrentScore += 3000 * hyperJumpCount;
				audio.pitch = 0.5f + (float)hyperMatter/(float)maxHyperMatter;
				audio.Play();
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
