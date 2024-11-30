using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct MP_PlayerData : IEquatable<MP_PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString64Bytes playerUsername;
    public FixedString64Bytes playerId;
    public FixedString512Bytes playerDeck;

    public bool Equals(MP_PlayerData other)
    {
        return clientId == other.clientId
            && playerUsername == other.playerUsername
            && playerId == other.playerId
            && playerDeck == other.playerDeck;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerUsername);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerDeck);
    }
}
