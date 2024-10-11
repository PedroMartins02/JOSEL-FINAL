namespace GameModel
{
    public interface ITurnState
    {
        void EnterState();
        void ExitState();
        void NextPhase();
    }
}