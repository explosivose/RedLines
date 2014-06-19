using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float hyperfactor = 1f;
		if (CubeMaster.Instance.HyperJump) hyperfactor = 10f;
		transform.position += Vector3.back * CubeMaster.Instance.cubeSpeed * hyperfactor * Time.deltaTime;
		transform.Rotate(Vector3.one * hyperfactor * 0.1f);
	}
	

}
