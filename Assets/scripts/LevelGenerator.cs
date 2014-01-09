using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour 
{
	public Level[] levels = new Level[0];


	private Vector3 head = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		head = transform.position;
		
		
		int pce = 0;
		// Loop through each Level
		for (int lvl = 0; lvl < levels.Length; lvl++) 
		{
			levels[lvl].LoadSections();
			
			// spawn level transition trigger
			Instantiate(levels[lvl].Trigger, head, Quaternion.identity);
			
			// Loop through level sections (start, middle, end)
			for (int stn = 0; stn < levels[lvl].sections.Length; stn++)
			{
				LevelSection section = levels[lvl].sections[stn];
				for (int spawnCount = 0; spawnCount < section.numberOfPiecesToSpawn; spawnCount++)
				{
					// choose a random piece prefab to spawn
					pce = Random.Range(0, section.Pieces.Length);
					Transform piece = section.Pieces[pce];
					
					// rotate yes/no?
					Quaternion rotation = Quaternion.identity;
					float r = Random.value;
					if (r > 0.5f)
						rotation = Quaternion.Euler(180f, 0f, 0f);
					
					// spawn level piece
					for (int i = 0; i < Random.Range(1,4); i++)
					{
						Vector3 offset = Vector3.forward * i * 10;
						piece = Instantiate(piece, head + offset, rotation) as Transform;
						piece.parent = transform;
					}
					
					// move head to the end
					Vector3 tail = piece.FindChild("Tail").position - piece.position;
					head += tail;
				}
			}
			
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
