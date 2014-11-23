using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScores : UIelement {

	public enum Data {
		PlayerNames,
		Scores,
		CurrentScore
	}
	public Data data;
	
	protected override void OnEnable()
	{
		finalText = "";
		List<Score> scores;
		switch (data)
		{
		case Data.PlayerNames:
			scores = ScoreBoard.GetScores();
			for (int i = 0; i < scores.Count; i++)
			{
				finalText += (i+1).ToString() + ". " + scores[i].player + "\n";
			}
			break;
		case Data.Scores:
			scores = ScoreBoard.GetScores();
			for (int i = 0; i < scores.Count; i++)
			{
				finalText += scores[i].score.ToString() + "\n";
			}
			break;
		case Data.CurrentScore:
			finalText = ScoreBoard.CurrentScore.ToString();
			break;
		}
		base.OnEnable();
	}
}
