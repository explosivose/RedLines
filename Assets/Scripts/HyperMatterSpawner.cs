using UnityEngine;
using System.Collections;

public class HyperMatterSpawner : MonoBehaviour
{
	public Transform hyperMatterPrefab;
	public float maxTime = 6f;
	public float minTime = 2f;
	
	void Start()
	{
		StartCoroutine( SpawnLoop() );
	}
	
	IEnumerator SpawnLoop()
	{
		while(true)
		{
			yield return new WaitForSeconds(Random.Range(minTime, maxTime));
			float offsetDistance = Random.Range(0f, LevelGenerator.MaxRadius);
			Vector3 pos = LevelGenerator.CurrentPosition + Random.onUnitSphere * offsetDistance;
			Instantiate(hyperMatterPrefab, pos, Random.rotation);
		}
	}

}
