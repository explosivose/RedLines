using UnityEngine;
using System.Collections;

public static class UI 
{
	public static Font font = Resources.Load<Font>("Akashi");
	public static Color fontColor;
	public static Color fontHighlightColor;
	
	public static Transform mainMenuPrefab = Resources.Load<Transform>("UI/MainMenu");
	public static Transform pauseMenuPrefab = Resources.Load<Transform>("UI/PauseMenu");
	public static Transform gameOverPrefab = Resources.Load<Transform>("UI/GameOver");

	public static int level;

	private static Transform mainMenuInstance;
	private static Transform pauseMenuInstance;
	private static Transform gameOverInstance;
	
	public static Vector3 menuPosition {
		get {
			Vector3 pos = Camera.main.transform.position;
			pos += Camera.main.transform.forward * 4f;
			return pos;
		}
	}
	public static Quaternion menuRotation {
		get {
			return Quaternion.LookRotation(
				Camera.main.transform.forward,
				Camera.main.transform.up);
		}
	}
	
	public static void HideMainMenu()
	{
		if (mainMenuInstance) GameObject.Destroy(mainMenuInstance.gameObject);
	}
	
	public static void ShowMainMenu() 
	{
		HideMainMenu();
		mainMenuInstance = GameObject.Instantiate(
			mainMenuPrefab,
			menuPosition,
			menuRotation) as Transform;
	}
	
	public static void HidePauseMenu()
	{
		if (pauseMenuInstance) GameObject.Destroy(pauseMenuInstance.gameObject);
	}
	
	public static void ShowPauseMenu()
	{
		HidePauseMenu();
		pauseMenuInstance = GameObject.Instantiate(
			pauseMenuPrefab,
			menuPosition,
			menuRotation) as Transform;
	}
	
	public static void HideGameOver()
	{
		if (gameOverInstance) GameObject.Destroy(gameOverInstance.gameObject);
	}
	
	public static void ShowGameOver()
	{
		HideGameOver();
		gameOverInstance = GameObject.Instantiate(
			gameOverPrefab,
			menuPosition,
			menuRotation) as Transform;
	}
}
