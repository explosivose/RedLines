using UnityEngine;
using System.Collections;

public class UIcontainer : MonoBehaviour 
{
	void Awake () 
	{
		transform.parent = Camera.main.transform;
	}
	
	void OnEnable()
	{
		UI.level++;
	}
	
	void OnDisable()
	{
		UI.level--;
	}
}
