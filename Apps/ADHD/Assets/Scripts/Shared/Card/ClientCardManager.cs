using System;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Logic;
using GameCore.Events;
using Unity.Netcode;

namespace Game.Multiplayer
{
    public class ClientCardManager : NetworkBehaviour
    {
        public Action<CardDataSnapshot> OnSnapshotRegistered;
        public Action<CardDataSnapshot, CardDataSnapshot> OnSnapshotUpdated;

        public static ClientCardManager Instance { get; private set; }

        private Dictionary<int, CardDataSnapshot> cardSnapshotDict;
        private Dictionary<ulong, List<int>> cardPlayerDict;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            cardSnapshotDict = new();
            cardPlayerDict = new();
        }

        private void Start()
        {
            if (IsServer)
            {
                EventManager.Subscribe(GameEventsEnum.CardStateChanged, OnCardStateChanged);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (IsServer)
            {
                EventManager.Unsubscribe(GameEventsEnum.CardStateChanged, OnCardStateChanged);
            }
        }

        private void OnCardStateChanged(object args) 
        {
            if (args.GetType() != typeof(CardStateChangedEventArgs)) return;

            CardStateChangedEventArgs stateChangedArgs = (CardStateChangedEventArgs)args;

            UpdateCardSnapshotRpc(stateChangedArgs.CardGameID);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void RegisterCardSnapshotRpc(CardDataSnapshot cardSnapshot, ulong playerID)
        {
            cardSnapshotDict[cardSnapshot.GameID] = cardSnapshot;

            OnSnapshotRegistered?.Invoke(cardSnapshot);

            if (!cardPlayerDict.ContainsKey(playerID))
                cardPlayerDict[playerID] = new List<int>();

            List<int> cardIDList = cardPlayerDict[playerID];

            cardIDList.Add(cardSnapshot.GameID);
            cardPlayerDict[playerID] = cardIDList;
        }

        [Rpc(SendTo.Server)]
        public void UpdateCardSnapshotRpc(int cardGameID)
        {
            CardDataSnapshot updatedCardSnap = CardManager.Instance.GetCardSnapshot(cardGameID);

            UpdateCardSnapshotRpc(updatedCardSnap);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateCardSnapshotRpc(CardDataSnapshot cardSnapshot)
        {
            cardSnapshotDict.TryGetValue(cardSnapshot.GameID, out CardDataSnapshot previousSnapshot);

            cardSnapshotDict[cardSnapshot.GameID] = cardSnapshot;

            OnSnapshotUpdated?.Invoke(previousSnapshot, cardSnapshot);
        }

        public CardDataSnapshot GetCardSnapshotByGameId(int cardGameID)
        {
            cardSnapshotDict.TryGetValue(cardGameID, out CardDataSnapshot cardSnapshot);

            return cardSnapshot;
        }

        public bool CardBelongsToPlayer(ulong playerID, int cardGameID)
        {
            List<int> cardList = cardPlayerDict[playerID];

            return cardList != null && cardList.Contains(cardGameID);
        }
    }
}