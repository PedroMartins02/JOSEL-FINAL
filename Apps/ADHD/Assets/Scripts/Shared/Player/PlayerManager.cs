using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Data;
using GameModel;
using Unity.VisualScripting;

namespace Game.Logic
{
    public class PlayerManager
    {
        private static PlayerManager instance = null;

        public static PlayerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerManager();
                }
                return instance;
            }
        }

        private Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();

        private PlayerManager()
        {
            players = new Dictionary<ulong, Player>();
        }

        public Player CreatePlayer(ulong clientId, MP_PlayerData playerData)
        {
            DeckData deckData = DeckData.DeserializeDeckData(playerData.playerDeck.ToString());

            Deck deck = new Deck(deckData.Name, deckData.CardList);

            PlayerGameData playerGameData 
                = new PlayerGameData(
                    clientId,
                    playerData.playerUsername.ToString(), 
                    GameRulesManager.Instance.GetIntRuleValue(RuleTarget.StartingHealth),
                    GameRulesManager.Instance.GetIntRuleValue(RuleTarget.StartingBlessings),
                    (MythCard)CardDatabase.Singleton.GetCardOfId(deckData.MythId),
                    deck
                    );

            Player player = new Player(playerGameData);

            players[clientId] = player;

            return player;
        }

        public void RegisterPlayer(ulong clientId, Player player)
        {
            players.Add(clientId, player);
        }

        public Player GetPlayerByClientId(ulong clientId)
        {
            players.TryGetValue(clientId, out Player player);

            return player;
        }

        public int PlayerCount => players.Count;

        public IList<Player> PlayerList => players.Values.AsReadOnlyList();
    }
}