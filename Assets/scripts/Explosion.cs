using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	public float endSize = 10f;
	public float speedOfExpansion = 1f;
	public Material[] materials = new Material[1];

	private float startTime;
	private int matIndex;
	private Transform player;
	// Use this for initialization
	void Awake () 
	{
		matIndex = Random.Range (0, materials.Length);
		renderer.material = materials [matIndex];
		startTime = Time.time;
		player = GameObject.FindWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float timeToExpand = endSize / speedOfExpansion;
		float scale = Mathf.Lerp (0f, endSize, (Time.time - startTime) * 1/timeToExpand);
		transform.localScale = Vector3.one * scale * 4f;

		Vector2 origin;
		origin.x = transform.position.x;
		origin.y = transform.position.y;

		Vector2 playerOrigin;
		playerOrigin.x = player.position.x;
		playerOrigin.y = player.position.y;

		if (Vector2.Distance (origin, playerOrigin) < scale)
			player.rigidbody2D.AddForce ((playerOrigin - origin) * endSize);

		Collider2D[] hits = Physics2D.OverlapCircleAll (origin, scale);
		foreach (Collider2D hit in hits) 
		{
			if (hit.rigidbody2D != null)
			{
				Vector2 hitOrigin;
				hitOrigin.x = hit.transform.position.x;
				hitOrigin.y = hit.transform.position.y;
				hit.rigidbody2D.AddForce((hitOrigin - origin)*endSize*10f);
				Debug.Log(hit.name + "was hit by explosion force.");
			}

		}
	}
}
