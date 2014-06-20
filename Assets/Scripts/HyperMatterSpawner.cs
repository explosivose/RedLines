using UnityEngine;
using System.Collections;

public class HyperMatterSpawner : MonoBehaviour
{
	public Transform hyperMatterPrefab;
	public float maxTime = 6f;
	public float minTime = 2f;
	
	private bool hyper = false;
	
	void Start()
	{
		StartCoroutine( SpawnLoop() );
	}
	
	void FixedUpdate()
	{
		if (CubeMaster.Instance.HyperJump && !hyper) StartCoroutine(HyperSpawn());
	}
	
	
	IEnumerator SpawnLoop()
	{
		while(true)
		{
			yield return new WaitForSeconds(Random.Range(minTime, maxTime));
			if (!CubeMaster.Instance.HyperJump)
			{
				float offsetDistance = Random.Range(0f, LevelGenerator.MaxRadius -1f);
				Vector3 pos = LevelGenerator.CurrentPosition + Random.onUnitSphere * offsetDistance;
				Instantiate(hyperMatterPrefab, pos, Random.rotation);
			}
		}
	}
	
	// avoid spawning hypermatter near the player whilst in hyperjump
	IEnumerator HyperSpawn()
	{
		hyper = true;
		while(CubeMaster.Instance.HyperJump)
		{
			float offsetDistance = Random.Range(2f, LevelGenerator.MaxRadius);
			Vector3 pos = LevelGenerator.CurrentPosition + Random.onUnitSphere * offsetDistance;
			Instantiate(hyperMatterPrefab, pos, Random.rotation);
			yield return new WaitForSeconds(Random.Range(0.1f, 2f));
		}
		hyper = false;
	}

}
