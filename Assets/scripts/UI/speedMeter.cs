using UnityEngine;
using System.Collections;

public class speedMeter : MonoBehaviour {

	private Player player;

	// Use this for initialization
	void Start () {
		GameObject playerObj = GameObject.Find("Player3D");
		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
		}
		else 
		{
			player = null;
			Debug.Log("UI Error: Speed Meter: Player game object not found!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null){
			
			int speedtmp = (int)Mathf.Round(player.currentSpeed);
			
			foreach (Transform child in transform)
			{
				int childnum;
				if (!int.TryParse(child.name, out childnum))
					Debug.Log ("UI Error: Can't parse string to int");
				else
					if(childnum <= speedtmp/10f)
					{
						child.renderer.material.color = new Color(0.5f,1,1);
					}
					else
						child.renderer.material.color = Color.black;


				//child is your child transform
			}
			
		}
		

	}
}
