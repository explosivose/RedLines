using UnityEngine;
using System.Collections;

public static class Strings {

	public const string gameTitle = "REDLINES 1000";
	public const string gameVersion = "001-dev";
	
	public const string releaseDate = "Saturday 22nd November 2014";
	
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
		guiReleaseDate 		= 0,
		guiMainMenu 		= 1,
		guiStart			= 2,
		guiPaused 			= 3,
		guiResume			= 4
		
	}
	
	public static string[] guiTable = new string[] {
		"Release Date", 	// 0
		"MAIN MENU",		// 1
		"START",			// 2
		"PAUSED",			// 3
		"RESUME"			// 4
	};
}
