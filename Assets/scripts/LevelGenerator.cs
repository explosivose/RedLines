using UnityEngine;
using System.Collections;
using System.Collections.Generic; // required for List<T>

[System.Serializable]
public class HyperSpace
{
	public string name;
	public float maxMidChange;
	public float midSampleRate;
	public float maxTop;
	public float minTop;
	public float topSampleRate;
	public float maxBot;
	public float minBot;
	public float botSampleRate;
}

public class LevelGenerator : MonoBehaviour 
{
	public bool levelDebug;
	public Material levelMaterial;
	public float levelLength;
	
	// public variables that can be edited in the inspectorrrr
	public HyperSpace hyperSpace;
	
	public float xGap;
	
	private Player player;
	
	// this shit has to be static so that SectionData objects can access them reliably
	private static List<SectionData> data = new List<SectionData>();	// world positions for mesh data
	
	// basically a copy of the public floats above because static shit can't be
	// easily exposed for the unity inspector
	private static float maxMidChange;
	private static float midSampleRate;
	private static float minTop;
	private static float maxTop;
	private static float topSampleRate;
	private static float minBot;
	private static float maxBot;
	private static float botSampleRate;
	
	private GameObject topMesh;
	private GameObject botMesh;
	
	// alias the furthest and shortest distances in level data
	private static float head {
		get { return data[data.Count-1].Distance; }
	}
	private static float tail {
		get { return data[0].Distance; }
	}
	
	[System.Serializable]
	private class SectionData
	{
		private float dist = 0f;
		private float up = 0f;
		private float mid = 0f;
		private float down = 0f;
		
		private void Calculate()
		{
			up = Mathf.PerlinNoise(dist*topSampleRate, -0.1f);
			Rescale(ref up, 1f, 0f, maxTop, minTop);
			
			mid = Mathf.PerlinNoise(dist*midSampleRate, 0f);
			Rescale (ref mid, 1f, 0f, maxMidChange, 0f);
			
			down = Mathf.PerlinNoise(dist*botSampleRate, 0.1f);
			Rescale(ref down, 1f, 0f, maxBot, minBot);
		}
		
		private void Rescale(ref float value, float oldMax, float oldMin, float newMax, float newMin)
		{
			float oldRange = oldMax - oldMin;
			float newRange = newMax - newMin;
			value = (((value - oldMin)*newRange)/oldRange)+newMin;
		}
		
		// constructors
		public SectionData(float x)
		{
			dist = x;
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
		
	}
	
	void OnLevelWasLoaded(int level)
	{
		data.Clear();
	}
	
	private void Start()
	{
		// initialise static level data parameters
		maxMidChange = hyperSpace.maxMidChange;
		midSampleRate = hyperSpace.midSampleRate;
		maxBot = hyperSpace.maxBot;
		minBot = hyperSpace.minBot;
		botSampleRate = hyperSpace.botSampleRate;
		maxTop = hyperSpace.maxTop;
		minTop = hyperSpace.minTop;
		topSampleRate = hyperSpace.topSampleRate;
		
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
		while (head < player.transform.position.x + levelLength)
		{
			GenerateDataAtHead();
		}
	}
	
	private void FixedUpdate()
	{
		// level parameter floats must be static for StageData objects
		// they're private because public statics are not easily exposed to the Unity inspector
		// public floats are exposed in the Unity inspector (useful)
		// this is dirty but easy getaround for realtime updates to level generator parameters
		maxMidChange = hyperSpace.maxMidChange;
		midSampleRate = hyperSpace.midSampleRate;
		maxBot = hyperSpace.maxBot;
		minBot = hyperSpace.minBot;
		botSampleRate = hyperSpace.botSampleRate;
		maxTop = hyperSpace.maxTop;
		minTop = hyperSpace.minTop;
		topSampleRate = hyperSpace.topSampleRate;
		
		bool updateMesh = false;
		
		// keep generating stuff ahead of the player
		if (head < player.transform.position.x + levelLength)
		{
			GenerateDataAtHead();
			updateMesh = true;
		}
		
		// forget old level data
		if (tail < player.transform.position.x - levelLength)
		{
			DestroyDataAtTail();
			updateMesh = true;
		}
		
		if (levelDebug) DebugDrawLevel();
		
		if (updateMesh) UpdateMesh();
	}
	
	private void GenerateDataAtHead()
	{
		data.Add(new SectionData(head + 10f));
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
