using UnityEngine;
using System.Collections;

public static class Strings {

	public const string gameTitle = "REDLINES 1K";
	public const string gameVersion = "v002-a";
	
	public const string releaseDate = "Monday 1st December 2014";
	
	public const string website = "http://supercore.co.uk";
	
	public const string creators =
		"Creators:\n" +
		"Matt Blickem - Master of the Universe\n" +
		"Sami Tanbouz - Fellow Developer\n" +
		"Dan Cohen - Crafter of the Third Dimension\n";
		
	public const string worksUsed = 
		"Creative Commons Contributors:\n" +
		"Michel Baradari - SFX\n" +
		"Chris Zabriskie - Music\n" +
		"Ten by Twenty - Fonts\n";
		
	public const string acknowledgements = 
		"David Read - Everyone knows a \"Dave\"\n" +
		"Kathy Zalecka - Blew it\n" +
		"Ben Wilson - Cool Dude\n" +
		"Luke Walker - What a guy\n";
	
	public static string guiReleaseDate = "Release Date";
	public static string guiMainMenu = "MAIN MENU";
	public static string guiStart = "START";
	public static string guiPaused = "PAUSED";
	public static string guiResume = "RESUME";
	public static string guiScoreboard = "SCOREBOARD";
	public static string guiOptions = "OPTIONS";
	public static string guiCredits = "CREDITS";
	public static string guiQuit = "QUIT";
	public static string guiGameOver = "HYPERDUMP!";
	public static string guiPilotName = "PILOTNAME";
	public static string guiScore = "SCORE";
	public static string guiAgain = "AGAIN";
	public static string guiWipeScores = "WIPE SCORES";
	public static string guiMasterVolume = "MASTER VOLUME";
	public static string guiMusicVolume = "MUSIC VOLUME";
	public static string guiGameHints = "GAME HINTS";
	
	public enum guiIndex {
		gameTitle,
		gameVersion,
		guiReleaseDate,
		releaseDate,
		website,
		creators,
		worksUsed,
		acknowledgements,
		playerName,
		guiMainMenu,
		guiStart,
		guiPaused,
		guiResume,
		guiScoreboard,
		guiOptions,
		guiCredits,			
		guiQuit,
		guiGameOver,
		guiPilotName,
		guiScore,
		guiAgain,
		guiWipeScores,
		guiMasterVolume,
		guiMusicVolume,
		guiGameHints
		
	}
	
	public static string[] guiTable = new string[] {
		gameTitle,
		gameVersion,
		guiReleaseDate,
		releaseDate,
		website,
		creators,
		worksUsed,
		acknowledgements,
		Options.defaultPlayerName,
		guiMainMenu,
		guiStart,
		guiPaused,
		guiResume,
		guiScoreboard,
		guiOptions,
		guiCredits,			
		guiQuit,
		guiGameOver,
		guiPilotName,
		guiScore,
		guiAgain,
		guiWipeScores,
		guiMasterVolume,
		guiMusicVolume,
		guiGameHints		
	};
}
