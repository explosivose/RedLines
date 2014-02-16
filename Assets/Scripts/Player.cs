using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

	public float maxSpeed     = 10f;
	public float currentSpeed = 0f;
	public Transform deathsplosion;
	
	public static bool isDead = false;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	void FixedUpdate () 
	{
		float thrust = maxSpeed * rigidbody.mass * rigidbody.drag;
		float V = Input.GetAxisRaw("Vertical");
		
		Vector3 direction = new Vector3(0f, V).normalized;
		
		rigidbody.AddForce(direction * thrust);
		
		Vector3 vector3Rotation = new Vector3 (0f, 0f, -V * 45f);
		Quaternion rotation = Quaternion.Euler(vector3Rotation);
		//rotation *= Quaternion.LookRotation(rigidbody.velocity);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
		
		
		currentSpeed = rigidbody.velocity.magnitude;
	}
	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Death" && !isDead)
		{
			Death();
		}
	}
	
	void Death()
	{
		Instantiate(deathsplosion, transform.position, transform.rotation);
		maxSpeed = 0f;
		//rigidbody.useGravity = true;
		isDead = true;
	}
}
