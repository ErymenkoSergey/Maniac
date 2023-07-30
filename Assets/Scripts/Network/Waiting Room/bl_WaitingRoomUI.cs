using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class bl_WaitingRoomUI : bl_PhotonHelper
{
    [Header("References")]
    public GameObject Content;
    public GameObject WaitingPlayerPrefab;
    public GameObject LoadingMapUI;
    public GameObject StartScreen;
    public GameObject LeaveConfirmUI;
    public GameObject waitingRequiredPlayersUI;
    public RectTransform PlayerListPanel;
    public Text RoomNameText;
    public Text MapNameText;
    public Text GameModeText;
    public Text TimeText;
    public Text GoalText;
    public Text BotsText;
    public Text FriendlyFireText;
    public Text PlayerCountText;
    public Image MapPreview;
    public List<RectTransform> PlayerListHeaders = new List<RectTransform>();
    private Player[] _players;
    public Button[] readyButtons;
    [SerializeField] private Button _changeTeamButton;
    private List<bl_WaitingPlayerUI> playerListCache = new List<bl_WaitingPlayerUI>();

    private void OnEnable()
    {
        Content.SetActive(false);
        _changeTeamButton.onClick.AddListener(ChangeTeam);
    }

    private void OnDisable()
    {
        _changeTeamButton.onClick.RemoveListener(ChangeTeam);
    }

    private void ChangeTeam()
    {
        var name = bl_Lobby.Instance.GetNickName();
        var index = 0;

        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].NickName == name)
            {
                index = i;
            }
        }

        if (_players[index].GetPlayerTeam() == Team.Hiding)
        {
            _players[index].SetPlayerTeam(Team.Maniac);
        }
        else
        {
            _players[index].SetPlayerTeam(Team.Hiding);
        }

        InstancePlayerList();
    }

    public void Show()
    {
        UpdateRoomInfoUI();
        InstancePlayerList();
        Content.SetActive(true);
        StartScreen.SetActive(true);
    }

    public void Hide()
    {
        LeaveConfirmUI.SetActive(false);
        StartScreen.SetActive(false);
        Content.SetActive(false);
        bl_LobbyUI.Instance.blackScreenFader.FadeOut(0.5f);
    }

    public void InstancePlayerList()
    {
        playerListCache.ForEach(x => { if (x != null) { Destroy(x.gameObject); } });
        playerListCache.Clear();

        _players = PhotonNetwork.PlayerList;
        List<Player> secondTeam = new List<Player>();
        bool otm = isOneTeamModeUpdate;
        PlayerListHeaders.ForEach(x => x.gameObject.SetActive(!otm));

        for (int i = 0; i < _players.Length; i++)
        {
            if (otm)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (_players[i].GetPlayerTeam() != Team.All)
                    {
                        _players[i].SetPlayerTeam(Team.All);
                    }
                }
                SetPlayerToList(_players[i]);
            }
            else
            {
                if (_players[i].GetPlayerTeam() == Team.Hiding)
                {
                    SetPlayerToList(_players[i]);
                }
                else if (_players[i].GetPlayerTeam() == Team.Maniac)
                {
                    secondTeam.Add(_players[i]);
                }
            }
        }
        if (!otm)
        {
            PlayerListHeaders[1].SetAsLastSibling();

        }
        if (secondTeam.Count > 0)
        {
            for (int i = 0; i < secondTeam.Count; i++)
            {
                SetPlayerToList(secondTeam[i]);
            }
        }

        UpdatePlayerCount();
    }

    public void SetPlayerToList(Player player)
    {
        GameObject g = Instantiate(WaitingPlayerPrefab) as GameObject;
        bl_WaitingPlayerUI wp = g.GetComponent<bl_WaitingPlayerUI>();
        wp.SetInfo(player);
        g.transform.SetParent(PlayerListPanel, false);
        playerListCache.Add(wp);
    }

    public void UpdateRoomInfoUI()
    {
        GameMode mode = GetGameModeUpdated;
        Room room = PhotonNetwork.CurrentRoom;
        RoomNameText.text = room.Name.ToUpper();
        string mapName = (string)room.CustomProperties[PropertiesKeys.SceneNameKey];
        bl_GameData.SceneInfo si = bl_GameData.Instance.AllScenes.Find(x => x.RealSceneName == mapName);
        MapPreview.sprite = si.Preview;
        MapNameText.text = si.ShowName.ToUpper();
        GameModeText.text = mode.GetName().ToUpper();
        int t = (int)room.CustomProperties[PropertiesKeys.TimeRoomKey];
        TimeText.text = (t / 60).ToString().ToUpper() + ":00";
        BotsText.text = string.Format("BOTS: {0}", (bool)room.CustomProperties[PropertiesKeys.WithBotsKey] ? "ON" : "OFF");
        FriendlyFireText.text = string.Format("FRIENDLY FIRE: {0}", (bool)room.CustomProperties[PropertiesKeys.RoomFriendlyFire] ? "ON" : "OFF");
        UpdatePlayerCount();
        readyButtons[0].gameObject.SetActive(PhotonNetwork.IsMasterClient);
        readyButtons[1].gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        readyButtons[1].GetComponentInChildren<Text>().text = bl_WaitingRoom.Instance.isLocalReady ? "CANCEL" : "READY";

        GoalText.text = (string)room.CustomProperties[PropertiesKeys.RoomGoal].ToString() + " " + GetGameModeUpdated.GetModeInfo().GoalName.ToUpper();
    }

    public void UpdatePlayerCount()
    {
        int required = GetGameModeUpdated.GetGameModeInfo().RequiredPlayersToStart;
        if (required > 1)
        {
            bool allRequired = (PhotonNetwork.PlayerList.Length >= required);
            readyButtons[0].interactable = (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length >= required);
            PlayerCountText.text = string.Format("{0} OF {2} PLAYERS ({1} MAX)", PhotonNetwork.PlayerList.Length, PhotonNetwork.CurrentRoom.MaxPlayers, required);
            waitingRequiredPlayersUI?.SetActive(!allRequired);
        }
        else
        {
            readyButtons[0].interactable = true;
            waitingRequiredPlayersUI?.SetActive(false);
            PlayerCountText.text = string.Format("{0} PLAYERS ({1} MAX)", PhotonNetwork.PlayerList.Length, PhotonNetwork.CurrentRoom.MaxPlayers);
        }
    }

    public void UpdateAllPlayersStates()
    {
        playerListCache.ForEach(x => { if (x != null) x.UpdateState(); });
    }

    public void SetLocalReady()
    {
        bl_WaitingRoom.Instance.SetLocalPlayerReady();
        readyButtons[1].GetComponentInChildren<Text>().text = bl_WaitingRoom.Instance.isLocalReady ? "CANCEL" : "READY";
    }

    public void MasterStartTheGame()
    {
        bl_WaitingRoom.Instance.StartGame();
    }

    public void LeaveRoom(bool comfirmed)
    {
        if (comfirmed)
        {
            bl_LobbyUI.Instance.blackScreenFader.FadeIn(0.5f);
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            LeaveConfirmUI.SetActive(true);
        }
    }

    private static bl_WaitingRoomUI _instance;
    public static bl_WaitingRoomUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<bl_WaitingRoomUI>();
            }
            return _instance;
        }
    }
}