using UnityEngine;

public class HostGameSession : IGameSession
{
    public void StartGame()
    {
        Debug.Log("Host starting the game.");
        // Shuffle deck, deal cards, etc.
    }

    public void PlayCard(int cardId)
    {
        Debug.Log("Host played card: " + cardId);
        // Logic for the host to play a card
    }
}
