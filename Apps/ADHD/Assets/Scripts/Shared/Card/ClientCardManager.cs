using System;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Logic;
using Unity.Netcode;

namespace Game.Multiplayer
{
    public class ClientCardManager : NetworkBehaviour
    {
        public Action<CardDataSnapshot> OnSnapshotRegistered;
        public Action<CardDataSnapshot, CardDataSnapshot> OnSnapshotUpdated;

        public static ClientCardManager Instance { get; private set; }

        private Dictionary<int, CardDataSnapshot> cardSnapshotDict;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            cardSnapshotDict = new();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void RegisterCardSnapshotRpc(CardDataSnapshot cardSnapshot)
        {
            cardSnapshotDict[cardSnapshot.GameID] = cardSnapshot;

            OnSnapshotRegistered?.Invoke(cardSnapshot);
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
    }
}