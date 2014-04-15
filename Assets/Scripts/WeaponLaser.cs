using UnityEngine;
using System.Collections;
[RequireComponent(typeof(LineRenderer))]
public class WeaponLaser : MonoBehaviour 
{
	public bool debug = true;
	public int rendererPoints = 25;
	public float rateOfFire = 0.2f;
	
	public AudioClip[] audioFireLaser;
	
	private LineRenderer lr;
	private bool firing = false;
	private float fireTime = 0f;
	private Color startC;
	private Color endC;
	
	void Start()
	{
		lr = GetComponent<LineRenderer>();
		lr.useWorldSpace = true;
	}
	
	void Update()
	{
		if (debug && Input.GetButton("Fire1"))
			Fire();
		
		if (!firing)
		{
			float lerp = Time.time - fireTime;
			if(lerp < rateOfFire)
			{
				startC = Color.Lerp(startC, Color.clear, lerp);
				endC = Color.Lerp (endC, Color.clear, lerp);
				lr.SetColors(startC, endC);
			}
			else
			{
				lr.SetVertexCount(2);
				lr.SetWidth(0.01f, 0f);
				lr.SetColors(Color.red, Color.red);
				lr.SetPosition(0, transform.position);
				RaycastHit hit;
				if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit))
				{
					lr.SetPosition(1, hit.point);
					if (hit.transform.tag == "HyperMatter")
						lr.SetColors(Color.green, Color.red);
				}
				else 
				{
					lr.SetPosition(1, transform.position + Camera.main.transform.forward * 100f);
				}
			}	
		}
	}
	
	public void Fire()
	{
		if (!firing) 
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit))
			{
				if (hit.transform.tag == "HyperMatter")
					hit.transform.BroadcastMessage("Explode");
				
				lr.SetVertexCount(rendererPoints);
				lr.SetWidth(0.01f, 1f);
				startC = new Color(1f, Random.value, Random.value);
				endC = new Color(1f, Random.value, Random.value);
				lr.SetColors(startC, endC);
				lr.enabled = true;
				StartCoroutine( FireRoutine(hit.point) );
			}
		}
	}
	
	IEnumerator FireRoutine(Vector3 target)
	{
		firing = true;
		int x = Random.Range(0, audioFireLaser.Length);
		AudioSource.PlayClipAtPoint(audioFireLaser[x], transform.position);
		fireTime = Time.time;
		float targetDistance = Vector3.Distance(transform.position, target);
		while (fireTime + 0.2f > Time.time)
		{
			for (int i = 0; i < rendererPoints; i++)
			{
				Vector3 point = Vector3.Lerp(transform.position, target, i/targetDistance);
				point += Random.insideUnitSphere * i/targetDistance;
				lr.SetPosition(i, point);
			}
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(rateOfFire);
		fireTime = Time.time;
		firing = false;
	}

}
