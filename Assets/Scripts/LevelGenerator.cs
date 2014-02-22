using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator 
{

	public static moving State
	{
		get { return state; }
	}
	
	private static Vector3 currentPosition = Vector3.zero;
	private static moving state = moving.straight;
	private static int ticker = 0;
	
	public enum moving
	{
		upSteep,
		up,
		straight,
		down,
		downSteep
	}
	
	public static void Reset()
	{
		ticker = 0;
		state = moving.straight;
	}
	
	public static List<CubeMeta> Generate(int gap, int minimumWallWidth, int maximumWallWidth)
	{	
		
		// Set the next position
		NextPosition();
		
		// Create list of meta data for cube spawning
		List<CubeMeta> cubes = new List<CubeMeta>();
		
		int mid = gap - 20;
		if (mid < 0) mid = 0;
		
		// number of cubes on the top
		int topCubes = Random.Range(minimumWallWidth, maximumWallWidth);
		
		// number of cubes on the bottom
		int botCubes = Random.Range(minimumWallWidth, maximumWallWidth);
		
		// number of cubes (and gaps) in this column
		int total = topCubes + botCubes + gap + mid;
		
		for (int i = 1; i < total; i++)
		{
			/*	i < topCubes
			*	gaps (no cubes)
			*	i > bot cubes
			*/
			if (i < topCubes || i > total - botCubes)
			{
				CubeMeta tempCube = new CubeMeta();
				// target position is height determined by i
				tempCube.targetPosition = currentPosition + (Vector3.up * i);
				// start offset is some Z offset
				tempCube.startPosition = tempCube.targetPosition + (Vector3.back * Random.Range(-20, 20));
				// additional offset is just zero here
				tempCube.positionOffset = Vector3.zero;//Vector3.forward * Mathf.Abs(i - (total/2f)) * 0.25f;
				
				// audio beat value should be less than 1
				float beat = Mathf.Abs(i - (total/2f));
				Rescale(ref beat, total/2f, 0f, 0.5f, 0f);
				tempCube.audioBeat = beat;
				
				// add metadata to list
				cubes.Add(tempCube);
			}
			
			if (i > topCubes + gap/2f && i < total - botCubes - gap/2f )
			{
				CubeMeta tempCube = new CubeMeta();
				// target position is height determined by i
				tempCube.targetPosition = currentPosition + (Vector3.up * i);
				// start offset is some Z offset
				tempCube.startPosition = tempCube.targetPosition + (Vector3.back * Random.Range(-20, 20));
				// additional offset is just zero here
				tempCube.positionOffset = Vector3.forward * Mathf.Abs(i - (total/2f)) * 0.25f;
				
				// audio beat value should be less than 1
				float beat = Mathf.Abs(i - (total/2f));
				Rescale(ref beat, total/2f, 0f, 0.5f, 0f);
				tempCube.audioBeat = 0f;
				
				// add metadata to list
				cubes.Add(tempCube);
			}
		}
		
		return cubes;
		
	}
	
	private static float xRadius = 5f;
	private static float yRadius = 5f;
	private const float radiusMax = 6f;
	private const float radiusMin = 3f;
	
	public static List<CubeMeta> Generate3D()
	{
		NextPosition();
		
		List<CubeMeta> cubes = new List<CubeMeta>();
		
		// 3D level is a tunnel formed from sequential ellipses
		// ellipses walls are the cubes
		// Here's the maths:
		// 		General Equation of an ellipse 
		// 			x^2/a^2  +   y^2/b^2  =  1
		//		where:
		//			x, y are the coordinates of any point on the ellipse
		// 			a, b are the radius on the x and y axes respectively
		//		source: http://www.mathopenref.com/coordgeneralellipse.html
		
		
		xRadius += Random.Range(-1f, 0.5f);
		xRadius = Mathf.Clamp (xRadius, radiusMin, radiusMax);
		
		yRadius += Random.Range(-1f, 0.5f);
		yRadius = Mathf.Clamp (yRadius, radiusMin, radiusMax);
		
		// Scan a square shape around the ellipse
		// Add a cube around the outside of the ellipse
		for (float y = -yRadius-1; y <= yRadius+1; y++)
		{
			for (float x = -xRadius-1; x <= xRadius+1; x++)
			{
				if ( ((x*x)/(xRadius*xRadius)) + ((y*y)/(yRadius*yRadius)) > 1f )
				{
					CubeMeta tempCube = new CubeMeta();
					Vector3 pos = new Vector3(0f, y, x);
					//Debug.DrawLine(pos, pos + Vector3.left, Color.green, 1f);
					tempCube.targetPosition = currentPosition + pos;
					tempCube.startPosition  = currentPosition + pos*10f;
					tempCube.positionOffset = Vector3.zero;
					cubes.Add(tempCube);
				}
				
				else if (Random.value < 0.05f)
				{
					CubeMeta tempCube = new CubeMeta();
					Vector3 pos = new Vector3(0f, y, x);
					//Debug.DrawLine(pos, pos + Vector3.left, Color.green, 1f);
					tempCube.targetPosition = currentPosition + pos;
					tempCube.startPosition  = currentPosition + pos*10f;
					tempCube.positionOffset = Vector3.zero;
					cubes.Add(tempCube);
				}
			}
		}
		
		return cubes;
	}
	
	private static void NextPosition()
	{
		// Next positions are calculated using a Markov Chain style state machine
		// http://en.wikipedia.org/wiki/Markov_chain
		// 

		float roll = Random.value; // random float between 0 and 1
		ticker++;
		
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
	private static void Rescale(ref float value, float oldMax, float oldMin, float newMax, float newMin)
	{
		float oldRange = oldMax - oldMin;
		float newRange = newMax - newMin;
		value = (((value - oldMin)*newRange)/oldRange)+newMin;
	}
}
