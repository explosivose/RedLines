using UnityEngine;
using System.Collections;

public static class Options {

	public static string playerName;
	public const string defaultPlayerName = "mingebag";
	public const string keyPlayerName = "playerName";
	public const string keyMasterAudioVolume = "audioVolume";
	public const string keyMusicVolume = "musicVolume";
	public const string keyHints = "hints";
	
	// keep a copy of settings because if Options.Save() is called
	// when application is quitting then we might not catch some objects
	// before they are destroyed.
	public static float masterVolume
	{
		get {
			//_masterVolume = AudioListener.volume;
			return _masterVolume;
		}
		set {
			_masterVolume = value;
			AudioListener.volume = value;
		}
	}
	private static float _masterVolume;
	
	public static float musicVolume
	{
		get {
			if (Camera.main) {
				_musicVolume = Camera.main.audio.volume;
				return _musicVolume;
			}

			Debug.LogWarning("Options: No Main Camera to get music volume from.");
			return 0f;
		}
		set {
			_musicVolume = value;
			if (Camera.main) 	
				Camera.main.audio.volume = value;
			else
				Debug.LogWarning("Options: No Main Camera to set music volume on.");
		}
	}
	private static float _musicVolume;
	
	public static int showHints 
	{
		get {
			if (GameManager.Instance) {
				_showHints = GameManager.Instance.hints;
				return _showHints;
			}
			else {
				return 0;
			}
		}
		set {
			if (GameManager.Instance) {
				_showHints = value;
				GameManager.Instance.hints = value;
			}
			else {
				Debug.LogWarning("Options: No GameManager to save hint settings to.");
			}
		}
	}
	private static int _showHints;
	
	public static void LoadSettings()
	{
		playerName = PlayerPrefs.GetString(keyPlayerName, defaultPlayerName);
		Strings.guiTable[(int)Strings.guiIndex.playerName] = playerName;
		musicVolume = PlayerPrefs.GetFloat(keyMusicVolume, 0.75f);
		masterVolume = PlayerPrefs.GetFloat(keyMasterAudioVolume, 0.75f);
		showHints = PlayerPrefs.GetInt(keyHints, 1);
	}
	
	public static void SaveSettings()
	{
		PlayerPrefs.SetString(keyPlayerName, playerName);
		PlayerPrefs.SetFloat(keyMusicVolume, _musicVolume);
		PlayerPrefs.SetFloat(keyMasterAudioVolume, _masterVolume);
		PlayerPrefs.SetInt(keyHints, _showHints);
		LoadSettings();
	}
	

}
