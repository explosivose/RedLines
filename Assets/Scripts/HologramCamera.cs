using UnityEngine;
using System.Collections;

public class HologramCamera : MonoBehaviour {

	public Shader hologramShader;
	private Transform player;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		camera.enabled = false;
		camera.RenderWithShader(hologramShader, "");
		camera.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPos = player.position;
		playerPos.z -= camera.farClipPlane/2f;
		transform.position = playerPos;
		transform.rotation = Quaternion.identity;
	}
}
