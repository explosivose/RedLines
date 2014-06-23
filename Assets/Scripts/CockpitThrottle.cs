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
		float z = Berp(transform.localPosition.z, target.z, Time.deltaTime * 0.5f);
		if (CubeMaster.Instance.HyperJump) {
			z += Random.value * 0.03f;
		}
		target.z = z;
		transform.localPosition = target;
	}
	
	
	float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}
}
