using System;
using UnityEngine;

[Serializable]
public class GameModeSettings
{
    public string ModeName;
    public GameMode gameMode;
    public bool isEnabled = true;

    [Header("Settings")]
    public bool supportBots = false;
    public bool AutoTeamSelection = false;
    [Range(1, 16)] public int RequiredPlayersToStart = 1;
    public OnRoundStartedSpawn onRoundStartedSpawn = OnRoundStartedSpawn.SpawnAfterSelectTeam;
    public OnPlayerDie onPlayerDie = OnPlayerDie.SpawnAfterDelay;
    public string GoalName = "Kills";

    [Header("Options")]
    public int[] maxPlayers = new int[] { 6, 2, 4, 8 };
    public int[] GameGoalsOptions = new int[] { 50, 100, 150, 200 };
    public int[] timeLimits = new int[] { 300, 420, 480, 600, 900, 1200 };

    public string GetGoalFullName(int goalID) { return string.Format("{0} {1}", GameGoalsOptions[goalID], GoalName); }

    public string GetGoalName(int goalID)
    {
        if (GameGoalsOptions.Length <= 0) return GoalName;
        return $"{GameGoalsOptions[goalID]} {GoalName}";
    }

    public int GetGoalValue(int goalID)
    {
        if (GameGoalsOptions.Length <= 0) return 0;
        if (goalID >= GameGoalsOptions.Length) return GameGoalsOptions[GameGoalsOptions.Length - 1];

        return GameGoalsOptions[goalID];
    }

    [Serializable]
    public enum OnRoundStartedSpawn
    {
        SpawnAfterSelectTeam,
        WaitUntilRoundFinish,
    }

    [Serializable]
    public enum OnPlayerDie
    {
        SpawnAfterDelay,
        SpawnAfterRoundFinish,
    }
}