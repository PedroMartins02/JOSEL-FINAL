using Unity.Netcode;
using UnityEngine;
using Zenject;

public class MatchmakingManager : MonoBehaviour
{

    // Zenject automatically injects the container
    [Inject] private DiContainer _container;

    public void StartMatchmaking()
    {
        // Use Unity Lobby/Relay API to find or create a lobby
        // Once a lobby is created, the host will create a relay
    }

    public void OnJoinedLobby()
    {
        // Check if you are the host
        var isHost = CheckIfHost();
        MultiplayerInstaller.isHost = isHost;

        // Resolve dependencies using Zenject
        var gameSession = _container.Resolve<IGameSession>();
        gameSession.StartGame();
    }
    public bool CheckIfHost()
    {
        // In Netcode for GameObjects, the host is the player running the server
        return NetworkManager.Singleton.IsHost;
    }
}
