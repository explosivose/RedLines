using UnityEngine;
using System.Collections;

public class HyperDust : MonoBehaviour {

	public float suckSpeed = 10f;

	private Vector3 initialScale;
	private bool 	collected;
	
	void Collect() {
		collected = true;
	}
	
	// Use this for initialization
	void Start () {
		initialScale = transform.localScale;
		Destroy(this.gameObject, 30f);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 scale = initialScale * (Mathf.Sin (Time.time*4f)/4f + 1f);
		transform.localScale = scale;
		transform.Rotate(Vector3.one);
		AttractedToPlayer();
		/*
		if (collected) {
			AttractedToPlayer();
		}
		else {
			transform.position += Vector3.back * CubeMaster.Instance.cubeSpeed * Time.deltaTime;
		}
		*/
	}
	
	void AttractedToPlayer() {
		Vector3 moveBack = Vector3.back * CubeMaster.Instance.cubeSpeed;
		Vector3 playerPos = Player.Instance.transform.position;
		Vector3 playerDir = (playerPos - transform.position).normalized;
		float playerDist = Vector3.Distance(playerPos, transform.position);
		Vector3 move = moveBack + playerDir * suckSpeed/(playerDist * playerDist);
		transform.position += move * Time.deltaTime;
		if (playerDist < 0.5f) {
			Player.Instance.AddHyperDust();
			Destroy(this.gameObject);
		}
	}
}
