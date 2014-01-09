using UnityEngine;
using System.Collections;


public class ProximitySound : MonoBehaviour {

	public float proximityDistance = 1.5f;



	void Start()
	{

	}

	void Update()
	{
		float nearest = NearestObject ();
		if (audio.isPlaying) 
		{
			if ( nearest >= proximityDistance ) 
			{
				audio.Pause();
			}
			else
			{
				// Get LOUDER if you're closer
				if ( proximityDistance == 0f)
					audio.volume = 1f;
				else
					audio.volume = (proximityDistance - nearest)/proximityDistance;
			}

		} 
		else 
		{
			if ( nearest < proximityDistance ) 
				audio.Play();
		}

	}

	float NearestObject()
	{
		float smallestDistance = proximityDistance;
		float distance = proximityDistance;
		
		Vector2 origin;
		origin.x = transform.position.x;
		origin.y = transform.position.y;
		
		RaycastHit2D hit;
		//look up
		hit =  Physics2D.Raycast (origin, Vector2.up, proximityDistance);
		if (hit != null) {
			distance = Vector2.Distance(origin, hit.point);
			if (distance < smallestDistance) smallestDistance = distance;
		}
		//look down
		hit =  Physics2D.Raycast (origin, -Vector2.up, proximityDistance);
		if (hit != null) {
			distance = Vector2.Distance(origin, hit.point);
			if (distance < smallestDistance) smallestDistance = distance;
		}
		return smallestDistance;
	}

	
}
