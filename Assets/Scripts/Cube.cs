using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {
	
	public Vector3 targetPosition = Vector3.zero;
	public float audioBeat = 0f;
	
	private Vector3 motionSmoothness = Vector3.zero;
	private CubeMaster master; 

	void Start () {
		master = GameObject.FindWithTag("CubeMaster").GetComponent<CubeMaster>();
		StartCoroutine("KillTrail");
	}

	IEnumerator KillTrail() {
		yield return new WaitForSeconds(1f);
		TrailRenderer trail = this.GetComponent<TrailRenderer>();
		trail.enabled = false;
	}


	void Update () {
		// Target position is not the actual final position. 
		// Final position is affected by position shifts introduced by special effects.
		// Add forward motion along x-axis to move level
		targetPosition += new Vector3(master.cubeRatePerSecond * master.cubeSpeed * Time.deltaTime, 0, 0);

		// Initilize shifts with target position as a starting value
		float xShift = targetPosition.x;
		float yShift = Mathf.Lerp(transform.position.y, targetPosition.y, Time.deltaTime * motionSmoothness.y);
		float zShift = Mathf.Lerp(transform.position.z, targetPosition.z, Time.deltaTime * motionSmoothness.z);

		// Audio effects
		int shakeMe = Random.Range(0,100);
		
		if(shakeMe > 40 && audioBeat > 0.2f && audioBeat <= 0.6f)
		{
			zShift += master.audioBeat*0.5f*audioBeat;
		}
		
		if(shakeMe > 40 && audioBeat > 0.6f )
		{
			zShift += master.audioBeat*1f*audioBeat;
		}

		yShift += master.audioBeat*.2f;

		// Move cube
		//zShift = Mathf.Clamp(zShift, targetPosition.z, targetPosition.z + 1);
		transform.position = new Vector3(xShift, yShift, zShift);

	}


	public void SetMotionSmooth(Vector3 value)
	{
		motionSmoothness = value;
	}

}
