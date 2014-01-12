using UnityEngine;
using System.Collections;
using System.Collections.Generic; // required for List<T>

public class SectionData
{
	private float dist = 0f;
	private float center = 0f;
	private float up = 0f;
	private float down = 0f;
	
	private GameObject cube;
	
	private void Rescale(ref float value, float oldMax, float oldMin, float newMax, float newMin)
	{
		float oldRange = oldMax - oldMin;
		float newRange = newMax - newMin;
		value = (((value - oldMin)*newRange)/oldRange)+newMin;
	}
	
	// constructors
	public SectionData(float distance, float minGap, float maxGap)
	{
		Distance = distance;
		up = Mathf.PerlinNoise(dist,1f);
		Rescale(ref up, 1f, 0f, maxGap/2f, minGap/2f);
		up = center + up;
		
		down = Mathf.PerlinNoise(dist,-1f);
		Rescale(ref down, 1f, 0f, maxGap/2f, minGap/2f);
		down = center - down;
	}
	
	// destructor
	~SectionData()
	{
		
	}
	
	// generate data based on perlin noise using Distance as a lookup
	public float Distance
	{
		get { return dist; }
		set {
			dist = value;
			center = Mathf.PerlinNoise(dist, 0f);
			up = center + Mathf.PerlinNoise(dist,1f);
			down = center - Mathf.PerlinNoise(dist,-1f);
		}
	}
	
	// return vertices
	public Vector3 Center 
	{
		get { return new Vector3(dist + center,center); }
		set { center = value.x; }
	}
	public Vector3 Top
	{
		get { return new Vector3(dist + center, up); }
		set { up = value.y; } // may need to protect against setting up < down and down > up
	}
	public Vector3 Bottom
	{
		get { return new Vector3(dist + center, down); }
		set { down = value.y; }
	}

}

public class LevelGenerator : MonoBehaviour 
{
	public float levelLength;
	public float minX; public float maxX;
	public float minY; public float maxY;
	public bool levelDebug;
	
	
	private Player player;
	private List<SectionData> data = new List<SectionData>();	// world positions for mesh data
	
	// alias the furthest and shortest distances in level data
	private float head {
		get { return data[data.Count-1].Distance; }
	}
	private float tail {
		get { return data[0].Distance; }
	}
	
	private void Start()
	{
		// get reference to player script
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		if (player == null)
			Debug.LogError("Could not find player!");
		
		
		// if these are negative then you may be stuck in an inf loop
		if (minX < 0f || maxX < 0f)
			Debug.LogError("MinX and MaxX must be positive values");
		
		
		// generate the first bits of data
		data.Add(new SectionData(transform.position.x, minY, maxY));
		while (head < player.transform.position.x + levelLength)
		{
			GenerateDataAtHead();
		}
	}
	
	private void FixedUpdate()
	{
		// keep generating stuff ahead of the player
		if (head < player.transform.position.x + levelLength)
		{
			GenerateDataAtHead();
		}
		
		// forget old level data
		if (tail < player.transform.position.x - levelLength)
		{
			DestroyDataAtTail();
		}
		
		if (levelDebug) DebugDrawLevel();
	}
	
	private void GenerateDataAtHead()
	{
		data.Add(new SectionData(head + Random.Range(minX, maxX), minY, maxY ));
		data.Add(data[data.Count-1]);
	}
	
	private void DestroyDataAtTail()
	{
		data.RemoveAt(0);
		data.RemoveAt(0);
	}
	
	private void DebugDrawLevel()
	{
		for(int i = 0; i < data.Count - 1; i++)
		{
			Debug.DrawLine(data[i].Top, data[i+1].Top);
			Debug.DrawLine(data[i].Bottom, data[i+1].Bottom);
			Debug.DrawLine (data[i].Center, data[i].Top );
			Debug.DrawLine (data[i].Center, data[i].Bottom );
		}
	}


	
}



[System.Serializable]
public class Level
{
	public string folderName;
	
	// How much to increment player acceleraetion by for this level
	public float thrustIncrease = 15f;
	
	// The colour gradients to use (actual color is dependant on player velocity)
	public Color sunColour;
	public Gradient flameGradient;
	
	private LevelTransitionTrigger trigger;
	public Transform Trigger
	{
		get
		{
			return trigger.transform;
		}
	}
	
	// Section data for this level (set in inspector)
	public LevelSection[] sections = new LevelSection[0];

	public void LoadSections()
	{
		foreach(LevelSection section in sections)
		{
			section.LoadPieces(folderName);
		}
		trigger = Resources.Load<Transform>("leveltrigger").GetComponent<LevelTransitionTrigger>();
		trigger.thrustIncrease = thrustIncrease;
		trigger.nextSunColour = sunColour;
	}
}

[System.Serializable]
public class LevelSection
{
	public string folderName;
	public int numberOfPiecesToSpawn = 0;
	
	private Transform[] pieces;
	public Transform[] Pieces
	{ 
		get
		{
			return pieces;
		}
	}
	
	public void LoadPieces(string levelFolderName)
	{
		Debug.Log ("Loading: Resources/" + levelFolderName + "/" + folderName);
		pieces = Resources.LoadAll<Transform>(levelFolderName + "/" + folderName);
	}
}
