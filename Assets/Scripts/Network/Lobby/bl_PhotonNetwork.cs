using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Replace default Hashtables with Photon hashtables
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[DefaultExecutionOrder(-1002)]
public class bl_PhotonNetwork : bl_PhotonHelper
{
    [HideInInspector] public bool hasPingKick = false;
    public bool hasAFKKick { get; set; }
    static readonly RaiseEventOptions EventsAll = new RaiseEventOptions();
    private List<PhotonEventsCallbacks> callbackList = new List<PhotonEventsCallbacks>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EventsAll.Receivers = ReceiverGroup.All;
        PhotonNetwork.NetworkingClient.EventReceived += OnEventCustom;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventCustom;
    }

    public void AddCallback(byte code, Action<Hashtable> callback)
    {
        callbackList.Add(new PhotonEventsCallbacks() { Code = code, Callback = callback });
    }

    public void RemoveCallback(Action<Hashtable> callback)
    {
        PhotonEventsCallbacks e = callbackList.Find(x => x.Callback == callback);
        if (e != null) { callbackList.Remove(e); }
    }

    public void OnEventCustom(EventData data)
    {
        if (data.CustomData == null) return;
        
        switch (data.Code)
        {
            case PropertiesKeys.KickPlayerEvent:
                OnKick();
                break;
            case PropertiesKeys.KillFeedEvent:
                bl_KillFeed.Instance?.OnMessageReceive((Hashtable)data.CustomData);
                break;
            default:
                if(callbackList.Count > 0)
                {
                    for (int i = 0; i < callbackList.Count; i++)
                    {
                        if(callbackList[i].Code == data.Code)
                        {
                            Hashtable hastTable = (Hashtable)data.CustomData;
                            callbackList[i].Callback.Invoke(hastTable);
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Send an event to be invoke in all clients subscribed to the callback
    /// Similar to PhotonView.RPC but without needed of a PhotonView
    /// </summary>
    public void SendDataOverNetwork(byte code, Hashtable data)
    {
        SendOptions so = new SendOptions();
        PhotonNetwork.RaiseEvent(code, data, EventsAll, so);
    }

    public void OnPingKick()
    {
        hasPingKick = true;
    }

    public void KickPlayer(Player p)
    {
        SendOptions so = new SendOptions();
        PhotonNetwork.RaiseEvent(PropertiesKeys.KickPlayerEvent, null, new RaiseEventOptions() 
        {
            TargetActors = new int[] { p.ActorNumber }
        }, so);
    }

    void OnKick()
    {
        if (PhotonNetwork.InRoom)
        {
            PlayerPrefs.SetInt(PropertiesKeys.KickKey, 1);
            PhotonNetwork.LeaveRoom();
        }
    }

    public static new Player LocalPlayer { get { return PhotonNetwork.LocalPlayer; } }
    public static bool IsConnected { get { return PhotonNetwork.IsConnected; } }
    public static bool IsConnectedInRoom { get { return PhotonNetwork.IsConnected && PhotonNetwork.InRoom; } }
    public static bool IsMasterClient => PhotonNetwork.IsMasterClient;

    private void OnApplicationQuit()
    {
        bl_GameData.Instance.ResetInstance();
        Resources.UnloadUnusedAssets();
    }

    public class PhotonEventsCallbacks
    {
        public byte Code;
        public Action<Hashtable> Callback;
    }

    private static bl_PhotonNetwork _instance;
    public static bl_PhotonNetwork Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<bl_PhotonNetwork>();
            return _instance;
        }
    }
}