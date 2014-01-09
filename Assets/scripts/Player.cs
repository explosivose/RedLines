using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public float thrust = 15f;
	public float maxSpeed
	{
		get 
		{
			return thrust / (rigidbody2D.mass * rigidbody2D.drag);
		}
	}
	public float currentSpeed
	{
		get
		{
			return rigidbody2D.velocity.magnitude;
		}
	}
	public float currentAcceleration
	{
		get 
		{
			return movement.magnitude;
		}
	}

	public Transform deathExplosion;
	public Transform nuke;

	private Vector2 movement;

	private bool nukeUsed = false;
	private bool isDead = false;
	// Use this for initialization
	void Start () 
	{
		isDead = false;
		tag = "Player";
	}

	// Update is called once per frame
	void Update () 
	{

		// do nothing else if the game is over (player dead)
		if (isDead) 
		{
			return;
		}

		Movement ();
		Aesthetics ();
		
	}

	void Movement()
	{
		// player controls
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		movement.x = horizontal * thrust;
		movement.y = vertical * thrust * 2f;
		
		
		if (!nukeUsed) 
		{
			if (Input.GetKey(KeyCode.Space))
			{
				nukeUsed = true;
				Instantiate (nuke, transform.position + Vector3.back, transform.rotation);
				GameManager.Instance.StartDialogue("Commander:", "TURN AROUND AND GET OUTTA THERE! NUKE AWAY!",10f);
			}
		}

		// player rotation is dependent on velocity
		float dampedV = Input.GetAxis ("Vertical");
		Vector3 rotation = Vector3.zero;
		
		if (rigidbody2D.velocity.x < 0f)
			rotation = new Vector3(dampedV * 10f,0f,180f - dampedV * 10f);
		else
			rotation = new Vector3(dampedV * 10f,0f,dampedV * 10f);
			
		transform.rotation = Quaternion.Euler(rotation);
	}

	void Aesthetics()
	{		
		// player audio pitch is dependent on speed
		audio.pitch = currentSpeed / (maxSpeed/12f);

	}

	void FixedUpdate()
	{
		rigidbody2D.AddForce(movement);
	}

	void OnCollisionEnter2D(Collision2D info)
	{
		if (info.relativeVelocity.magnitude > 10f && !isDead)
						PlayerDeath();
	}

	void PlayerDeath()
	{
		isDead = true;
		GameManager.Instance.State = GameManager.GameState.GameOver;
		GameManager.Instance.StartDialogue("Commander:", "Well, shit.",1.5f);
		audio.Stop();

		Instantiate (deathExplosion, transform.position + Vector3.back, Quaternion.LookRotation (Vector3.back));

		rigidbody2D.fixedAngle = false;
		rigidbody2D.drag = 2f;

		Debug.Log ("You're brown bread!");

	}
	
	public void LevelUp(float thrustIncrease, Gradient flames)
	{
		thrust += thrustIncrease;
		rigidbody2D.AddForce(movement * 2f);
	}
}
