using UnityEngine;
using System.Collections;
[RequireComponent(typeof(LineRenderer))]
public class WeaponLaser : MonoBehaviour 
{
	public bool debug = true;
	public int rendererPoints = 25;
	public float minimumRateOfFire = 0.2f;
	public Transform hitEffect;
	public AudioClip[] audioFireLaser;
	
	private float rateOfFire;
	private LineRenderer lr;
	private bool firing = false;
	private float fireTime = 0f;
	private Color startC;
	private Color endC;
	private Transform player;
	private GameObject[] lights;
	
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		rateOfFire = minimumRateOfFire;
		lr = GetComponent<LineRenderer>();
		lr.useWorldSpace = true;
		lights = new GameObject[rendererPoints];
		for (int i = 0; i < rendererPoints; i++)
		{
			lights[i] = new GameObject("light"+i, typeof(Light));
			lights[i].transform.parent = this.transform;
			lights[i].SetActive(false);
		}
	}
	
	void FixedUpdate()
	{
		if (debug && Input.GetButton("Fire1"))
			Fire();
		
		if (rateOfFire > minimumRateOfFire)
		{
			rateOfFire -= Time.deltaTime*0.1f;
		}
		
		if (!firing)
		{
			float lerp = Time.time - fireTime;
			if(lerp < rateOfFire)
			{
				startC = Color.Lerp(startC, Color.clear, lerp);
				endC = Color.Lerp (endC, Color.clear, lerp);
				lr.SetColors(startC, endC);
				for (int i = 0; i < rendererPoints; i++)
				{
					lights[i].light.color = Color.Lerp (startC, endC, lerp*i/rendererPoints);
				}
			}
			else
			{
				lr.SetVertexCount(2);
				lr.SetWidth(0.01f, 0f);
				lr.SetColors(Color.red, Color.red);
				lr.SetPosition(0, transform.position);
				RaycastHit hit;
				if (Physics.Raycast(transform.position, player.forward, out hit))
				{
					lr.SetPosition(1, hit.point);
					if (hit.transform.tag == "HyperMatter")
						lr.SetColors(Color.green, Color.red);
				}
				else 
				{
					lr.SetPosition(1, transform.position + player.forward * 50f);
				}				
				for (int i = 0; i < rendererPoints; i++)
				{
					lights[i].SetActive(false);
				}
			}	
		}
	}
	
	public void Fire()
	{
		if (!firing) 
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, player.forward, out hit))
			{
				StartCoroutine( FireRoutine(hit.transform.position) );
				if (hit.transform.tag == "HyperMatter")
					hit.transform.BroadcastMessage("Explode");
			}
			else
			{
				StartCoroutine( FireRoutine(transform.position + player.forward * 50f));
			}

		}
	}
	
	IEnumerator FireRoutine(Vector3 target)
	{
		firing = true;
		if (fireTime > Time.time - 2f)
		{
			rateOfFire += minimumRateOfFire;
		}
		int x = Random.Range(0, audioFireLaser.Length);
		AudioSource.PlayClipAtPoint(audioFireLaser[x], transform.position);
		fireTime = Time.time;
		lr.SetVertexCount(rendererPoints);
		lr.SetWidth(0.01f, 1f);
		startC = new Color(1f, Random.value, Random.value);
		endC = new Color(1f, Random.value, Random.value);
		lr.SetColors(startC, endC);
		lr.enabled = true;
		ScreenShake.Instance.Shake(0.2f, 1.5f);
		Instantiate(hitEffect, target, Random.rotation);
		float targetDistance = Vector3.Distance(transform.position, target);
		while (fireTime + 0.2f > Time.time)
		{
			for (int i = 0; i < rendererPoints; i++)
			{
				Vector3 point = Vector3.Lerp(transform.position, target, i/targetDistance);
				point += Random.insideUnitSphere * i/targetDistance;
				lr.SetPosition(i, point);
				lights[i].SetActive(true);
				lights[i].transform.position = point;
				lights[i].light.color = Color.Lerp(startC, endC, i/rendererPoints);
			}
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(rateOfFire);

		fireTime = Time.time;
		firing = false;
	}

}
