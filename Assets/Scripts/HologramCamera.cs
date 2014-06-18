using UnityEngine;
using System.Collections;

public class HologramCamera : MonoBehaviour {

	public Transform ship;
	public enum perspective {
		behind, top, right
	}
	public perspective viewFrom = perspective.behind;
	
	private Vector3 offset;
	
	// Use this for initialization
	void Start () {
		switch (viewFrom) {
		case perspective.behind:
			camera.farClipPlane = 30f;
			offset = Vector3.back * camera.farClipPlane/2f;
			break;
		case perspective.right:
			camera.farClipPlane = 10f;
			offset = Vector3.right * camera.farClipPlane/2f;
			break;
		case perspective.top:
			camera.farClipPlane = 10f;
			offset = Vector3.up * camera.farClipPlane/2f;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = ship.position + offset;;
		transform.LookAt(ship);
	}
}
