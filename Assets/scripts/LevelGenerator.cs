using UnityEngine;
using System.Collections;
using System.Collections.Generic; // required for List<T>


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class LevelGenerator : MonoBehaviour 
{



	public float levelLength;
	public float xDetail; public float xVariation;
	public float minGapSize; public float maxGapSize;
	public bool levelDebug;
	
	
	private Player player;
	private static List<SectionData> data = new List<SectionData>();	// world positions for mesh data
	private static float minX; private static float maxX;
	private static float minY; private static float maxY;
	
	
	// alias the furthest and shortest distances in level data
	private static float head {
		get { return data[data.Count-1].Distance; }
	}
	private static float tail {
		get { return data[0].Distance; }
	}
	
	private class SectionData
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
		public SectionData()
		{
			Distance = 0f;
			up = Mathf.PerlinNoise(dist,1f);
			Rescale(ref up, 1f, 0f, maxY/2f, minY/2f);
			up = center + up;
			
			down = Mathf.PerlinNoise(dist,-1f);
			Rescale(ref down, 1f, 0f, maxY/2f, minY/2f);
			down = center - down;
		}
		
		public SectionData(float x)
		{
			Distance = x;
			up = Mathf.PerlinNoise(dist,1f);
			Rescale(ref up, 1f, 0f, maxY/2f, minY/2f);
			up = center + up;
			
			down = Mathf.PerlinNoise(dist,-1f);
			Rescale(ref down, 1f, 0f, maxY/2f, minY/2f);
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
	
	void OnLevelWasLoaded(int level)
	{
		data.Clear();
	}
	
	private void Start()
	{
		// initialise static level data parameters
		minX = xDetail - xVariation;
		maxX = xDetail + xVariation;
		minY = minGapSize;
		maxY = maxGapSize;
	
		// get reference to player script
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		if (player == null)
			Debug.LogError("Could not find player!");
		
		
		// if these are negative then you may be stuck in an inf loop
		if (minX < 0f || maxX < 0f)
			Debug.LogError("MinX and MaxX must be positive values");
		
		
		// generate the first bits of data
		data.Add(new SectionData(transform.position.x));
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
		if (tail < player.transform.position.x - (levelLength/2f))
		{
			DestroyDataAtTail();
		}
		
		if (levelDebug) DebugDrawLevel();
		
		CreateMeshVertices();
	}
	
	private void GenerateDataAtHead()
	{
		data.Add(new SectionData(head + Random.Range(minX, maxX)));
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
	Vector3[] verts;
	Vector2[] uvs;
	int[] triangles;
	private void CreateMeshVertices()
	{
		verts = new Vector3[data.Count * 2];
		uvs = new Vector2[data.Count * 2];
		triangles = new int[(data.Count * 2 * 6) - 12];
		
		// verts and uvs
		int v = 0;
		for (int i = 0; i < data.Count; i++)
		{
			verts[v] = data[i].Top; 
			uvs[v++] = new Vector2(v,0);
			verts[v] = data[i].Top + Vector3.up * 10f;
			uvs[v++] = new Vector2(v,1);
		}
		
		// mesh triangles
		int t = 0;
		for (v = 0; v < data.Count -1; v+=2)
		{
			triangles[t++] = v;
			triangles[t++] = v + 1;
			triangles[t++] = v + 2;
			
			triangles[t++] = v + 3;
			triangles[t++] = v + 2;
			triangles[t++] = v + 1;
		}
		
		Mesh m = GetComponent<MeshFilter>().mesh;
		m.Clear();
		m.vertices = verts;
		m.uv = uvs;
		m.triangles = triangles;
		m.RecalculateNormals();
		
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
