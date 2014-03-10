using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeMaster : MonoBehaviour 
{

	// Cubes
	public GameObject cubePrefab;
	public int numberOfCubes = 1000;
	public float cubeSize = 1f;
	public float cubeRatePerSecond;
	public float cubeSpeed;
	
	// Map
	public Texture2D levelMap;
	public int mapHeight;
	public int mapLength = 0;
	public int mapRepeatCount = 0;
	public float cubeYMotionSmoothness;
	public float cubeZMotionSmoothness;
	public int gap = 8;
	public int minWall = 4;
	public int maxWall = 8;

	// Audio
	public float audioBeat = 0f;
	private float[] samples;
	
	private Transform player;
	private List<Transform> cubeList     = new List<Transform>();
	private int 			cubeListHead = 0;
	
	void Start () 
	{
		samples = new float[1024];
		player = GameObject.FindGameObjectWithTag("Player").transform;
		
		for (int i = 0; i < numberOfCubes; i++)
		{
			GameObject cube = (GameObject)Instantiate(cubePrefab, new Vector3(0f,(float)i), Quaternion.identity);
			cube.transform.parent = transform;
			cubeList.Add(cube.transform);
		}
		
		StartCoroutine( MakeLevel3D() );

	}
	
	// Read a level map from an image file
	IEnumerator ImageMapReader() 
	{

		// Loop repeat read of level map
		for(int k = 0; k < mapRepeatCount; k++)
		{
			for(int i = 0; i < mapLength; i++)
			{
				yield return new WaitForSeconds(1f/cubeRatePerSecond);
				List<CubeMeta> line = new List<CubeMeta>();
				List<CubeMeta> line2 = new List<CubeMeta>();

				for(int h = 0; h < mapHeight; h++)
				{
					if(levelMap.GetPixel(i,h).a == 1)
					{
						CubeMeta tempCube = new CubeMeta();
						tempCube.targetPosition = new Vector3(-50,h,0);
						tempCube.startPosition = new Vector3(-50, Random.Range(-20,20), Random.Range(10,20));
						tempCube.positionOffset = new Vector3(0,0,0);
						tempCube.audioBeat = levelMap.GetPixel(i,h).r;
						line.Add(tempCube);
					}

					if(levelMap.GetPixel(i,h).a == 1)
					{
						CubeMeta tempCube = new CubeMeta();
						tempCube.targetPosition = new Vector3(-50,h,0);
						tempCube.startPosition = new Vector3(-50, Random.Range(-20,20), Random.Range(-10,-20));
						tempCube.positionOffset = new Vector3(0,0,1);
						tempCube.audioBeat = levelMap.GetPixel(i,h).r;
						line2.Add(tempCube);
					}
				}
				LineMaker(line);
				LineMaker(line2);
			}
		}
	}

	private List<LevelGenerator.moving> levelStates = new List<LevelGenerator.moving>();
	float cubeLifetime = 0f;
	IEnumerator MakeLevel()
	{
		while(true)
		{
			int sinGap = Mathf.RoundToInt( gap * ( Mathf.Sin (Time.time/2f) + 2f ) );
			LineMaker(LevelGenerator.Generate(sinGap, minWall, maxWall));
			
			// Store level states for camera rotation calcs
			levelStates.Add(LevelGenerator.State);
			
			// levelLength: this oughta be calculated somehow
			// it's the distance from the CubeMaster to the CubeMonster
			float levelLength = 75f;
			cubeLifetime = levelLength / (cubeSpeed*cubeRatePerSecond);
			if (Time.time > cubeLifetime) levelStates.RemoveAt(0);
			yield return new WaitForSeconds(1f/cubeRatePerSecond);
		}
	}
	
	IEnumerator MakeLevel3D()
	{
		while(true)
		{
			LineMaker(LevelGenerator.Generate3D(cubeSize));
			levelStates.Add(LevelGenerator.State);
			yield return new WaitForSeconds(1f/cubeRatePerSecond);
		}
	}
	
	int accumulate = 0;
	float slant = 0f;
	
	void CameraAngle()
	{
		accumulate = 0;
		foreach(LevelGenerator.moving state in levelStates)
		{
			switch (state)
			{
			case LevelGenerator.moving.positive2:
				accumulate += 2;
				break;
				
			case LevelGenerator.moving.positive:
				accumulate++;
				break;
				
			case LevelGenerator.moving.straight:
				break;
				
			case LevelGenerator.moving.negative:
				accumulate--;
				break;
				
			case LevelGenerator.moving.negative2:
				accumulate -= 2;
				break;
			}
		}
		slant = (float)accumulate / (float)levelStates.Count;
		Quaternion rotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
		rotation *= Quaternion.AngleAxis(slant * -10f, transform.right);
		Transform cam = Camera.main.transform;
		cam.rotation = Quaternion.Lerp(cam.rotation, rotation, Time.deltaTime);
		cam.position = player.position + new Vector3(1.5f, 0.5f, 0f);
		
	}


	public void LineMaker(List<CubeMeta> lineMeta)
	{
		foreach(CubeMeta meta in lineMeta)
		{
			// Create new cube at starting position
			meta.startPosition += transform.position;
			//GameObject newCube = (GameObject)Instantiate(cubePrefab, meta.startPosition, Quaternion.identity);
			Transform newCube = cubeList[cubeListHead++];
			if (cubeListHead >= numberOfCubes) cubeListHead = 0;
			
			newCube.position = meta.startPosition;
			/*if (newCube.localScale != new Vector3(cubeSize, cubeSize, cubeSize));
			{
				newCube.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
			}*/
			
			
			Cube cubeScript = newCube.GetComponent<Cube>();

			// Initialize cube 
			cubeScript.targetPosition = meta.targetPosition + meta.positionOffset + transform.position;
			cubeScript.audioBeat = meta.audioBeat;
			cubeScript.SetMotionSmooth(new Vector3(0f, cubeYMotionSmoothness, cubeZMotionSmoothness));
		}
	}

	int cubeCount = 0;
	void Update () {
		audio.GetOutputData(samples, 0);
		float sum = 0;
		for (int i=0; i < 1024; i++){
			sum += samples[i]*samples[i]; // sum squared samples
		}
		audioBeat = Mathf.Sqrt(sum/1024);
		cubeCount = transform.childCount;
		CameraAngle();
	}
	
	
	
}


