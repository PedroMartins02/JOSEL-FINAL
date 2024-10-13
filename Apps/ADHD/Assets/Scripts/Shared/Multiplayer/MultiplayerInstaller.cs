using Zenject;

public class MultiplayerInstaller : MonoInstaller
{
    public static bool isHost;  // This will be set based on whether the player is the host

    public override void InstallBindings()
    {
        if (isHost)
        {
            Container.Bind<IGameSession>().To<HostGameSession>().AsSingle();
            //Container.Bind<IDeckManager>().To<HostDeckManager>().AsSingle();
            //Container.Bind<IPlayerTurnManager>().To<HostTurnManager>().AsSingle();
        }
        else
        {
            Container.Bind<IGameSession>().To<ClientGameSession>().AsSingle();
            //Container.Bind<IPlayerTurnManager>().To<ClientTurnManager>().AsSingle();
        }
    }
}
