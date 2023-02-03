
public struct PlayerData
{
    public ulong ClientId { get; private set; }

    public PlayerData(ulong clientId)
    {
        ClientId = clientId;
    }
}

