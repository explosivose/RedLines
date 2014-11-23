using UnityEngine;
using System.Collections;

public class UITextField : UIelement {

	public int maxCharacters;
	
	private bool fillin = false;
	private bool newName = false;

	
	protected override void OnMouseUpAsButton ()
	{
		base.OnMouseUpAsButton ();
		fillin = true;
		newName = false;
	}
	
	protected override void Update ()
	{
		base.Update ();
		if (!fillin) {
			text = Options.playerName;
			if (text == "") text = Options.defaultPlayerName;
			return;
		}
		if (Input.GetKey(KeyCode.Escape)) {
			fillin = false;
		}
		if (Input.GetKey(KeyCode.Delete)) {
			text = "";
		}
		foreach(char c in Input.inputString) {
			if (c == '\b') {
				if (text.Length != 0) {
					text = text.Substring(0, text.Length-1);
				}
			}
			else {
				if (c == '\n' || c == '\r') {
					fillin = false;
					newName = false;
					Options.playerName = text;
					Options.SaveSettings();
				}
				else if(text.Length < maxCharacters){
					if (!newName) {
						text = "";
						newName = true;
					}
					text += c;
				}
			}
		}
	}
}
