using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CubeMeta
{
	public float startTime = 0f;
	public Vector3 startPosition;
	public Vector3 currentPosition;
	public Vector3 targetPosition;
}

public class CubeMaster : MonoBehaviour 
{
	public GameObject cubePrefab;
	public int numberOfCubes = 1000;
	public Vector3 cubeScale = Vector3.one;
	public float cubeSpeed = 5f;
	
	// list of cube objects
	private List<Transform> cubeList = new List<Transform>();
	private int cubeListHead = 0;
	
	// list of cube start and target positions
	private List<CubeMeta> cubeMetaList = new List<CubeMeta>();
	
	
	// position offset added to all start positions of cubes
	private Vector3 masterSpawnOffset = Vector3.zero;
	
	// approximate number of 'slices' in the level
	// a slice is a wall of cubes
	private float sliceCount;
	
	// initialization
	void Start () 
	{
		cubePrefab.transform.localScale = cubeScale;
		// spawn a bunch of cubes
		for (int i = 0; i < numberOfCubes; i++)
		{
			GameObject cube = (GameObject)Instantiate(cubePrefab, new Vector3(0f,(float)i), Quaternion.identity);
			cube.transform.parent = transform;
			cubeList.Add(cube.transform);
		}
		
		// start level loop
		StartCoroutine(LevelLoop());
	}
	
	IEnumerator LevelLoop()
	{
		while (true)
		{
			// get the next CubeMeta data for the next Slice
			List<CubeMeta> cubeMetaSlice = LevelGenerator.Generate3D(cubeScale.y);
			
			// figure out the position ahead of the player to spawn
			masterSpawnOffset = Vector3.back;// * distance;
			
			// add CubeMetaSlice to cubeMetaQ
			foreach (CubeMeta m in cubeMetaSlice)
				cubeMetaList.Add(m);
			
			// remove from start of Q if Q length is > numberOfCubes
			while ( cubeMetaList.Count > numberOfCubes)
				cubeMetaList.RemoveAt(0);
			
			// figure out the rate cube spawning
			float time = cubeScale.z / cubeSpeed;
			yield return new WaitForSeconds(time);
		}
	}
	
	void Update()
	{
		// calculate cube positions using meta data
		float travelTime;
		float lerp;
		foreach(CubeMeta m in cubeMetaList)
		{
			travelTime = Time.time - m.startTime;
			lerp = travelTime;
			m.currentPosition.x = Mathf.Lerp(m.startPosition.x, m.targetPosition.x, lerp);
			m.currentPosition.y = Mathf.Lerp(m.startPosition.y, m.targetPosition.y, lerp);
			m.currentPosition.z = masterSpawnOffset.z + cubeSpeed*travelTime;
			
		}
		// assign positions to cubes
		for (int i = 0; i < cubeMetaList.Count; i++)
		{
			cubeList[i].position = cubeMetaList[i].currentPosition;
		}
	}
}
