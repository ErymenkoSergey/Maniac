using Photon.Pun;
using Photon.Realtime;
using MFPS.GameModes.TeamDeathMatch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bl_Maniac : bl_PhotonHelper, IGameMode
{
    [SerializeField] private float _timeCheckWinner = 10f;
    [SerializeField] private AudioClip _audioClipFinish;
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (bl_GameData.Instance.singleMode)
            return;

        Initialize();
    }

    public bool isLocalPlayerWinner
    {
        get
        {
            return GetWinnerTeam() == PhotonNetwork.LocalPlayer.GetPlayerTeam();
        }
    }

    public Team GetWinnerTeam()
    {
        int team1 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Hiding);
        int team2 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Maniac);

        Team winner = Team.None;
        if (team1 > team2)
        { winner = Team.Hiding; }
        else if (team1 < team2)
        { winner = Team.Maniac; }
        else
        { winner = Team.None; }
        return
            winner;
    }

    [SerializeField] private List<Player> _teamOne = new List<Player>();
    [SerializeField] private List<Player> _teamTwo = new List<Player>();

    private int SetDeathsTeamOne;
    private int SetDeathsTeamTwo;
    private int countDeathsTeam1 = 0;
    private int countDeathsTeam2 = 0;

    private bool _isLoopCheckWinner = true;
    private float _loopTime = 1.0f;

    private IEnumerator DeleyCheckWinner()
    {
        yield return new WaitForSeconds(_timeCheckWinner);
        StartCoroutine(CheckPlayerCounts());
    }

    private IEnumerator CheckPlayerCounts()
    {
        var teeamms = PhotonNetwork.PlayerList;

        for (int i = 0; i < teeamms.Length; i++)
        {
            if (teeamms[i].GetPlayerTeam() == Team.Hiding)
            {
                _teamOne.Add(teeamms[i]);
            }

            if (teeamms[i].GetPlayerTeam() == Team.Maniac)
            {
                _teamTwo.Add(teeamms[i]);
            }
        }

        SetDeathsTeamOne = _teamOne.Count;
        SetDeathsTeamTwo = _teamTwo.Count;

        while (_isLoopCheckWinner)
        {
            foreach (var player in _teamOne)
            {
                countDeathsTeam1 += player.GetDeaths();
            }

            foreach (var player in _teamTwo)
            {
                countDeathsTeam2 += player.GetDeaths();
            }

            Team winner = Team.None;
            if (SetDeathsTeamOne == countDeathsTeam1)
            {
                winner = Team.Maniac;
                SetUIWinner(winner);
            }
            if (SetDeathsTeamTwo == countDeathsTeam2)
            {
                winner = Team.Hiding;
                SetUIWinner(winner);
            }

            yield return new WaitForSeconds(_loopTime);
        }
    }

    private void SetUIWinner(Team winner)
    {
        _isLoopCheckWinner = false;
        GameOver(winner);
    }


    public void Initialize()
    {
        //check if this is the game mode of this room
        if (bl_GameManager.Instance.IsGameMode(GameMode.TDM, this))
        {
            bl_GameManager.Instance.SetGameState(MatchState.Starting);
            bl_TeamDeathMatchUI.Instance.ShowUp();
        }
        else
        {
            bl_TeamDeathMatchUI.Instance.Hide();
        }

        StartCoroutine(DeleyCheckWinner());
    }

    public void OnFinishTime(bool gameOver)
    {
        string finalText = "";
        if (!PhotonNetwork.OfflineMode && GetWinnerTeam() != Team.None)
        {
            finalText = GetWinnerTeam().GetTeamName();
        }
        else
        {
            finalText = bl_GameTexts.NoOneWonName;
        }
        bl_UIReferences.Instance.SetFinalText(finalText);
    }

    public void OnLocalPlayerDeath()
    {

    }

    public void OnLocalPlayerKill()
    {
        PhotonNetwork.CurrentRoom.SetTeamScore(PhotonNetwork.LocalPlayer.GetPlayerTeam());
    }

    public void OnLocalPoint(int points, Team teamToAddPoint)
    {
        PhotonNetwork.CurrentRoom.SetTeamScore(teamToAddPoint);
    }

    public void OnOtherPlayerEnter(Player newPlayer)
    {
    }

    public void OnOtherPlayerLeave(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PropertiesKeys.Team1Score) || propertiesThatChanged.ContainsKey(PropertiesKeys.Team2Score))
        {
            int team1 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Hiding);
            int team2 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Maniac);
            bl_TeamDeathMatchUI.Instance.SetScores(team1, team2);
        }
    }

    private void GameOver(Team winner)
    {
        string finalText = $"{winner}";
        bl_UIReferences.Instance.SetFinalText(finalText);
        bl_MatchTimeManager.Instance.FinishRound();
        _audioSource.PlayOneShot(_audioClipFinish);
    }
}


//using Photon.Pun;
//using Photon.Realtime;
//using MFPS.GameModes.TeamDeathMatch;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Threading.Tasks;

//public class bl_Maniac : bl_PhotonHelper, IGameMode
//{
//    private void Awake()
//    {
//        if (!PhotonNetwork.IsConnected)
//            return;

//        Initialize();
//    }

//    public bool isLocalPlayerWinner
//    {
//        get
//        {
//            return GetWinnerTeam() == PhotonNetwork.LocalPlayer.GetPlayerTeam();
//        }
//    }

//    public Team GetWinnerTeam()
//    {
//        int team1 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Team1);
//        int team2 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Team2);

//        Team winner = Team.None;
//        if (team1 > team2)
//        { winner = Team.Team1; }
//        else if (team1 < team2)
//        { winner = Team.Team2; }
//        else
//        { winner = Team.None; }
//        return winner;
//    }

//    [SerializeField] private List<Player> _teamOne = new List<Player>();
//    [SerializeField] private List<Player> _teamTwo = new List<Player>();

//    private int SetDeathsTeamOne;
//    private int SetDeathsTeamTwo;
//    private int countDeathsTeam1 = 0;
//    private int countDeathsTeam2 = 0;

//    private bool _isLoopCheckWinner = true;
//    private float _loopTime = 1.0f;

//    private async void DeleyCheckWinner()
//    {
//        await Task.Delay(12000);
//        //StartCoroutine(CheckPlayerCounts());
//    }

//    private IEnumerator CheckPlayerCounts()
//    {
//        var teeamms = PhotonNetwork.PlayerList;

//        for (int i = 0; i < teeamms.Length; i++)
//        {
//            if (teeamms[i].GetPlayerTeam() == Team.Team1)
//            {
//                _teamOne.Add(teeamms[i]);
//            }

//            if (teeamms[i].GetPlayerTeam() == Team.Team2)
//            {
//                _teamTwo.Add(teeamms[i]);
//            }
//        }

//        SetDeathsTeamOne = _teamOne.Count;
//        SetDeathsTeamTwo = _teamTwo.Count;

//        while (_isLoopCheckWinner)
//        {
//            foreach (var player in _teamOne)
//            {
//                countDeathsTeam1 += player.GetDeaths();
//            }

//            foreach (var player in _teamTwo)
//            {
//                countDeathsTeam2 += player.GetDeaths();
//            }

//            Team winner = Team.None;
//            if (SetDeathsTeamOne == countDeathsTeam1)
//            {
//                winner = Team.Team2;
//                SetUIWinner(winner);
//            }
//            if (SetDeathsTeamTwo == countDeathsTeam2)
//            {
//                winner = Team.Team1;
//                SetUIWinner(winner);
//            }

//            yield return new WaitForSeconds(_loopTime);
//            //ClearTeams();
//        }
//    }

//    private void ClearTeams()
//    {
//        _teamOne.Clear(); 
//        _teamTwo.Clear();
//    }


//    private void SetUIWinner(Team winner)
//    {
//        _isLoopCheckWinner = false;
//        GameOver(winner);
//    }


//    public void Initialize()
//    {
//        //check if this is the game mode of this room
//        if (bl_GameManager.Instance.IsGameMode(GameMode.TDM, this))
//        {
//            bl_GameManager.Instance.SetGameState(MatchState.Starting);
//            bl_TeamDeathMatchUI.Instance.ShowUp();
//        }
//        else
//        {
//            bl_TeamDeathMatchUI.Instance.Hide();
//        }

//        DeleyCheckWinner();
//    }

//    public void OnFinishTime(bool gameOver)
//    {
//        //determine the winner
//        string finalText = "";
//        if (!PhotonNetwork.OfflineMode && GetWinnerTeam() != Team.None)
//        {
//            finalText = GetWinnerTeam().GetTeamName();
//        }
//        else
//        {
//            finalText = bl_GameTexts.NoOneWonName;
//        }
//        bl_UIReferences.Instance.SetFinalText(finalText);
//    }

//    public void OnLocalPlayerDeath()
//    {
//        //? check count Players two Teams..
//    }

//    public void OnLocalPlayerKill()
//    {
//        PhotonNetwork.CurrentRoom.SetTeamScore(PhotonNetwork.LocalPlayer.GetPlayerTeam());
//    }

//    public void OnLocalPoint(int points, Team teamToAddPoint)
//    {
//        PhotonNetwork.CurrentRoom.SetTeamScore(teamToAddPoint);
//    }

//    public void OnOtherPlayerEnter(Player newPlayer)
//    {
//    }

//    public void OnOtherPlayerLeave(Player otherPlayer)
//    {
//    }

//    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
//    {
//        if (propertiesThatChanged.ContainsKey(PropertiesKeys.Team1Score) || propertiesThatChanged.ContainsKey(PropertiesKeys.Team2Score))
//        {
//            int team1 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Team1);
//            int team2 = PhotonNetwork.CurrentRoom.GetRoomScore(Team.Team2);
//            bl_TeamDeathMatchUI.Instance.SetScores(team1, team2);
//            CheckScores(team1, team2);
//        }
//    }

//    private void CheckScores(int team1, int team2)
//    {
//        if (PhotonNetwork.OfflineMode || !bl_RoomSettings.Instance.RoomInfoFetched)
//            return;
//        //check if any of the team reach the max kills
//        if (team1 >= bl_RoomSettings.Instance.GameGoal)
//        {
//            //GameOver();
//            return;
//        }
//        if (team2 >= bl_RoomSettings.Instance.GameGoal)
//        {
//            //GameOver();
//        }
//    }


//    private void GameOver(Team winner)
//    {
//        string finalText = $"{winner}";
//        bl_UIReferences.Instance.SetFinalText(finalText);
//        bl_MatchTimeManager.Instance.FinishRound();
//    }
//}
