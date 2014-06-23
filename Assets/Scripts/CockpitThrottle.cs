using UnityEngine;
using System.Collections;

public class CockpitThrottle : MonoBehaviour {
	
	public Transform fullThrottle;
	
	private Vector3 zeroThrottlePosition;
	
	// Use this for initialization
	void Start () {
		zeroThrottlePosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target = zeroThrottlePosition;
		if (CubeMaster.Instance.HyperJumpEnter) {
			target = fullThrottle.localPosition;
		}
		
		float s = 1.70158f * 1.525f;
		//hyperjumpentertime
		float z = Mathf.Lerp (transform.localPosition.z, target.z, Time.deltaTime * 10f);
		if (CubeMaster.Instance.HyperJump) {
			z += Random.value * 0.03f;
		}
		target.z = z;
		transform.localPosition = target;
	}
}
