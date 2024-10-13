using UnityEngine;

public class ClientGameSession : IGameSession
{
    public void StartGame()
    {
        Debug.Log("Client waiting for the host to start the game.");
        // Wait for game synchronization from the host
    }

    public void PlayCard(int cardId)
    {
        Debug.Log("Client requesting to play card: " + cardId);
        // Send request to the host to play this card
    }
}
