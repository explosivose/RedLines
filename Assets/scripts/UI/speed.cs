using UnityEngine;
using System.Collections;

public class speed : MonoBehaviour {

	private Player player;
	private TextMesh speedText;

	// Use this for initialization
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
	
	// Update is called once per frame
	void Update () {
		if (player != null){
			//Debug.Log();
			int speedtmp = (int)Mathf.Round(player.currentSpeed*10);
			speedText.text = speedtmp.ToString();
		}

	}
}
