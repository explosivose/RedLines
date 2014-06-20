using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour {

	public Transform[] obstacles;
	
	public float initialTimeBetweenObstacles = 20f;
	public float variation = 0.5f;
	public float obstacleFrequencyFactor = 0.25f;
	public float minimumTimeBetweenObstacles = 4f;
	
	
	// Use this for initialization
	void Start () {
		StartCoroutine( SpawnLoop() );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator SpawnLoop() {
		yield return new WaitForSeconds(1f);
		while (true) {
			float wait = initialTimeBetweenObstacles - (Time.timeSinceLevelLoad * obstacleFrequencyFactor);
			float window = wait * variation;
			wait += Random.Range(-window, window);
			if (wait < minimumTimeBetweenObstacles) wait = minimumTimeBetweenObstacles;
			yield return new WaitForSeconds(wait);
			int index = Random.Range(0, obstacles.Length);
			float offsetDistance = Random.Range(0f, LevelGenerator.MaxRadius);
			Vector3 pos = LevelGenerator.CurrentPosition + Random.onUnitSphere * offsetDistance;
			Instantiate(obstacles[index], pos, Random.rotation);
		}
	}
}
