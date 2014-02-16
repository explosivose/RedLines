using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour 
{

	private enum moving
	{
		upSteep,
		up,
		straight,
		down,
		downSteep
	}
	
	public bool generating = true;
	public float iterationTime = 0.1f;
	public int minimumGap = 8;
	public int minWallWidth = 4;
	public int maxWallWidth = 8;
	private moving state = moving.straight;
	private Vector3 currentPosition = Vector3.zero;
	private CubeMaster master;
	
	// Use this for initialization
	void Start () 
	{
		master = GameObject.FindWithTag("CubeMaster").GetComponent<CubeMaster>();
		currentPosition = transform.position;
		StartCoroutine( Generate() );
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	IEnumerator Generate()
	{	
		while (generating)
		{
			NextPosition();
			
			List<CubeMeta> cubes = new List<CubeMeta>();
			
			int total = (maxWallWidth * 2) + minimumGap;
			int topCubes = Random.Range(minWallWidth, maxWallWidth);
			int botCubes = total - Random.Range(minWallWidth, maxWallWidth);
			
			for (int i = 1; i < total; i++)
			{
				if (i < topCubes || i > botCubes)
				{
					CubeMeta tempCube = new CubeMeta();
					tempCube.targetPosition = currentPosition + (Vector3.up * i);
					tempCube.startPosition = tempCube.targetPosition + (Vector3.forward * Random.Range(-20, 20));
					tempCube.positionOffset = Vector3.zero;
					float beat = Mathf.Abs(i - (total/2f));
					Rescale(ref beat, total/2f, 0f, 0.5f, 0f);
					tempCube.audioBeat = 0f;
					cubes.Add(tempCube);
				}
			}

			master.LineMaker(cubes);
			
			yield return new WaitForSeconds(iterationTime);
		}
	}
	
	private void NextPosition()
	{
		float roll = Random.value;
		
		// Determine new state
		switch (state)
		{
		case moving.upSteep:
			if      (roll > 0.75f) state = moving.up;
			else                   state = moving.upSteep;
			break;
		
		case moving.up:
			if      (roll > 0.75f) state = moving.upSteep;
			else if (roll > 0.25f) state = moving.up;
			else                   state = moving.straight;
			break;
			
		case moving.straight:
			if      (roll > 0.50f) state = moving.straight;
			else if (roll > 0.25f) state = moving.up;
			else                   state = moving.down;
			break;
			
		case moving.down:
			if      (roll > 0.75f) state = moving.downSteep;
			else if (roll > 0.25f) state = moving.down;
			else                   state = moving.straight;
			break;
			
		case moving.downSteep:
			if      (roll > 0.75f) state = moving.down;
			else                   state = moving.downSteep;
			break;
		}
		
		switch (state)
		{
		case moving.upSteep:
			currentPosition += Vector3.up * 1f;
			break;
		
		case moving.up:
			currentPosition += Vector3.up * 0.5f;
			break;
			
		case moving.down:
			currentPosition += Vector3.down * 0.5f;
			break;
			
		case moving.downSteep:
			currentPosition += Vector3.down * 1f;
			break;
		}
	}
	
	private void Rescale(ref float value, float oldMax, float oldMin, float newMax, float newMin)
	{
		float oldRange = oldMax - oldMin;
		float newRange = newMax - newMin;
		value = (((value - oldMin)*newRange)/oldRange)+newMin;
	}
}
