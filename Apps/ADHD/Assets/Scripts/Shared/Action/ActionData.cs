using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Logic.Actions;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ActionData : INetworkSerializable
{
    public ActionType ActionType;

    // Common Fields
    public ulong PlayerId;

    // Specific fields for different actions
    public string CardId;
    public ulong NetworkObjectCardId;
    public ulong TargetCardId;
    public int Damage;
    public int Heal;
    public int Blessings;

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
                serializer.SerializeValue(ref CardId);
                break;
            case ActionType.PlayCard:
                serializer.SerializeValue(ref NetworkObjectCardId);
                break;
            case ActionType.Attack:
                serializer.SerializeValue(ref NetworkObjectCardId); // Attacker
                serializer.SerializeValue(ref TargetCardId); // Defender
                serializer.SerializeValue(ref Damage);
                break;
                // Add cases for other action types
        }
    }
}
