using UnityEngine;
using System.Collections;

public class ShipFlame : MonoBehaviour {


	private ParticleSystem particleSys;
	private Player player;

	// Use this for initialization
	void Start () {

		particleSys = GetComponent<ParticleSystem>().particleSystem;

		GameObject playerObj = GameObject.Find("Player3D");
		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
		}
		else 
		{
			player = null;
			Debug.Log("Particles Error: Flame particle: Player game object not found!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null){
			particleSys.startLifetime = 0.1f+player.currentSpeed/140f + player.currentAcceleration/40f;
			particleSys.gravityModifier = Input.GetAxisRaw ("Vertical")/16;
		}

	}
}
