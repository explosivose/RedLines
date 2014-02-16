using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour 
{

	public bool generating = true;
	public float iterationTime = 0.1f;
	public int minimumGap = 8;
	public int minWallWidth = 4;
	public int maxWallWidth = 8;
	
	private moving state = moving.straight;
	private Vector3 currentPosition = Vector3.zero;
	private CubeMaster master;
	
	private enum moving
	{
		upSteep,
		up,
		straight,
		down,
		downSteep
	}
	
	void Start () 
	{
		master = GameObject.FindWithTag("CubeMaster").GetComponent<CubeMaster>();
		currentPosition = transform.position;
		StartCoroutine( Generate() );
		StartCoroutine( ChangeStuff() );
	}
	

	IEnumerator ChangeStuff()
	{
		while (generating)
		{
			yield return new WaitForSeconds(2f);
			minimumGap += Random.Range(-2,2);
			minimumGap = Mathf.Clamp(minimumGap, 3, 8);
		}
	}
	
	
	IEnumerator Generate()
	{	
		while (generating)
		{
			// Set the next position
			NextPosition();
			
			// Create list of meta data for cube spawning
			List<CubeMeta> cubes = new List<CubeMeta>();
			
			// number of cubes (and gaps) in this column
			int total = (maxWallWidth * 2) + minimumGap;
			// indexes of cubes on the top
			int topCubes = Random.Range(minWallWidth, maxWallWidth);
			// indexes of cubes on the bottom
			int botCubes = total - Random.Range(minWallWidth, maxWallWidth);
			
			for (int i = 1; i < total; i++)
			{
				/*	i < topCubes
				*	gaps (no cubes)
				*	i > bot cubes
				*/
				if (i < topCubes || i > botCubes)
				{
					CubeMeta tempCube = new CubeMeta();
					// target position is height determined by i
					tempCube.targetPosition = currentPosition + (Vector3.up * i);
					// start offset is some Z offset
					tempCube.startPosition = tempCube.targetPosition + (Vector3.forward * Random.Range(-20, 20));
					// additional offset is just zero here
					tempCube.positionOffset = Vector3.zero;
					
					// audio beat value should be less than 1
					float beat = Mathf.Abs(i - (total/2f));
					Rescale(ref beat, total/2f, 0f, 0.5f, 0f);
					tempCube.audioBeat = 0f;
					
					// add metadata to list
					cubes.Add(tempCube);
				}
			}
			
			// Spawn column of cubes
			master.LineMaker(cubes);
			
			// wait for iteration time
			yield return new WaitForSeconds(iterationTime);
		}
	}
	
	private void NextPosition()
	{
		// Next positions are calculated using a Markov Chain style state machine
		// http://en.wikipedia.org/wiki/Markov_chain
		// 

		float roll = Random.value; // random float between 0 and 1
		
		// Determine new state
		switch (state)
		{
		case moving.upSteep:
			if      (roll > 0.75f) state = moving.up;		// 25% chance
			else                   state = moving.upSteep;	// 75% chance
			break;
		
		case moving.up:
			if      (roll > 0.75f) state = moving.upSteep;	// 25%
			else if (roll > 0.25f) state = moving.up;      	// 50%
			else                   state = moving.straight; // 25%
			break;
			
		case moving.straight:
			if      (roll > 0.50f) state = moving.straight;	// 50%
			else if (roll > 0.25f) state = moving.up;		// 25%
			else                   state = moving.down;		// 25%
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
		
		// Determine new position based on state
		switch (state)
		{
		case moving.upSteep:
			currentPosition += Vector3.up * 1.25f;
			break;
		
		case moving.up:
			currentPosition += Vector3.up * 0.75f;
			break;
			
		case moving.down:
			currentPosition += Vector3.down * 0.75f;
			break;
			
		case moving.downSteep:
			currentPosition += Vector3.down * 1.25f;
			break;
		}
	}
	
	// Rescale a value from old range to new range
	// i.e. 5 is between 1 (min) and 10 (max)
	// Rescale(ref 5, 1, 10, 30, 50) --> 40
	private void Rescale(ref float value, float oldMax, float oldMin, float newMax, float newMin)
	{
		float oldRange = oldMax - oldMin;
		float newRange = newMax - newMin;
		value = (((value - oldMin)*newRange)/oldRange)+newMin;
	}
}
