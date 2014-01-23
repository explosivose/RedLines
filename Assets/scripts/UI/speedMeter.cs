using UnityEngine;
using System.Collections;

public class speedMeter : MonoBehaviour {

	public float speedBarFactor;

	private Player player;


	void Start () 
	{
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null) 
		{
			player = (Player)playerObj.GetComponent (typeof(Player));
		}
		else 
		{
			player = null;
			Debug.LogError("UI Error: Speed Meter: Player game object not found!");
		}
	}

	void Update () 
	{
		if (player != null)
		{
			
			int speedtmp = (int)Mathf.Round(player.currentSpeed);
			
			foreach (Transform child in transform)
			{
				int childnum;
				if (!int.TryParse(child.name, out childnum))
					Debug.LogError ("UI Error: Can't parse string to int");
				else
					if(childnum <= speedtmp/speedBarFactor)
						child.renderer.material.color = GameManager.Instance.ColourSecondary;
					else
						child.renderer.material.color = Color.black;
			}	
		}
	}


}
