using UnityEngine;
using System.Collections;

public class LevelTransitionTrigger : MonoBehaviour {

	public float thrustIncrease;
	public Gradient newFlameGradient;
	public Color nextSunColour;
	
	void OnTriggerEnter2D(Collider2D info)
	{
		if (info.gameObject.tag == "Player") 
		{
			info.gameObject.GetComponent<Player>().LevelUp(thrustIncrease, newFlameGradient);
			GameManager.Instance.LevelUp(nextSunColour);
		}
	}
}
