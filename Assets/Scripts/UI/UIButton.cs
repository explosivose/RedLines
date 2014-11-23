using UnityEngine;
using System.Collections;

public class UIButton : UIelement {

	public enum Action {
		UIContainer,
		UIBack,
		StartGame,
		ResumeGame,
		QuitApplication,
		OpenWebsite,
		gameOverRestart,
		gameOverQuit
	}
	public Action action;
	public Transform containerPrefab;
	private Transform containerInstance;
	private Transform previousContainer;
	

	
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
		case Action.OpenWebsite:
			Application.OpenURL(Strings.website);
			break;
		 case Action.gameOverRestart:
			PlayerPrefs.SetString(Options.keyPlayerName, Options.playerName);
			ScoreBoard.SaveAndClearCurrentScore();
			UI.HideGameOver();
			GameManager.Instance.StartGame();
		 	break;
		 case Action.gameOverQuit:
			PlayerPrefs.SetString(Options.keyPlayerName, Options.playerName);
			ScoreBoard.SaveAndClearCurrentScore();
			UI.HideGameOver();
			UI.ShowMainMenu();
		 	break;
		}
	}
	
	
	void SetPreviousContainer(Transform container)
	{
		previousContainer = container;
	}
}
