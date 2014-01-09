using UnityEngine;
using System.Collections;

public class Nuke : MonoBehaviour {

	public float armTime = 10f;
	public Transform explosion;
	float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		StartCoroutine ("Countdown");

	}
	
	// Update is called once per frame
	void Update () {
		int timeRemaining = Mathf.RoundToInt((startTime + armTime) - Time.time);
		renderer.material.color = Color.Lerp (Color.white, Color.black, timeRemaining / armTime);
	}

	IEnumerator Countdown()
	{
		yield return new WaitForSeconds(armTime);
		Explode ();
	}

	void Explode()
	{
		Instantiate (explosion, transform.position, transform.rotation);
		Destroy (gameObject);
	}
}
