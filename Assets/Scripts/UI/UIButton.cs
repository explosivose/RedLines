using UnityEngine;
using System.Collections;

public class UIButton : UIelement {

	public enum Action {
		UIContainer,
		UIBack,
		StartGame,
		ResumeGame,
		QuitApplication,
	}
	public Action action;
	public Transform containerPrefab;
	private Transform containerInstance;
	private Transform previousContainer;
	
	protected override void Awake ()
	{
		base.Awake ();
		switch (action)
		{
		case Action.UIContainer:
			// NOT SURE YET (data is in Strings.gui* format... bleh)
			break;
		case Action.UIBack:
			text = Strings.guiMainMenu;
			break;
		case Action.StartGame:
			text = Strings.guiStart;
			break;
		case Action.ResumeGame:
			text = Strings.guiResume;
			break;
		case Action.QuitApplication:
			text = Strings.guiQuit;
			break;
		}
		
	}
	
	protected override void OnDisable ()
	{
		base.OnDisable ();
		if (containerInstance) Destroy(containerInstance);
	}
	
	protected override void OnMouseUpAsButton ()
	{
		base.OnMouseUpAsButton ();
		switch (action)
		{
		case Action.UIContainer:
			// hide my parent container
			transform.parent.gameObject.SetActive(false);
			// instantiate my containerPrefab
			containerInstance = Instantiate(
				containerPrefab,
				UI.menuPosition,
				UI.menuRotation) as Transform;
			// broadcast reference to my parent container
			containerInstance.BroadcastMessage(
				"SetPreviousContainer", 
				transform.parent, 
				SendMessageOptions.DontRequireReceiver);
			break;
		case Action.UIBack:
			// destroy my parent container
			Destroy(transform.parent.gameObject);
			// unhide the previous container (if there is one)
			if (previousContainer) previousContainer.gameObject.SetActive(true);
			break;
		case Action.StartGame:
			GameManager.Instance.StartGame();
			break;
		case Action.ResumeGame:
			GameManager.Instance.UnPause();
			break;
		case Action.QuitApplication:
			GameManager.Instance.QuitGame();
			break;
		}
	}
	
	
	void SetPreviousContainer(Transform container)
	{
		previousContainer = container;
	}
}
