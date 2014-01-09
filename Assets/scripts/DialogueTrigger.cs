using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour {

	public string dialogue;
	public string character;
	public float displayTime;
	public bool showOnce = true;
	void OnTriggerEnter2D(Collider2D info)
	{
		if (info.gameObject.tag == "Player") 
		{
			GameManager.Instance.StartDialogue(character, dialogue, displayTime);
			if (showOnce) Destroy(this.gameObject);
		}
	}
}
