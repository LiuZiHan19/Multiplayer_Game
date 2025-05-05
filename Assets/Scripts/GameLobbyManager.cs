using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyManager : MonoBehaviour
{
    public static GameLobbyManager Instance { get; private set; }

    [SerializeField] private Text debugText;

    private const string _hostAddressKey = "HostAddress";

    private GameNetworkRoomManager _networkManager;

    private Callback<LobbyCreated_t> _onLobbyCreated;
    private Callback<GameLobbyJoinRequested_t> _onGameLobbyJoinRequested;
    private Callback<LobbyEnter_t> _onLobbyEnter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SteamManager.Initialized == false)
        {
            debugText.text = "SteamManager初始化失败.";
            return;
        }

        debugText.text = "SteamManager初始化成功.";
        _networkManager = GetComponent<GameNetworkRoomManager>();
        _onLobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _onGameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        _onLobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            debugText.text = "创建大厅失败.";
            return;
        }

        debugText.text = "创建大厅成功.";
        _networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), _hostAddressKey,
            SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        debugText.text = "有玩家请求进入房间.";
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        debugText.text = "玩家进入房间.";
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), _hostAddressKey);
        _networkManager.networkAddress = hostAddress;

        if (_networkManager.isNetworkActive == false)
        {
            _networkManager.StartClient();
            debugText.text = "玩家正在链接主机.";
        }
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
    }
}