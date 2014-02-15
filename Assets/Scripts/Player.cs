using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

	public float maxSpeed     = 10f;
	public float currentSpeed = 0f;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	void FixedUpdate () 
	{
		float thrust = maxSpeed * rigidbody.mass * rigidbody.drag;
		
		Vector3 direction = new Vector3(0f, Input.GetAxisRaw("Vertical")).normalized;
		
		
		rigidbody.AddForce(direction * thrust);
		
		currentSpeed = rigidbody.velocity.magnitude;
	}
}
