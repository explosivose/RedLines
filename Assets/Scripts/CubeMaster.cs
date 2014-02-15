using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeMaster : MonoBehaviour {

	// Cubes
	public GameObject cube;
	public float cubeRatePerSecond;
	public float cubeSpeed;

	// Map
	public Texture2D levelMap;
	public int mapHeight;
	public int mapLength = 0;
	public int mapRepeatCount = 0;
	public int cubeYMotionSmoothness;
	public int cubeZMotionSmoothness;

	// Audio
	public float audioBeat = 0f;
	private float[] samples;
	
	
	void Start () {
		StartCoroutine("ImageMapReader");
		samples = new float[1024];
	}
	
	// Read a level map from an image file
	IEnumerator ImageMapReader() {

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
						tempCube.targetPosition = new Vector3(0,h,0);
						tempCube.startPosition = new Vector3(0f, Random.Range(-20,20), Random.Range(10,20));
						tempCube.positionOffset = new Vector3(0,0,0);
						tempCube.audioBeat = levelMap.GetPixel(i,h).r;
						line.Add(tempCube);
					}

					if(levelMap.GetPixel(i,h).a == 1)
					{
						CubeMeta tempCube = new CubeMeta();
						tempCube.targetPosition = new Vector3(0,h,0);
						tempCube.startPosition = new Vector3(0f, Random.Range(-20,20), Random.Range(-10,-20));
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



	void LineMaker(List<CubeMeta> allCubes)
	{
		foreach(CubeMeta oneCube in allCubes){
			// Create new cube at starting position
			GameObject newCube = (GameObject)Instantiate(cube, oneCube.startPosition, Quaternion.identity);
			newCube.transform.parent = this.transform;
			Cube cubeScript = newCube.GetComponent<Cube>();

			// Initialize cube 
			cubeScript.targetPosition = oneCube.targetPosition + oneCube.positionOffset;
			cubeScript.audioBeat = oneCube.audioBeat;
			cubeScript.SetMotionSmooth(new Vector3(0f, cubeYMotionSmoothness, cubeZMotionSmoothness));
		}
	}

	
	void Update () {
		audio.GetOutputData(samples, 0);
		float sum = 0;
		for (int i=0; i < 1024; i++){
			sum += samples[i]*samples[i]; // sum squared samples
		}
		audioBeat = Mathf.Sqrt(sum/1024);
	}
	
}


