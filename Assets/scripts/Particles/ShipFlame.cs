using UnityEngine;
using System.Collections;

public class ShipFlame : MonoBehaviour {


	private ParticleSystem particleSys;
	private Player player;

	public float speedFactor = 140f;
	public float accelerationFactor = 40f;

	// Use this for initialization
	void Start () {

		particleSys = GetComponent<ParticleSystem>().particleSystem;

		GameObject playerObj = GameObject.FindWithTag("Player");

		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
		}
		else 
		{
			player = null;
			Debug.LogError("Particles Error: Flame particle: Player game object not found!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null){
			particleSys.startLifetime = 0.1f+player.currentSpeed/speedFactor + player.currentAcceleration/accelerationFactor;
			particleSys.gravityModifier = Input.GetAxisRaw ("Vertical")/16;
		}
		particleSys.startColor = GameManager.Instance.ColourPrimary;

	}
}
