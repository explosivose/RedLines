using UnityEngine;
using System.Collections;

public class HyperMatter : MonoBehaviour 
{
	public Transform hyperDustPrefab;
	public Transform hyperMatterHintPrefab;
	public Transform explosion;
	public float minSpeedFactor = 0.75f;
	public float maxSpeedFactor = 1.25f;
	
	private float speed;
	private Vector3 initialScale;
	private Transform hyperMatterHint;
	private bool provideHint = false;
	
	void Start()
	{
		float cubeSpeed = CubeMaster.Instance.cubeSpeed;
		speed = Random.Range(minSpeedFactor * cubeSpeed, maxSpeedFactor * cubeSpeed);
		initialScale = transform.localScale;
		Destroy(this.gameObject, 30f);
		
		if (Time.timeSinceLevelLoad < 10f)
		{
			hyperMatterHint = Instantiate(hyperMatterHintPrefab) as Transform;
			provideHint = true;
		}
	}
	
	void Update()
	{
		transform.position += Vector3.back * speed * Time.deltaTime;
		Vector3 scale = initialScale * (Mathf.Sin (Time.time*4f)/4f + 1f);
		transform.localScale = scale;
		transform.Rotate(Vector3.one);
		if (provideHint)
		{
			hyperMatterHint.position = transform.position;
		}
	}
	
	public void Explode()
	{
		Instantiate(explosion, transform.position, Random.rotation);
		
		for (int i = 0; i < Random.Range(4, 8); i++)
		{
			Vector3 pos = transform.position + Random.insideUnitSphere;
			Transform dust = Instantiate(hyperDustPrefab, pos, Quaternion.identity) as Transform;
			dust.parent = transform;
		}
		
		if (provideHint)
		{
			provideHint = false;
			Destroy(hyperMatterHint.gameObject);
		}
		
		renderer.enabled = false;
		collider.enabled = false;
	}
	
}
