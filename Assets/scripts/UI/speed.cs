using UnityEngine;
using System.Collections;

public class speed : MonoBehaviour {

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
			speedText.text = speedtmp.ToString();
			speedText.color = GameManager.Instance.ColourPrimary;
		}

	}
}
