using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CubeMeta
{
	public float   startTime = 0f;
	public Vector3 startPosition;
	public Vector3 currentPosition;
	public Vector3 targetPosition;
	public Vector3 layerCenter;
	public int layerIndex;
}

// speed data is sampled every fixed update
// the data is then used to calculate cube positions
[System.Serializable]
public class SpeedData
{
	public float speed 		= 0f;
	public float time  		= 0f;
	public float distance 	= 0f;
	public SpeedData(float t, float v)
	{
		speed = v;
		time = t;
		distance = Time.fixedDeltaTime * v;
	}
}

public class CubeMaster : Singleton<CubeMaster> 
{
	public bool 		debug = false;
	public GameObject	cubePrefab;
	public bool 		animateCubes = true;
	public int 			numberOfCubes = 1000;
	public Vector3 		cubeScale = Vector3.one;
	public float 		cubeSpeed = 5f;
	public float 		cubeAccel = 1f;
	public float 		cubeDecel = 4f;
	public float 		cubeSpeedModifier = 1f;
	public int 			cubeSpeedSampleCount = 800;
	
	private float 		hyperJumpEnterTime = 0f;
	private float 		hyperJumpExitTime = 0f;
	private Transform 	player;
	private float 		cubeSpeedModified;
	
	// list of cube objects
	private List<Transform> cubeList = new List<Transform>();
	
	// list of cube start and target positions
	private List<CubeMeta> cubeMetaList = new List<CubeMeta>();
	
	// position offset added to all start positions of cubes
	private Vector3 masterSpawnOffset = Vector3.zero;
	
	private List<SpeedData> speedData = new List<SpeedData>();
	private int speedDataHead = 0;

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
		
		for (int i = 0; i < cubeSpeedSampleCount; i++)
		{
			speedData.Add(new SpeedData(0f, 0f));
		}	
		
		// spawn the first stretch of level before game starts
		LevelGenerator.Reset();
		/*
		float distance = Vector3.Distance(transform.position, player.position);
		float timer = - (distance/cubeSpeed);
		for (float i = 0f; i < distance; i += cubeScale.z)
		{
			List<CubeMeta> cubeMetaSlice = LevelGenerator.Generate3D(cubeScale.y);
			foreach (CubeMeta m in cubeMetaSlice)
			{
				m.startTime = timer;
				cubeMetaList.Add(m);
			}
			while ( cubeMetaList.Count > numberOfCubes)
				cubeMetaList.RemoveAt(0);
			timer += cubeScale.z / cubeSpeed;
		}*/
		
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
	
	void FixedUpdate()
	{
		cubeAccel += Time.deltaTime * 0.001f;
		cubeSpeed += Time.deltaTime * cubeAccel;
		cubeSpeedModified = cubeSpeed * cubeSpeedModifier;
		speedData[speedDataHead++] = new SpeedData(Time.time, cubeSpeedModified);
		if (speedDataHead >= cubeSpeedSampleCount) speedDataHead = 0;
	}
	
	void Update()
	{
		if (debug) D();

		// calculate cube positions using meta data
		float distance = 0f;
		float hyperTime = 0f;
		int layerIndex = -1;
		foreach(CubeMeta m in cubeMetaList)
		{
			// if we're on a new layer then recalculate the z distance
			if (layerIndex != m.layerIndex) 
			{
				layerIndex = m.layerIndex;
				distance = 0f;
				foreach(SpeedData d in speedData)
				{
					if (m.startTime < d.time)
						distance += d.distance;
				}
			}
			
			switch(masterState)
			{
			default:
			case cubeMasterState.direct:
				// map the target position straight to current position
				m.currentPosition.x = m.targetPosition.x;
				m.currentPosition.y = m.targetPosition.y;
				m.currentPosition.z = masterSpawnOffset.z - distance;
				break;
			case cubeMasterState.hyperSpaceEnter:
				// move current position away from layer center
				m.currentPosition.x = m.targetPosition.x;
				m.currentPosition.y = m.targetPosition.y;
				m.currentPosition.z = masterSpawnOffset.z - distance;
				Vector3 offset = m.targetPosition - m.layerCenter;
				hyperTime = (Time.time - hyperJumpEnterTime) * 4f;
				m.currentPosition += offset * hyperTime;
				break;
			case cubeMasterState.hyperSpaceExit:
				// lerp current position to target position
				hyperTime = (Time.time - hyperJumpExitTime) *  1f;
				m.currentPosition.x = Mathf.Lerp(m.currentPosition.x, m.targetPosition.x, hyperTime);
				m.currentPosition.y = Mathf.Lerp(m.currentPosition.y, m.targetPosition.y, hyperTime);
				m.currentPosition.z = masterSpawnOffset.z - distance;
				if (hyperTime > 1f) masterState = cubeMasterState.direct;
				break;
			}
		}

		// assign positions to cubes
		for (int i = 0; i < cubeMetaList.Count; i++)
		{
			cubeList[i].position = cubeMetaList[i].currentPosition;
		}
	}
	
	void D()
	{
		if (Input.GetKeyUp(KeyCode.Space))
			HyperJump = !HyperJump;
	}
}
