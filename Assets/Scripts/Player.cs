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
	
	
	private Vector3 direction = Vector3.zero;
	private Vector3 targetPosition = Vector3.zero;
	private bool hyperJump = false;
	private int hyperMatter = 0;
	
	private TextMesh guiHyperMatter;
	private TextMesh guiSpeed;
	private Transform guiHyperSpaceHint;
	
	// Use this for initialization
	void Start () 
	{
		targetPosition = transform.position;
		guiHyperMatter = transform.FindChild("guiHyperValue").GetComponent<TextMesh>();
		guiSpeed =       transform.FindChild("guiSpeedValue").GetComponent<TextMesh>();
		guiHyperSpaceHint = transform.FindChild("guiHyperSpaceHint");
		PlayRandomSound(audioGameStart, transform.position);
	}
	
	void Update () 
	{
		GUIUpdate();
		if (isDead || hyperJump)
			return;
			
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
		
		float maxrot = 45f;
		Vector3 vrot = transform.rotation.eulerAngles;
		
		// left/right rotation
		vrot.y += h * turnSpeed * Time.deltaTime;
		if (vrot.y >  90f+maxrot) vrot.y =  90f+maxrot; // clamp between 90+max and 90-max
		if (vrot.y <  90f-maxrot) vrot.y =  90f-maxrot;
		
		// up/down rotation
		vrot.z += v * turnSpeed * Time.deltaTime;
		if (vrot.z > maxrot      && vrot.z <  90f) vrot.z = maxrot;
		if (vrot.z < 360f-maxrot && vrot.z > 270f) vrot.z = 360f-maxrot;
		
		// calculate movement based on rotation
		
		// need to avoid the wrap around from 0deg to 359deg for movement
		// i.e. 359deg becomes -1deg
		if (vrot.z > 270f) vrot.z -= 360f;
		
		float x = Rescale(vrot.y, 90f+maxrot, 90f-maxrot, 1f, -1f);
		float y = Rescale(vrot.z, maxrot, -maxrot, 1f, -1f);
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
		if (hyperJump)
		{
			guiSpeed.text = "----";
			guiHyperMatter.text = (100 * hyperMatter / maxHyperMatter).ToString() + "%";
		}
		else
		{
			guiSpeed.text = (Mathf.RoundToInt(100*CubeMaster.Instance.cubeSpeed)).ToString();
			guiHyperMatter.text = (100 * hyperMatter / maxHyperMatter).ToString() + "%";
		}
		
		guiHyperSpaceHint.renderer.enabled = (hyperMatter == maxHyperMatter);
	}
	
	IEnumerator HyperJump()
	{
		hyperJump = true;
		PlayRandomSound(audioHyperJumpEnter, transform.position);
		hyperMatter = 0;
		CubeMaster.Instance.HyperJump = true;
		LevelGenerator.Reset(transform.position);
		yield return new WaitForSeconds(CubeMaster.Instance.CubeTravelTime);
		CubeMaster.Instance.HyperJump = false;
		CubeMaster.Instance.Decel();
		PlayRandomSound(audioHyperJumpExit, transform.position);
		ScreenShake.Instance.Shake(0.2f, 0.5f);
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
			if (hyperMatter < maxHyperMatter) 
			{
				hyperMatter++;
				PlayRandomSound(audioHyperDustPickup, transform.position);
				if (hyperMatter == maxHyperMatter)
					PlayRandomSound(audioHyperJumpReady, transform.position);
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
		GameManager.Instance.GameOver(0);
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
