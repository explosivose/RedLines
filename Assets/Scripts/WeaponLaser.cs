using UnityEngine;
using System.Collections;
[RequireComponent(typeof(LineRenderer))]
public class WeaponLaser : MonoBehaviour 
{
	public bool debug = true;
	public int rendererPoints = 25;
	
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
		if (debug)
		{
			if (Input.GetButton("Fire1"))
			{
				Vector3 target = Camera.main.transform.forward * 100f;
				Fire (target);
			}
		}
		if (!firing)
		{
			startC = Color.Lerp(startC, Color.clear, Time.time - fireTime);
			endC = Color.Lerp (endC, Color.clear, Time.time - fireTime);
			lr.SetColors(startC, endC);
		}
	}
	
	public void Fire(Vector3 target)
	{
		if (!firing) 
		{
			lr.SetVertexCount(rendererPoints);
			startC = new Color(1f, Random.value, Random.value);
			endC = new Color(1f, Random.value, Random.value);
			lr.SetColors(startC, endC);
			lr.enabled = true;
			StartCoroutine( FireRoutine(target) );
		}
	}
	
	IEnumerator FireRoutine(Vector3 target)
	{
		firing = true;
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
		yield return new WaitForSeconds(0.2f);
		fireTime = Time.time;
		firing = false;
	}

}
