using UnityEngine;
using System.Collections;

public class Cockpit : MonoBehaviour {


	private TextMesh guiHyperMatter;
	private TextMesh guiSpeed;
	private TextMesh guiScore;
	private Transform guiHyperSpaceHint;
	
	// Use this for initialization
	void Start () {
		guiHyperMatter = transform.FindChild("guiHyperValue").GetComponent<TextMesh>();
		guiSpeed =       transform.FindChild("guiSpeedValue").GetComponent<TextMesh>();
		guiScore		=transform.FindChild("guiScoreValue").GetComponent<TextMesh>();
		guiHyperSpaceHint = transform.FindChild("guiHyperSpaceHint");
	}
	
	// Update is called once per frame
	void Update () {
		if ( CubeMaster.Instance.HyperJump || Player.Instance.isDead )
		{
			guiSpeed.text = "----";
			guiHyperMatter.text = "----";
		}
		else
		{
			guiSpeed.text = (Mathf.RoundToInt(100*CubeMaster.Instance.cubeSpeed)).ToString();
			guiHyperMatter.text = Player.Instance.HyperTankPercentage.ToString() + "%";
		}
		
		
		guiScore.text = ScoreBoard.CurrentScore.ToString();
		
		guiHyperSpaceHint.renderer.enabled = (Player.Instance.HyperReady && GameManager.Instance.ShowHints);
		
	}
}
