using UnityEngine;
using System.Collections;

public class Hologram : MonoBehaviour {
	
	private Transform player;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 rotation = player.rotation.eulerAngles;
		rotation.x += 270f;
		transform.localRotation = Quaternion.Euler(rotation);
	}
}
