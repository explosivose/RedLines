using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public enum ControlMode {
		mouseFromCenter,
		mouseRotation
	}
	
	public ControlMode controlMode = ControlMode.mouseFromCenter;
	public float maxSpeed     = 10f;
	public float turnSpeed = 100f;
	public Transform deathsplosion;
	public static bool isDead = false;
	
	private Vector3 direction = Vector3.zero;
	private Vector3 targetPosition = Vector3.zero;
	private Vector3 screenCenter = Vector3.zero;

	
	
	// Use this for initialization
	void Start () 
	{
		targetPosition = transform.position;
	}
	
	void Update () 
	{
		if (isDead) {
			Screen.lockCursor = false;
			return;
		}
		switch (controlMode)
		{
		case ControlMode.mouseFromCenter:
			mouseFromCenterUpdate();
			break;
		case ControlMode.mouseRotation:
			mouseRotationUpdate();
			break;
		default:
			break;
		}
		
		MouseCursor();
	}
	
	void OnGUI()
	{
		screenCenter = new Vector3(Screen.width/2, Screen.height/2);
		switch (controlMode)
		{
		case ControlMode.mouseFromCenter:
			Rect pos = new Rect(screenCenter.x + direction.x, screenCenter.y - direction.y, 100f, 30f);
			GUI.Label(pos, direction.magnitude.ToString());
			break;
		}
		
	}
	
	void MouseCursor()
	{
		switch(controlMode)
		{
		case ControlMode.mouseFromCenter:
			Screen.lockCursor = false;
			break;
		case ControlMode.mouseRotation:
			if (Screen.lockCursor == false)
			{
				Time.timeScale = 0f;
				if (Input.anyKey)
					Screen.lockCursor = true;
			}
			else
			{
				Time.timeScale = 1f;
			}
			break;
			
		}
	}
	
	
	void mouseFromCenterUpdate()
	{
		Vector3 mpos = Input.mousePosition;
		direction = (mpos - screenCenter);
		if (direction.magnitude < 10) direction = Vector3.zero;
		Vector3 dirNormalized = direction / 100f;
		
		targetPosition += dirNormalized * maxSpeed * Time.deltaTime;
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2f);
		
		Vector3 vrotation = new Vector3(dirNormalized.x * 45, 90f, -dirNormalized.y * 45);
		Quaternion rotation = Quaternion.Euler(vrotation);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
	}
	
	void mouseRotationUpdate()
	{
		// decide which input to take
		float mx = Input.GetAxis ("Mouse X");
		float ha = Input.GetAxis("Horizontal");
		float h = ha;
		if (Mathf.Abs(mx) > Mathf.Abs(ha)) h = mx;
		float my = -Input.GetAxis ("Mouse Y");
		float va = -Input.GetAxis("Vertical");
		float v = va;
		if (Mathf.Abs(my) > Mathf.Abs(va)) v = my;
		
		float maxrot = 45f;
		// calculate rotation based on input
		Vector3 vrot = transform.rotation.eulerAngles;
		vrot.y += h * turnSpeed * Time.deltaTime;
		if (vrot.y >  90f+maxrot) vrot.y =  90f+maxrot;
		if (vrot.y <  90f-maxrot) vrot.y =  90f-maxrot;
		vrot.z += v * turnSpeed * Time.deltaTime;
		if (vrot.z > maxrot      && vrot.z <  90f) vrot.z = maxrot;
		if (vrot.z < 360f-maxrot && vrot.z > 270f) vrot.z = 360f-maxrot;
		
		// lag rotation
		Quaternion rotation = Quaternion.Euler(vrot);//Quaternion.Lerp(transform.rotation, Quaternion.Euler(vrot), Time.deltaTime * 16f);
		transform.rotation = Quaternion.Euler(vrot);
		// reassign rotation vector after lag
		vrot = rotation.eulerAngles;
		
		// need to avoid the 359, 0 border..
		if (vrot.z > 270f) vrot.z -= 360f;
		
		// calculate movement based on rotation
		float x = Rescale(vrot.y, 90f+maxrot, 90f-maxrot, 1f, -1f);
		float y = Rescale(vrot.z, maxrot, -maxrot, 1f, -1f);
		direction = new Vector3(x, -y);
		targetPosition += direction * maxSpeed * Time.deltaTime;
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 16f);
		
	}
	

	

	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Death" && !isDead)
		{
			Death();
		}
	}
	
	void Death()
	{
		Instantiate(deathsplosion, transform.position, transform.rotation);
		maxSpeed = 0f;
		//rigidbody.useGravity = true;
		isDead = true;
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
}
