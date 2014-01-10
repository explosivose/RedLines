using UnityEngine;
using System.Collections;

public class sphere : MonoBehaviour {

	private Player player;
	
	// Use this for initialization
	void Start () {
		GameObject playerObj = GameObject.Find("Player2");
		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
		}
		else 
		{
			player = null;
			Debug.Log("UI Error: Sphere: Player game object not found!");
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null){
			int speedtmp = -1*(int)Mathf.Round(player.currentSpeed);
			transform.Rotate(transform.up,speedtmp);
		}
		
	}
}
