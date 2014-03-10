using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

	public float maxSpeed     = 10f;
	public float currentSpeed = 0f;
	public Transform deathsplosion;
	
	public static bool isDead = false;
	
	private Vector3 targetPosition = Vector3.zero;
	
	// Use this for initialization
	void Start () 
	{
		targetPosition = transform.position;
	}
	
	void Update () 
	{
		float V = Input.GetAxisRaw("Vertical");
		float H = Input.GetAxisRaw("Horizontal");
		if (Input.GetButton("Fire1") ) V *= 1.5f;
		Vector3 direction = new Vector3(0f, V, H).normalized;
		
		targetPosition += direction * maxSpeed * Time.deltaTime;
		
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2f);
		
		Vector3 vector3Rotation = new Vector3 (0f, H * 45f, -V * 45f);
		Quaternion rotation = Quaternion.Euler(vector3Rotation);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
		
		
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
