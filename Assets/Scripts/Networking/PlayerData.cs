
public struct PlayerData
{
    public ulong ClientId { get; private set; }
    public string PlayerName { get; private set; }

    public PlayerData(ulong clientId, string playerName)
    {
        ClientId = clientId;
        PlayerName = playerName;
    }
}

