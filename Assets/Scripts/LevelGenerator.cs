using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator 
{

	public static moving State
	{
		get { return vState; }
	}
	
	public static Vector3 CurrentPosition
	{
		get { return currentPosition; }
	}
	
	public static float MaxRadius
	{
		get { return radiusMax; }
	}
	
	private static Vector3 currentPosition = Vector3.zero;
	private static moving vState = moving.straight;
	private static moving hState = moving.straight;
	private static size xrState = size.locked;
	private static size yrState = size.locked;
	private static int ticker = 0;
	private static bool obstacles = true;
	
	// change in position states
	public enum moving
	{
		positive2,
		positive,
		straight,
		negative,
		negative2,
		locked
	}
	
	// change in radius states
	private enum size
	{
		mutate,	// +random.range(-0.25,0.25)
		target, // while(radius < target) radius+=0.25f; while(target < radius) radius-=0.25f;
		locked  // no change
	}
	
	public static void Reset()
	{
		currentPosition = Vector3.zero;
		vState = moving.straight;
		hState = moving.straight;
		xRadius = radiusMax;
		yRadius = radiusMax;
	}
	
	public static void Reset(Vector3 startPosition)
	{
		currentPosition = startPosition;
		currentPosition.z = 0f;
		vState = moving.straight;
		hState = moving.straight;
		xRadius = radiusMax;
		yRadius = radiusMax;
	}
	
	public static void LockRadius()
	{
		// prevent size changes
		xrState = size.locked;
		yrState = size.locked;
	}
	
	public static void LockRadius(float radius)
	{
		// lerp to circular shape
		xrState = size.target;
		yrState = size.target;
		yRadiusTarget = radius;
		xRadiusTarget = radius;
	}
	
	public static void LockRadius(float x, float y)
	{
		// lerp to x and y radii
		xrState = size.target;
		yrState = size.target;
		yRadiusTarget = y;
		xRadiusTarget = x;
	}
	
	public static void LockPosition()
	{
		// push position states outside the markov chain state machine
		vState = moving.locked;	// position cannot change
		hState = moving.locked;
	}
	
	public static void Unlock()
	{
		xrState = size.mutate;
		yrState = size.mutate;
		vState = moving.straight;
		hState = moving.straight;
	}
	
	public static bool Obstacles 
	{
		get { return obstacles; }
		set { obstacles = value; }
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
				tempCube.startPosition = tempCube.targetPosition + (Vector3.left * Random.Range(-20, 20));
				// additional offset is just zero here
				//tempCube.positionOffset = Vector3.zero;//Vector3.forward * Mathf.Abs(i - (total/2f)) * 0.25f;
				
				// audio beat value should be less than 1
				//float beat = Mathf.Abs(i - (total/2f));
				//Rescale(ref beat, total/2f, 0f, 0.5f, 0f);
				//tempCube.audioBeat = beat;
				
				// add metadata to list
				cubes.Add(tempCube);
			}
			
			if (i > topCubes + gap/2f && i < total - botCubes - gap/2f )
			{
				CubeMeta tempCube = new CubeMeta();
				// target position is height determined by i
				tempCube.targetPosition = currentPosition + (Vector3.up * i);
				// start offset is some Z offset
				tempCube.startPosition = tempCube.targetPosition + (Vector3.left * Random.Range(-20, 20));
				// additional offset is just zero here
				//tempCube.positionOffset = Vector3.forward * Mathf.Abs(i - (total/2f)) * 0.25f;
				
				// audio beat value should be less than 1
				//float beat = Mathf.Abs(i - (total/2f));
				//Rescale(ref beat, total/2f, 0f, 0.5f, 0f);
				//tempCube.audioBeat = 0f;
				
				// add metadata to list
				cubes.Add(tempCube);
			}
		}
		
		return cubes;
		
	}
	
	private static float xRadius = 6f;
	private static float xRadiusTarget = 0f;
	private static float yRadius = 4f;
	private static float yRadiusTarget = 0f;
	private const float radiusMax = 6f;
	private const float radiusMin = 2.5f;
	private static float spacing = 1f;
	
	public static List<CubeMeta> Generate3D(float cubeSize)
	{
		// 3D level is a tunnel formed from sequential ellipses
		// ellipses walls are the cubes
		// Here's the maths:
		// 		General Equation of an ellipse 
		// 			x^2/a^2  +   y^2/b^2  =  1
		//		where:
		//			x, y are the coordinates of any point on the ellipse
		// 			a, b are the radius on the x and y axes respectively
		//		source: http://www.mathopenref.com/coordgeneralellipse.html
		
		spacing = cubeSize;
		List<CubeMeta> cubes = new List<CubeMeta>();
		NextPosition();		// update center point
		NextSize();			// update radii
		
		bool middleSpread = false;
		if (Random.value < 0.1f) middleSpread = true;
		
		// Scan a square shape around the ellipse
		// Add a cube around the outside of the ellipse
		for (float y = -yRadius-1; y <= yRadius+1; y++)
		{
			for (float x = -xRadius-1; x <= xRadius+1; x++)
			{
				float xy = ((x*x)/(xRadius*xRadius)) + ((y*y)/(yRadius*yRadius));
				
				if ( xy > 1f && xy < 2f)
				{
					CubeMeta tempCube = new CubeMeta();
					Vector3 pos = new Vector3(x*spacing, y*spacing);
					//Debug.DrawLine(pos, pos + Vector3.left, Color.green, 1f);
					tempCube.targetPosition = currentPosition + pos;
					if (middleSpread) tempCube.startPosition = currentPosition;
					else tempCube.startPosition  = currentPosition + pos*5f;
					tempCube.layerCenter = currentPosition;
					tempCube.layerIndex = ticker;
					tempCube.startTime = Time.time;
					cubes.Add(tempCube);
				}
				//random obstacle
				else if (Random.value < 0.005f && obstacles)
				{
					CubeMeta tempCube = new CubeMeta();
					Vector3 pos = new Vector3(x*spacing, y*spacing);
					//Debug.DrawLine(pos, pos + Vector3.left, Color.green, 1f);
					tempCube.targetPosition = currentPosition + pos;
					tempCube.startPosition  = currentPosition + pos*5f;
					tempCube.layerCenter = currentPosition;
					tempCube.layerIndex = ticker;
					tempCube.startTime = Time.time;
					cubes.Add(tempCube);
				}
			}
		}
		return cubes;
	}
	
	private static void NextSize()
	{
		// decide on changes in radii
		
		switch (xrState)
		{
		default:
		case size.locked:
			break;
		case size.mutate:
			xRadius += Random.Range(-0.25f, 0.25f);
			break;
		case size.target:
			if (xRadius < xRadiusTarget) xRadius += 0.25f;
			if (xRadius > xRadiusTarget) xRadius -= 0.25f;
			break;
		}
		xRadius = Mathf.Clamp (xRadius, radiusMin, radiusMax); 
		
		switch (yrState)
		{
		default:
		case size.locked:
			break;
		case size.mutate:
			yRadius += Random.Range(-0.25f, 0.25f);
			break;
		case size.target:
			if (yRadius < yRadiusTarget) yRadius += 0.25f;
			if (yRadius > yRadiusTarget) yRadius -= 0.25f;
			break;
		}
		yRadius = Mathf.Clamp (yRadius, radiusMin, radiusMax);
	}
	
	private static void NextPosition()
	{
		ticker++;
		
		// Next positions are calculated using a Markov Chain style state machine
		// http://en.wikipedia.org/wiki/Markov_chain
		// 

		float roll = Random.value; // random float between 0 and 1
		
		
		/* Determine new state
		*/
		switch (vState)
		{
		case moving.positive2:
			if      (roll > 0.75f) vState = moving.positive;		// 25% chance
			else                   vState = moving.positive2;	// 75% chance
			break;
		
		case moving.positive:
			if      (roll > 0.75f) vState = moving.positive2;	// 25%
			else if (roll > 0.25f) vState = moving.positive;      	// 50%
			else                   vState = moving.straight; // 25%
			break;
			
		case moving.straight:
			if      (roll > 0.50f) vState = moving.straight;	// 50%
			else if (roll > 0.25f) vState = moving.positive;		// 25%
			else                   vState = moving.negative;		// 25%
			break;
			
		case moving.negative:
			if      (roll > 0.75f) vState = moving.negative2;
			else if (roll > 0.25f) vState = moving.negative;
			else                   vState = moving.straight;
			break;
			
		case moving.negative2:
			if      (roll > 0.75f) vState = moving.negative;
			else                   vState = moving.negative2;
			break;
		}
		
		switch (hState)
		{
		case moving.positive2:
			if      (roll > 0.75f) hState = moving.positive;		// 25% chance
			else                   hState = moving.positive2;	// 75% chance
			break;
			
		case moving.positive:
			if      (roll > 0.75f) hState = moving.positive2;	// 25%
			else if (roll > 0.50f) hState = moving.positive;      	// 25%
			else                   hState = moving.straight; // 50%
			break;
			
		case moving.straight:
			if      (roll > 0.25f) hState = moving.straight;	// 75%
			else if (roll > 0.125f) hState = moving.positive;		// 12.5%
			else                   hState = moving.negative;		// 12.5%
			break;
			
		case moving.negative:
			if      (roll > 0.75f) hState = moving.negative2;
			else if (roll > 0.50f) hState = moving.negative;
			else                   hState = moving.straight;
			break;
			
		case moving.negative2:
			if      (roll > 0.75f) hState = moving.negative;
			else                   hState = moving.negative2;
			break;
		}
		
		/* Determine new position based on new state
		*/
		switch (vState)
		{
		case moving.positive2:
			currentPosition += Vector3.up * 1.25f;
			break;
		
		case moving.positive:
			currentPosition += Vector3.up * 0.75f;
			break;
			
		case moving.negative:
			currentPosition += Vector3.down * 0.75f;
			break;
			
		case moving.negative2:
			currentPosition += Vector3.down * 1.25f;
			break;
		}
		
		switch (hState)
		{
		case moving.positive2:
			currentPosition += Vector3.right * 1.25f;
			break;
			
		case moving.positive:
			currentPosition += Vector3.right * 0.75f;
			break;
			
		case moving.negative:
			currentPosition += Vector3.left * 0.75f;
			break;
			
		case moving.negative2:
			currentPosition += Vector3.left * 1.25f;
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
