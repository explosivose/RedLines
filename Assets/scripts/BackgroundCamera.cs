using UnityEngine;
using System.Collections;

public class BackgroundCamera : MonoBehaviour {
	
	public float brightness = 0.1f;
	
	// Update is called once per frame
	void Update () {
		Color background = GameManager.Instance.ColourPrimary;
		background.r *= brightness;
		background.g *= brightness;
		background.b *= brightness;
		camera.backgroundColor = background;
	}
}
