using UnityEngine;
using System.Collections;

public class CockpitHyperMeter : MonoBehaviour {

	public Transform fullMeter;
	
	private Vector3 emptyMeterPosition;
	private float t;
	// Use this for initialization
	void Start () {
		emptyMeterPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target = fullMeter.localPosition;
		
		float p = Player.Instance.HyperTankPercentage / 100f;
		
		t = Berp (t, p, Time.deltaTime * (p+0.1f));
		
		float y = Mathf.Lerp (emptyMeterPosition.y, fullMeter.localPosition.y, t);
		
		target = transform.localPosition;
		target.y = y;
		transform.localPosition = target;
	}
	
	
	float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}
}
