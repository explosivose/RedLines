using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Score
{
	private string playerName;
	private float dist;
	private string t;
	
	public Score(float distance)
	{
		playerName = PlayerPrefs.GetString("playerName");
		dist = distance;
		t = DateTime.Now.ToString();
	}
	public Score(float distance, string name, string time)
	{
		playerName = name;
		dist = distance;
		t = time;
	}
	
	public float Distance
	{
		get { return dist; }
	}
	
	public string PlayerName
	{
		get { return playerName; }
	}
	
	public string TimeSet
	{
		get { return t; }
	}
}

public class ScoreManager : Singleton<ScoreManager> {
	
	/* ScoreManager info
	
	This class is in charge of:
		*loading and storing player scores
	
	This class is a singleton (can only be instantiated once) 	
	*/
	
	// maximum number of scores to store
	private const int numberOfScores = 8;
	
	// return a list of all the stored scores
	public List<Score> GetScores()
	{
		List<Score> scores = new List<Score>();
		
		string tempname = "";
		float tempdist = 0f;
		string temptime = "";
		
		for (int i = 0; i < numberOfScores; i++)
		{
			if (PlayerPrefs.HasKey(i + "ScoreDist"))
			{
				tempname = PlayerPrefs.GetString(i+"ScoreName");
				tempdist = PlayerPrefs.GetFloat(i+"ScoreDist");
				temptime = PlayerPrefs.GetString(i+"ScoreTime");
				scores.Add(new Score(tempdist, tempname, temptime));
			}
		}
		
		return scores;
	}
	
	// find and return the best stored score
	public Score HighScore
	{
		get
		{
			List<Score> scores = GetScores();
			if (scores.Count == 0) return new Score(0f);
			
			Score tempScore = scores[0];
			for (int i = 0; i < scores.Count; i++)
			{
				if (scores[i].Distance > tempScore.Distance )
				{
					tempScore = scores[i];
				}
			}
			return tempScore;
		}
	}
	
	// enter a new score (if it's good enough!)
	public void NewScore(float score)
	{
		List<Score> scores = GetScores();
		
		// Insert new score if it exceeds any of the stored scores
		for(int i = 0; i < scores.Count; i++)
		{
			if ( score > scores[i].Distance )
			{
				scores.Insert(i, new Score(score));
				i = scores.Count;
				scores.RemoveAt(i); 
			}
		}
		
		// Store scores
		for (int i = 0; i < scores.Count; i++)
		{
			PlayerPrefs.SetString(i+"ScoreName", scores[i].PlayerName);
			PlayerPrefs.SetFloat (i+"ScoreDist", scores[i].Distance);
			PlayerPrefs.GetString(i+"ScoreTime", scores[i].TimeSet);
		}
	}
	

}
