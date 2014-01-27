using UnityEngine;
using System.Collections;
using System.Collections.Generic; // required for List<T>

[System.Serializable]
public class HyperSpace
{
	public string name = "";
	
	private float topSeed = 0f;
	public float topSampleRate = 0f;
	public float maxTop = 0f;
	public float minTop = 0f;
	public float TopSeed
	{
		get { return topSeed;  }
		set { topSeed = value; }
	}
	
	public float maxMidChange = 0f;
	public float midSampleRate = 0f;
	private float midSeed = 0f;
	public float MidSeed
	{
		get { return midSeed;  }
		set { midSeed = value; }
	}
	
	public float minBot = 0f;
	public float maxBot = 0f;
	public float botSampleRate = 0f;
	private float botSeed = 0f;
	public float BotSeed
	{
		get { return botSeed;  }
		set { botSeed = value; }
	}
	
	public void RandomSeeds()
	{
		midSeed = Random.value;
		topSeed = Random.value;
		botSeed = Random.value;
	}
}

public class LevelGenerator : MonoBehaviour 
{
	public bool levelDebug;
	public Transform hyperMatter;
	public Material levelMaterial;
	public float levelLength;
	public bool spawnHyperMatter = true;
	private float t;
	// public variables that can be edited in the inspectorrrr
	public HyperSpace currentHyperSpace;
	private HyperSpace oldHyperSpace;
	private HyperSpace newHyperSpace;
	
	public void SetHyperSpace(HyperSpace hyperSpace_new)
	{
		Debug.Log ("New hyperspace set.");
		oldHyperSpace = newHyperSpace;
		newHyperSpace = hyperSpace_new;
		hyperTime = Time.time;
	}
	
	private float hyperTime = 0f;
	
	private Player player;
	
	// this shit has to be static so that SectionData objects can access them reliably
	private static List<SectionData> data = new List<SectionData>();	// world positions for mesh data
	
	// basically a copy of the public floats above because static shit can't be
	// easily exposed for the unity inspector

	private static HyperSpace hyperSpace;
	
	private GameObject topMesh;
	private GameObject botMesh;
	
	// alias the furthest and shortest distances in level data
	public static Vector3 head {
		get { return data[data.Count-1].Center; }
	}
	public static Vector3 tail {
		get { return data[0].Center; }
	}
	
	[System.Serializable]
	private class SectionData
	{
		// constructors
		public SectionData(float distance)
		{
			dist = distance;
			Calculate();
		}
		
		public float Distance
		{
			get { return dist; }
			set { 
				dist = value; 
				Calculate();
			}
		}
		
		// return vertices
		public Vector3 Center {
			get { return new Vector3(dist, mid); }
		}
		
		public Vector3 Top {
			get { return new Vector3(dist, mid + up); }
		}
		
		public Vector3 Bottom {
			get { return new Vector3(dist, mid - down); }
		}
		
		private float dist = 0f;
		private float up = 0f;
		private float mid = 0f;
		private float down = 0f;
		
		private void Calculate()
		{
			up = Mathf.PerlinNoise(dist*hyperSpace.topSampleRate, -0.1f);
			Rescale(ref up, 1f, 0f, hyperSpace.maxTop, hyperSpace.minTop);
			
			mid = Mathf.PerlinNoise(dist*hyperSpace.midSampleRate, 0f);
			Rescale (ref mid, 1f, 0f, hyperSpace.maxMidChange, 0f);
			
			down = Mathf.PerlinNoise(dist*hyperSpace.botSampleRate, 0.1f);
			Rescale(ref down, 1f, 0f, hyperSpace.maxBot, hyperSpace.minBot);
		}
		
		private void Rescale(ref float value, float oldMax, float oldMin, float newMax, float newMin)
		{
			float oldRange = oldMax - oldMin;
			float newRange = newMax - newMin;
			value = (((value - oldMin)*newRange)/oldRange)+newMin;
		}
		
	}
	
	void OnLevelWasLoaded(int level)
	{
		data.Clear();
	}
	
	private void Start()
	{
		hyperSpace = currentHyperSpace;
		newHyperSpace = HyperSpaceMaker.NewHyperSpace;
		oldHyperSpace = currentHyperSpace;

		
		// make child objects for containing level meshes
		topMesh = new GameObject("topMesh");
		topMesh.transform.parent = transform;
		topMesh.AddComponent<MeshFilter>();
		topMesh.AddComponent<MeshRenderer>();
		topMesh.GetComponent<MeshRenderer>().material = levelMaterial;
		topMesh.AddComponent<Rigidbody>();
		topMesh.rigidbody.isKinematic = true;
		topMesh.AddComponent<MeshCollider>();
		
		botMesh = new GameObject("botMesh");
		botMesh.transform.parent = transform;
		botMesh.AddComponent<MeshFilter>();
		botMesh.AddComponent<MeshRenderer>();
		botMesh.GetComponent<MeshRenderer>().material = levelMaterial;
		botMesh.AddComponent<Rigidbody>();
		botMesh.rigidbody.isKinematic = true;
		botMesh.AddComponent<MeshCollider>();
		
		// get reference to player script
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		if (player == null)
			Debug.LogError("Could not find player!");
		
		// generate the first bits of data
		
		data.Add(new SectionData(transform.position.x));
		while (head.x < player.transform.position.x + levelLength)
		{
			t = (head.x - player.transform.position.x)/levelLength;
			hyperSpace.maxTop = Mathf.Lerp(oldHyperSpace.maxTop, newHyperSpace.maxTop, t);
			hyperSpace.minTop = Mathf.Lerp(oldHyperSpace.minTop, newHyperSpace.minTop, t);
			hyperSpace.topSampleRate = Mathf.Lerp(oldHyperSpace.topSampleRate, newHyperSpace.topSampleRate, t );
			hyperSpace.TopSeed = Mathf.Lerp(oldHyperSpace.TopSeed, newHyperSpace.TopSeed, t );
			
			hyperSpace.maxMidChange = Mathf.Lerp (oldHyperSpace.maxMidChange, newHyperSpace.maxMidChange, t);
			hyperSpace.midSampleRate = Mathf.Lerp (oldHyperSpace.midSampleRate, newHyperSpace.midSampleRate, t);
			hyperSpace.MidSeed = Mathf.Lerp (oldHyperSpace.MidSeed, newHyperSpace.MidSeed, t);
			
			hyperSpace.maxBot = Mathf.Lerp(oldHyperSpace.maxBot, newHyperSpace.maxBot, t);
			hyperSpace.minBot = Mathf.Lerp(oldHyperSpace.minBot, newHyperSpace.minBot, t);
			hyperSpace.botSampleRate = Mathf.Lerp (oldHyperSpace.botSampleRate, newHyperSpace.botSampleRate, t);
			hyperSpace.BotSeed = Mathf.Lerp (oldHyperSpace.BotSeed, newHyperSpace.BotSeed, t);
			GenerateDataAtHead();
		}
	}
	
	
	
	private void FixedUpdate()
	{
		// level parameter floats must be static for StageData objects
		// they're private because public statics are not easily exposed to the Unity inspector
		// public floats are exposed in the Unity inspector (useful)
		// this is dirty but easy getaround for realtime updates to level generator parameters
		t = (Time.time - hyperTime) / 10f;
		
		currentHyperSpace = hyperSpace;
		
		hyperSpace.maxTop = Mathf.Lerp(oldHyperSpace.maxTop, newHyperSpace.maxTop, t);
		hyperSpace.minTop = Mathf.Lerp(oldHyperSpace.minTop, newHyperSpace.minTop, t);
		hyperSpace.topSampleRate = Mathf.Lerp(oldHyperSpace.topSampleRate, newHyperSpace.topSampleRate, t );
		hyperSpace.TopSeed = Mathf.Lerp(oldHyperSpace.TopSeed, newHyperSpace.TopSeed, t );
		
		hyperSpace.maxMidChange = Mathf.Lerp (oldHyperSpace.maxMidChange, newHyperSpace.maxMidChange, t);
		hyperSpace.midSampleRate = Mathf.Lerp (oldHyperSpace.midSampleRate, newHyperSpace.midSampleRate, t);
		hyperSpace.MidSeed = Mathf.Lerp (oldHyperSpace.MidSeed, newHyperSpace.MidSeed, t);
				
		hyperSpace.maxBot = Mathf.Lerp(oldHyperSpace.maxBot, newHyperSpace.maxBot, t);
		hyperSpace.minBot = Mathf.Lerp(oldHyperSpace.minBot, newHyperSpace.minBot, t);
		hyperSpace.botSampleRate = Mathf.Lerp (oldHyperSpace.botSampleRate, newHyperSpace.botSampleRate, t);
		hyperSpace.BotSeed = Mathf.Lerp (oldHyperSpace.BotSeed, newHyperSpace.BotSeed, t);
		
		
		bool updateMesh = false;
		
		// keep generating stuff ahead of the player
		if (head.x < player.transform.position.x + levelLength)
		{
			GenerateDataAtHead();
			updateMesh = true;
		}
		
		// forget old level data
		if (tail.x < player.transform.position.x - levelLength)
		{
			DestroyDataAtTail();
			updateMesh = true;
		}
		
		if (levelDebug) DebugDrawLevel();
		
		if (updateMesh) UpdateMesh();
	}
	
	private void GenerateDataAtHead()
	{
		data.Add(new SectionData(head.x + player.thrust));
		if (!GameManager.Instance.ChangingHyperSpace)
		{
			float r = Random.value;
			if (r < 0.01f) Instantiate (hyperMatter, head, Random.rotation);
		}

	}
	
	private void DestroyDataAtTail()
	{
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
		Debug.DrawLine (data[data.Count-1 ].Center, data[data.Count-1 ].Top );
		Debug.DrawLine (data[data.Count-1 ].Center, data[data.Count-1 ].Bottom );
	}

	SectionData[] debugdata;
	
	private void SquareUpdateMesh()
	{
		Vector3[] verts = new Vector3[data.Count * 4];
		Vector2[] uvs = new Vector2[data.Count * 4];
		int[] triangles = new int[(data.Count * 2) * 3];
		
		// verts and UVs
		int v = 0;
		for (int i = 1; i < data.Count - 1; i++)
		{
			float x = data[i-1].Top.x + (data[i].Top.x - data[i-1].Top.x)/2f;
			float y = Mathf.Max(data[i].Top.y, data[i-1].Top.y);
			verts[v] = new Vector3(x,y);
			uvs[v++] = new Vector2(0,1);
			
			y = Mathf.Min (data[i].Top.y, data[i-1].Top.y);
			verts[v] = new Vector3(x,y);
			uvs[v++] = new Vector2(0,0);
			
			x = data[i].Top.x + (data[i+1].Top.x - data[i].Top.x)/2f;
			y = Mathf.Max(data[i].Top.y, data[i-1].Top.y);
			verts[v] = new Vector3(x,y);
			uvs[v++] = new Vector2(1,1);
			
			y = Mathf.Min (data[i].Top.y, data[i-1].Top.y);
			verts[v] = new Vector3(x,y);
			uvs[v++] = new Vector2(1,0);
			
			if (levelDebug)
			{
				Debug.DrawLine(verts[v-4], verts[v-3], Color.red,1f);
				Debug.DrawLine(verts[v-3],verts[v-1],Color.red,1f);
				Debug.DrawLine(verts[v-1],verts[v-2],Color.red,1f);
				Debug.DrawLine(verts[v-2],verts[v-4],Color.red,1f);
			}
		}
		
		// triangles
		int t = 0;
		for (v = 0; v < verts.Length; v+=4)
		{
			triangles[t++] = v;
			triangles[t++] = v + 2;
			triangles[t++] = v + 1;
			
			triangles[t++] = v + 3;
			triangles[t++] = v + 1;
			triangles[t++] = v + 2;
		}
		
		Mesh m = topMesh.GetComponent<MeshFilter>().mesh;
		m.Clear();
		m.vertices = verts;
		m.uv = uvs;
		m.triangles = triangles;
		m.RecalculateNormals();
		//topMesh.GetComponent<MeshCollider>().sharedMesh = null;
		//topMesh.GetComponent<MeshCollider>().sharedMesh = m;
	}
	
	private void UpdateMesh()
	{
		//debugdata = data.ToArray();
		Vector3[] verts = new Vector3[data.Count * 4];
		Vector2[] uvs = new Vector2[data.Count * 4];
		int[] triangles = new int[(data.Count * 4 * 6) - (4 * 3)];
		
		// TOP MESH
		
		// verts and uvs
		int v = 0;
		for (int i = 0; i < data.Count; i++)
		{
			verts[v] = data[i].Top; 
			uvs[v++] = new Vector2(v,0);
			verts[v] = data[i].Top + Vector3.up * 10f;
			uvs[v++] = new Vector2(v,1);
			verts[v] = data[i].Top + Vector3.forward * 10f;
			uvs[v++] = new Vector2(v,1);
		}
		
		// mesh triangles
		int t = 0;
		for (v = 0; v < (data.Count*3) - 3 ; v+=3)
		{
			triangles[t++] = v;
			triangles[t++] = v + 1;
			triangles[t++] = v + 3;
			
			triangles[t++] = v + 4;
			triangles[t++] = v + 3;
			triangles[t++] = v + 1;
			
			triangles[t++] = v;
			triangles[t++] = v + 3;
			triangles[t++] = v + 2;
			
			triangles[t++] = v + 5;
			triangles[t++] = v + 2;
			triangles[t++] = v + 3;
		}
		
		Mesh m = topMesh.GetComponent<MeshFilter>().mesh;
		m.Clear();
		m.vertices = verts;
		m.uv = uvs;
		m.triangles = triangles;
		m.RecalculateNormals();
		topMesh.GetComponent<MeshCollider>().sharedMesh = null;
		topMesh.GetComponent<MeshCollider>().sharedMesh = m;
		
		// DO IT ALL AGAIN FOR BOTTOM MESH WOO
		
		// verts and uvs
		v = 0;
		for (int i = 0; i < data.Count; i++)
		{
			verts[v] = data[i].Bottom; 
			uvs[v++] = new Vector2(v,0);
			verts[v] = data[i].Bottom - Vector3.up * 10f;
			uvs[v++] = new Vector2(v,1);
			verts[v] = data[i].Bottom + Vector3.forward * 10f;
			uvs[v++] = new Vector2(v,1);
		}
		
		// mesh triangles
		t = 0;
		for (v = 0; v < (data.Count*3) - 3 ; v+=3)
		{
			triangles[t++] = v;
			triangles[t++] = v + 2;
			triangles[t++] = v + 3;
			
			triangles[t++] = v + 5;
			triangles[t++] = v + 3;
			triangles[t++] = v + 2;
			
			triangles[t++] = v;
			triangles[t++] = v + 3;
			triangles[t++] = v + 1;
			
			triangles[t++] = v + 4;
			triangles[t++] = v + 1;
			triangles[t++] = v + 3;
		}
		
		m = botMesh.GetComponent<MeshFilter>().mesh;
		m.Clear();
		m.vertices = verts;
		m.uv = uvs;
		m.triangles = triangles;
		m.RecalculateNormals();
		botMesh.GetComponent<MeshCollider>().sharedMesh = null;
		botMesh.GetComponent<MeshCollider>().sharedMesh = m;
		
	}
}
