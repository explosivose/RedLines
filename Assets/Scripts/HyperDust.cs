using UnityEngine;
using System.Collections;

public class HyperDust : MonoBehaviour {

	Vector3 initialScale;
	
	// Use this for initialization
	void Start () {
		initialScale = transform.localScale;
		Destroy(this.gameObject, 30f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += Vector3.back * CubeMaster.Instance.cubeSpeed * Time.deltaTime;
		Vector3 scale = initialScale * (Mathf.Sin (Time.time*4f)/4f + 1f);
		transform.localScale = scale;
		transform.Rotate(Vector3.one);
	}
}
