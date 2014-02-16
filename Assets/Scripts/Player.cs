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
	
	void Update () 
	{
		float V = Input.GetAxisRaw("Vertical");
		if (Input.GetButton("Fire1") ) V *= 1.5f;
		Vector3 direction = new Vector3(0f, V);
		
		transform.position += direction * maxSpeed * Time.deltaTime;
		
		Vector3 vector3Rotation = new Vector3 (0f, 0f, -V * 45f);
		Quaternion rotation = Quaternion.Euler(vector3Rotation);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 8f);
		
		
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
