using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using GameModel; // Or your preferred networking solution

public class HostDeckManager : IDeckManager
{
    private List<Card> _deck;

    public HostDeckManager()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    // Initialize a standard deck of cards (or customize based on your game's rules)
    private void InitializeDeck()
    {
        _deck = new List<Card>();

        /*// Example of a 52-card deck initialization
        for (int suit = 0; suit < 4; suit++) // Suits (Hearts, Diamonds, Clubs, Spades)
        {
            for (int value = 1; value <= 13; value++) // Card values (Ace to King)
            {
                _deck.Add(new Card(suit, value));
            }
        }
        */
    }

    // Shuffle the deck
    public void ShuffleDeck()
    {
        for (int i = 0; i < _deck.Count; i++)
        {
            Card temp = _deck[i];
            int randomIndex = Random.Range(i, _deck.Count);
            _deck[i] = _deck[randomIndex];
            _deck[randomIndex] = temp;
        }

        Debug.Log("Deck shuffled.");
    }


    // Draw a card from the top of the deck
    public Card DrawCard()
    {
        if (_deck.Count == 0)
        {
            Debug.LogError("Deck is empty.");
            return null;
        }

        Card drawnCard = _deck[0];
        _deck.RemoveAt(0);

        // You can also broadcast this to the clients using RPC
        BroadcastDrawnCard(drawnCard);

        return drawnCard;
    }

    // Broadcast to all clients when a card is drawn (using an RPC)
    private void BroadcastDrawnCard(Card card)
    {
        // Assuming you're using Unity's Netcode for GameObjects (NGO)
        DrawCardClientRpc(card);
    }

    [ClientRpc]
    private void DrawCardClientRpc(Card card)
    {
        // This method will run on each client to update the deck on their side
        Debug.Log("Client received drawn card: " + card);
    }

    public List<int> DealCards()
    {
        throw new System.NotImplementedException();
    }
}
