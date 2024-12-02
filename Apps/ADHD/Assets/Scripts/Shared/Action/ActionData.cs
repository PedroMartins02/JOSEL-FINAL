using Game.Data;
using Game.Logic.Actions;
using Unity.Netcode;

[System.Serializable]
public struct ActionData : INetworkSerializable
{
    public ActionType ActionType;

    // Common Fields
    public ulong PlayerId;

    // Specific fields for different actions
    public string CardID;
    public CardDataSnapshot CardData;
    public int CardGameID;
    public int TargetCardGameID;
    public int Damage;
    public int Heal;
    public int Blessings;
    public ulong TargetPlayerId;

    // Other information
    public int NumberOfCardsDrawn;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ActionType);
        serializer.SerializeValue(ref PlayerId);

        // Serialize fields based on ActionType
        switch (ActionType)
        {
            case ActionType.DrawCard:
                serializer.SerializeValue(ref CardData);
                break;
            case ActionType.PlayCard:
                serializer.SerializeValue(ref CardGameID);
                break;
            case ActionType.AttackCard:
                serializer.SerializeValue(ref CardGameID); // Attacker
                serializer.SerializeValue(ref TargetCardGameID); // Defender
                serializer.SerializeValue(ref Damage);
                break;
            case ActionType.AttackMyth:
                serializer.SerializeValue(ref CardGameID);
                serializer.SerializeValue(ref Damage);
                serializer.SerializeValue(ref TargetPlayerId);
                break;
                // Add cases for other action types
        }
    }
}
