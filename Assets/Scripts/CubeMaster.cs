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
	public Vector3 layerCenter;
}

public class CubeMaster : Singleton<CubeMaster> 
{
	public GameObject cubePrefab;
	public bool animateCubes = true;
	public int numberOfCubes = 1000;
	public Vector3 cubeScale = Vector3.one;
	public float cubeSpeed = 5f;
	public float cubeAccel = 1f;
	public float cubeDecel = 4f;
	
	private float hyperJumpEnterTime = 0f;
	private float hyperJumpExitTime = 0f;
	private Transform player;
	
	// list of cube objects
	private List<Transform> cubeList = new List<Transform>();
	
	// list of cube start and target positions
	private List<CubeMeta> cubeMetaList = new List<CubeMeta>();
	
	// position offset added to all start positions of cubes
	private Vector3 masterSpawnOffset = Vector3.zero;
	

	private enum cubeMasterState {
		direct,		// cubes are forced to their target positions immediately
		animated, 	// cubes will smoothly fall into their target position over time
		hyperSpaceEnter, 	// cubes will fly away from the center 
		hyperSpaceExit		// cubes will fly from their current position to target position
	}
	
	private cubeMasterState masterState = cubeMasterState.direct;
	
	public bool HyperJump
	{
		get {
			if (masterState == cubeMasterState.hyperSpaceEnter
				|| masterState == cubeMasterState.hyperSpaceExit)
				return true;
			else
				return false;
		}
		set {
			// enter hyperspace
			if (value)
			{
				masterState = cubeMasterState.hyperSpaceEnter;
				hyperJumpEnterTime = Time.time;
			}
			// return from hyperspace
			else
			{
				masterState = cubeMasterState.hyperSpaceExit;
				hyperJumpExitTime = Time.time;
			}
		}
	}
	
	public float CubeTravelTime
	{
		get {
			float distance = transform.position.z - player.position.z;
			return distance/cubeSpeed;
		}
	}

	public void Decel()
	{
		cubeSpeed *= cubeDecel;
	}
	
	// initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	
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
		cubeSpeed += Time.deltaTime * cubeAccel;
		
		// calculate cube positions using meta data
		float travelTime;
		
		switch(masterState)
		{
		default:
		case cubeMasterState.direct:
			foreach(CubeMeta m in cubeMetaList)
			{
				travelTime = Time.time - m.startTime;
				m.currentPosition.x = m.targetPosition.x;
				m.currentPosition.y = m.targetPosition.y;
				m.currentPosition.z = masterSpawnOffset.z - cubeSpeed*travelTime;
			}
			break;
		case cubeMasterState.animated:
			foreach(CubeMeta m in cubeMetaList)
			{
				travelTime = Time.time - m.startTime;
				m.currentPosition.x = Mathf.Lerp(m.startPosition.x, m.targetPosition.x, travelTime);
				m.currentPosition.y = Mathf.Lerp(m.startPosition.y, m.targetPosition.y, travelTime);
				m.currentPosition.z = masterSpawnOffset.z - cubeSpeed*travelTime;
			}
			break;
		case cubeMasterState.hyperSpaceEnter:
			foreach(CubeMeta m in cubeMetaList)
			{
				travelTime = Time.time - m.startTime;
				Vector3 offset = m.targetPosition - m.layerCenter;
				float hyperTime = (Time.time - hyperJumpEnterTime) * 4f;
				m.currentPosition.x = m.targetPosition.x;
				m.currentPosition.y = m.targetPosition.y;
				m.currentPosition.z = masterSpawnOffset.z - cubeSpeed*travelTime;
				m.currentPosition += offset * hyperTime;
			}
			break;
		case cubeMasterState.hyperSpaceExit:
			foreach(CubeMeta m in cubeMetaList)
			{
				travelTime = Time.time - m.startTime;
				float hyperTime = (Time.time - hyperJumpExitTime) *  1f;
				m.currentPosition.x = Mathf.Lerp(m.currentPosition.x, m.targetPosition.x, hyperTime);
				m.currentPosition.y = Mathf.Lerp(m.currentPosition.y, m.targetPosition.y, hyperTime);
				m.currentPosition.z = masterSpawnOffset.z - cubeSpeed*travelTime;
				if (hyperTime > 1f) masterState = cubeMasterState.direct;
			}
			break;
		}
		
		// assign positions to cubes
		for (int i = 0; i < cubeMetaList.Count; i++)
		{
			cubeList[i].position = cubeMetaList[i].currentPosition;
		}
	}
}
