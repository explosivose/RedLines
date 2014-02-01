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
			if (player.currentSpeed > player.maxSpeed)
			{
				particleSys.startLifetime = (player.currentSpeed * 2)/player.maxSpeed;
				particleSys.startSize = (player.currentSpeed * 1.5f)/player.maxSpeed;
				particleSys.startSpeed = (player.currentSpeed * 2)/player.maxSpeed;
			}
			else
			{
				particleSys.startLifetime = player.currentSpeed/player.maxSpeed;
				particleSys.startSize = player.currentSpeed/player.maxSpeed;
				particleSys.startSpeed = player.currentSpeed/player.maxSpeed;
			}
		}
		particleSys.gravityModifier = Input.GetAxisRaw ("Vertical")/8;
		particleSys.startColor = GameManager.Instance.ColourPrimary;

	}
}
