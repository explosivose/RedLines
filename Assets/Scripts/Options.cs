using UnityEngine;
using System.Collections;

public static class Options {

	public static string playerName;
	public const string defaultPlayerName = "mingebag";
	
	public const string keyPlayerName = "playerName";
	public const string keyMasterAudioVolume = "audioVolume";
	public const string keyMusicVolume = "musicVolume";
	public const string keyHints = "hints";
	
	public static void LoadSettings()
	{
		playerName = PlayerPrefs.GetString(keyPlayerName, defaultPlayerName);
		Camera.main.audio.volume = PlayerPrefs.GetFloat(keyMusicVolume, 0.75f);
		AudioListener.volume = PlayerPrefs.GetFloat(keyMasterAudioVolume, 0.75f);
		GameManager.Instance.hints = PlayerPrefs.GetInt(keyHints, 1);
	}
	
	public static void SaveSettings()
	{
		PlayerPrefs.SetString(keyPlayerName, playerName);
		PlayerPrefs.SetFloat(keyMusicVolume, Camera.main.audio.volume);
		PlayerPrefs.SetFloat(keyMasterAudioVolume, AudioListener.volume);
		PlayerPrefs.SetInt(keyHints, GameManager.Instance.hints);
		LoadSettings();
	}
	

}
