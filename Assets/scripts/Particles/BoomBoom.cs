using UnityEngine;
using System.Collections;

public class BoomBoom : MonoBehaviour {

	public Light halo;
	private float toggle = 1f;

	// Use this for initialization
	void Start () {
		halo = (Light)gameObject.GetComponent(typeof(Light));
		halo.range = 0.2f;
		halo.intensity = 0.1f;
		StartCoroutine("explosion");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

	IEnumerator explosion(){
		for(float f = 0f; f <= 1.4f; f += 0.2f)
		{
			if(toggle > 0f)
				halo.intensity = f;
			else
				halo.intensity = 0f;

			halo.range += 2f;
			toggle *= -1f;
			Debug.Log(halo.intensity);
			yield return new WaitForSeconds(0.1f);
		}

		for(float f = 0f; f <= 2f; f += 0.2f)
		{
			halo.range += 0.4f;
			halo.intensity -= 0.1f;
			yield return new WaitForSeconds(0.08f);
		}
		halo.intensity = 0f;

	}
}
