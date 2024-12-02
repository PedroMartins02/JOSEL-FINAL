using GameModel;
using System;
using Unity.Collections;
using Unity.Netcode;

public struct MP_PlayerData : IEquatable<MP_PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString64Bytes playerUsername;
    public FixedString64Bytes playerId;
    public FixedString512Bytes playerDeck;
    public Elements playerWeather;
    public Elements playerTime;
    public int MMR; 

    public bool Equals(MP_PlayerData other)
    {
        return clientId == other.clientId
            && playerUsername == other.playerUsername
            && playerId == other.playerId
            && playerDeck == other.playerDeck
            && MMR == other.MMR;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerUsername);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerDeck);
        serializer.SerializeValue(ref MMR);
    }
}
