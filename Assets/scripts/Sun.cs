using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {

	public Color slowColor = Color.blue;
	public Color fastColor = Color.red;

	private Player player;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}

	void Update () 
	{
		light.color = Color.Lerp (slowColor, fastColor, player.currentSpeed / player.maxSpeed);
		Color background = light.color;
		background.r = (1f - background.r)/4f;
		background.g = (1f - background.g)/4f;
		background.b = (1f - background.b)/4f;
		Camera.main.backgroundColor =background;
	}
}
