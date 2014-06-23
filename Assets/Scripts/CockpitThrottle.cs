using UnityEngine;
using System.Collections;

public class CockpitThrottle : MonoBehaviour {
	
	public Transform fullThrottle;
	
	private Vector3 zeroThrottlePosition;
	private Quaternion zeroThrottleRotation;
	
	// Use this for initialization
	void Start () {
		zeroThrottlePosition = transform.localPosition;
		zeroThrottleRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target = zeroThrottlePosition;
		if (CubeMaster.Instance.HyperJumpEnter) {
			target = fullThrottle.localPosition;
		}
		
		float z = Berp(transform.localPosition.z, target.z, Time.deltaTime * 0.5f);
		if (CubeMaster.Instance.HyperJump) {
			z += Random.value * 0.03f;
			//a += Random.value * 1f;
		}
		
		target.z = z;
		transform.localPosition = target;
		
		
		float t = Mathf.InverseLerp(zeroThrottlePosition.z, fullThrottle.localPosition.z, transform.localPosition.z);
		transform.localRotation = Quaternion.Lerp(zeroThrottleRotation, fullThrottle.localRotation, t);
		
		
	}
	
	
	float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}
}
