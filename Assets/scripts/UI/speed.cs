using UnityEngine;
using System.Collections;

public class speed : MonoBehaviour {
	
	public bool showSpeed;
	public bool showHyperSpaceName;
	
	private Player player;
	private TextMesh speedText;

	public float speedMultiplier;

	void Start () {
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
			speedText = (TextMesh)gameObject.GetComponent(typeof(TextMesh));
		}
		else 
		{
			player = null;
			Debug.LogError("UI Error: Speed counter: Player game object not found!");
		}


	}

	void Update () {
		if (player != null){
			int speedtmp = (int)Mathf.Round(player.currentSpeed*speedMultiplier);
			speedText.color = GameManager.Instance.ColourPrimary;
			
			string text = "";
			if (showHyperSpaceName && showSpeed) 
				text = HyperSpaceMaker.CurrentHyperSpace.name + ": " + speedtmp.ToString();
			else if (showSpeed) 
				text = speedtmp.ToString();
			else if (showHyperSpaceName) 
				text = HyperSpaceMaker.CurrentHyperSpace.name;
			
			speedText.text = text;
		}

	}
}
