using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Score
{
	private string m_name;
	private float m_score;
	private string m_date;
	
	public Score(float score)
	{
		m_name = PlayerPrefs.GetString("playerName");
		m_score = score;
		m_date = DateTime.Now.ToString();
	}

	public Score(string name, float score, string date)
	{
		m_name = name;
		m_score = score;
		m_date = date;
	}
	
	public float score
	{
		get { return m_score; }
	}
	public string player
	{
		get { return m_name; }
	}
	public string date
	{
		get { return m_date; }
	}
}

public class ScoreBoard
{

	private const int numberOfScores = 10;
	
	private static bool scoresCached = false;
	private static List<Score> scores = new List<Score>();
	
	public static void NewScore(float score)
	{
		bool added = false;
		if (!scoresCached) 
			LoadScores();
			
		if (scores.Count == 0)
		{
			scores.Add(new Score(score));
			added = true;
		}
		else
		{
			// insert new score in the right position in the table;
			for (int i = 0; i < scores.Count; i++)
			{
				if ( score > scores[i].score)
				{
					scores.Insert(i, new Score(score));
					added = true;
					i = scores.Count;
				}
			}
		}
		
		// append new score if it wasn't inserted
		if (!added && scores.Count < numberOfScores)
		{
			scores.Add(new Score(score));
			added = true;
		}
		
		SaveScores();
		
	}
	
	public static List<Score> GetScores()
	{
		if (!scoresCached)
		{
			LoadScores();
		}
		return scores;
	}
	
	public static void DeleteSavedScores()
	{
		for (int i = 0; i < numberOfScores; i++)
		{
			if (PlayerPrefs.HasKey(i+"Score"))
			{
				PlayerPrefs.DeleteKey(i+"Score");
				PlayerPrefs.DeleteKey(i+"ScoreName");
				PlayerPrefs.DeleteKey(i+"ScoreDate");
			}
		}
		scoresCached = false;
	}

	private static void LoadScores()
	{
		scores.Clear();
		string tmp_name = "";
		float tmp_score = 0f;
		string tmp_date = "";
		for (int i = 0; i < numberOfScores; i++)
		{
			if (PlayerPrefs.HasKey(i+"Score"))
			{
				tmp_name = PlayerPrefs.GetString(i+"ScoreName");
				tmp_score = PlayerPrefs.GetFloat(i+"Score");
				tmp_date = PlayerPrefs.GetString(i+"ScoreDate");
				scores.Add(new Score(tmp_name, tmp_score, tmp_date));
			}
		}
		scoresCached = true;
		Debug.Log ("Loaded " + scores.Count + " saved scores.");
	}
	
	public static void SaveScores()
	{
		for (int i = 0; i < scores.Count; i++)
		{
			PlayerPrefs.SetString(i+"ScoreName", scores[i].player);
			PlayerPrefs.SetFloat(i+"Score", scores[i].score);
			PlayerPrefs.SetString(i+"ScoreDate", scores[i].date);
		}
	}
	
	
}
