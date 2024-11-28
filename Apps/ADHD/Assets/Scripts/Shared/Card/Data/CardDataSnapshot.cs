using System.Collections;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Actions;
using GameModel;
using Unity.Netcode;
using UnityEngine;


namespace Game.Data
{
    [System.Serializable]
    public struct CardDataSnapshot : INetworkSerializable
    {
        public string Id;
        public int GameID;
        public CardType CardType;
        public int StartingBlessings;
        public int CurrentBlessings;
        public int StartingHealth;
        public int CurrentHealth;
        public int StartingAttack;
        public int CurrentAttack;
        public CardStateType CurrentState;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // TODO: Should change according to the Card Type like in the ActionData Class
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref GameID);
            serializer.SerializeValue(ref CardType);
            serializer.SerializeValue(ref StartingBlessings);
            serializer.SerializeValue(ref CurrentBlessings);
            serializer.SerializeValue(ref StartingHealth);
            serializer.SerializeValue(ref CurrentHealth);
            serializer.SerializeValue(ref StartingAttack);
            serializer.SerializeValue(ref CurrentAttack);
            serializer.SerializeValue(ref CurrentState);
        }
    }
}