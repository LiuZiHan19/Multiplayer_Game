using Mirror;

public class GameNetworkRoomManager : NetworkRoomManager
{
    public static GameNetworkRoomManager Instance { get; private set; }

    public override void Awake()
    {
        base.Awake();
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
}