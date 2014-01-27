using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public float thrust = 15f;
	public Transform deathExplosion;

	public AudioClip hyperJump;

	public float maxSpeed
	{
		get { return thrust / (rigidbody.mass * rigidbody.drag); }
	}
	public float currentSpeed
	{
		get { return rigidbody.velocity.magnitude; }
	}
	public float currentAcceleration
	{
		get { return movement.magnitude; }
	}
	
	private float verticalThrust = 100f;
	private Vector2 movement;
	private bool isDead = false;
	
	private Transform light_back;
	
	// Use this for initialization
	void Start () 
	{
		transform.position = LevelGenerator.tail;
		isDead = false;
		tag = "Player";
		
		light_back = transform.FindChild("light_back");
	}

	// Update is called once per frame
	void Update () 
	{

		// do nothing else if the game is over (player dead)
		if (isDead) 
		{
			return;
		}

		Movement();
		Aesthetics();
		
	}

	void Movement()
	{
		verticalThrust = 2f * thrust;
		// player controls
		float horizontal = 1f;//Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		movement.x = horizontal * thrust;
		movement.y = vertical * verticalThrust;
		
		
		// ship rotation
		float dampedV = Input.GetAxis ("Vertical");
		Vector3 rotation = Vector3.zero;
		
		rotation = new Vector3(dampedV * 20f,0f,dampedV * 20f);
		
		Quaternion newRot = Quaternion.Euler(rotation) * Quaternion.LookRotation(rigidbody.velocity);
		transform.rotation = Quaternion.Lerp (transform.rotation, newRot, Time.deltaTime * 16f);
		
		
	}

	private float prevSpeed = 0f;

	void Aesthetics()
	{		
		// player audio pitch is dependent on speed
		audio.pitch = currentSpeed / (maxSpeed/12f);
		
		float deltaSpeed = (currentSpeed - prevSpeed)/Time.fixedDeltaTime;
		prevSpeed = currentSpeed;
		if (deltaSpeed < 0f) deltaSpeed = 0f;
		audio.volume = deltaSpeed * 0.02f;
		
		light_back.light.color = GameManager.Instance.ColourPrimary;
		light_back.light.intensity = 8 * (currentSpeed/maxSpeed);
	}

	void FixedUpdate()
	{
		rigidbody.AddForce(movement);
	}

	void OnCollisionEnter(Collision info)
	{
		if (info.relativeVelocity.magnitude > 10f && !isDead)
			PlayerDeath();
	}
	
	void OnTriggerEnter(Collider info)
	{
		if(info.tag == "HyperMatter")
		{
			GameManager.Instance.HyperSpaceIncrement();
			AudioSource.PlayClipAtPoint(hyperJump, transform.position);
		}
	}

	void PlayerDeath()
	{
		isDead = true;
		GameManager.Instance.State = GameManager.GameState.GameOver;
		GameManager.Instance.StartDialogue("Commander:", "Well, shit.",1.5f);
		audio.Stop();
		
		Instantiate (deathExplosion, transform.position + Vector3.back, Quaternion.LookRotation (Vector3.back));

		rigidbody.drag = 2f;

		Debug.Log ("You're brown bread!");
		
		ScoreManager.Instance.NewScore(transform.position.x);
	}
}
