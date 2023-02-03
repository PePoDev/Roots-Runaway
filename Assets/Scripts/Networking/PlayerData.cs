
public struct PlayerData
{
    public ulong ClientId { get; private set; }
    public string PlayerName { get; private set; }
    public ulong SeletedCharacterId { get; private set; }

    public PlayerData(ulong clientId, string playerName,ulong seletedCharacterId)
    {
        ClientId = clientId;
        PlayerName = playerName;
        SeletedCharacterId = seletedCharacterId;
    }
}

