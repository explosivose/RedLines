using UnityEngine;
using System.Collections;

public class stars : MonoBehaviour {

	private Player player;
	private ParticleSystem starSystem;
	public float speedFactor;

	// Use this for initialization
	void Start () {
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
			starSystem = (ParticleSystem)gameObject.GetComponent(typeof(ParticleSystem));
		}
		else 
		{
			player = null;
			Debug.LogError("Background Error: Stars: Player game object not found!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null){

			//ParticleSystemRenderer pr = (ParticleSystemRenderer)someGameObject.particleSystem.renderer;
			//pr.renderMode = ParticleSystemRenderMode.VerticalBillboard;
			//pr.material.Color = Color.Blue;

			int speedValue = (int)Mathf.Round(player.currentSpeed);
			Debug.Log((Mathf.Exp(speedValue/15f)));

			starSystem.particleSystem.startSpeed = (Mathf.Exp(speedValue/15f));

			ParticleSystemRenderer pr = (ParticleSystemRenderer)starSystem.renderer;

			float x = (Mathf.Exp(speedValue/100f)-1.4f);
			x = Mathf.Clamp(x, 0f, 5f);

			pr.velocityScale = x;
		}
	}
}
