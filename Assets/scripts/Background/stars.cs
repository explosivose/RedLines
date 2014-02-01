using UnityEngine;
using System.Collections;

public class stars : MonoBehaviour {

	private Player player;
	private ParticleSystem starSystem;
	private ParticleSystemRenderer starRenderer;

	public float particleExpSpeedFactor;
	public float particleExpScaleFactor;
	public float particleScaleShift;

	// Use this for initialization
	void Start () {
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
			starSystem = (ParticleSystem)gameObject.GetComponent(typeof(ParticleSystem));
			starRenderer = (ParticleSystemRenderer)starSystem.renderer;
		}
		else 
		{
			player = null;
			Debug.LogError("Background Error: Stars: Player game object not found!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null)
		{
			int speedValue = (int)Mathf.Round(player.currentSpeed);

			// Particles Speed
			starSystem.particleSystem.startSpeed = Mathf.Clamp(Mathf.Exp(speedValue/player.thrust),0f, 14f);

			// Particles Scale
			float x = (Mathf.Exp(speedValue/particleExpScaleFactor)-particleScaleShift);
			x = Mathf.Clamp(x, 0f, 1f);
			starRenderer.velocityScale = x;
			
			// Particles colour
			starRenderer.particleSystem.startColor = GameManager.Instance.ColourSecondary;
		}
	}
}
