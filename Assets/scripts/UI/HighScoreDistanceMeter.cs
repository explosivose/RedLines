using UnityEngine;
using System.Collections;

public class HighScoreDistanceMeter : MonoBehaviour 
{

	private Player player;
	private float highScore;
	private TextMesh uiText;
	
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		if (player == null)
			Debug.LogError("Could not find player");
		
		uiText = GetComponent<TextMesh>();
		
		highScore = ScoreManager.Instance.HighScore.Distance;
	}
	
	void Update()
	{
		uiText.text = Mathf.RoundToInt(player.transform.position.x - highScore) + " KM";
	}
}
