using GameCore.Events;
using GameModel;

namespace Game.Logic
{
    public class StunnedState : InPlayState
    {
        public override CardStateType StateType => CardStateType.Stunned;

        private readonly int stunDuration; // In Turns

        private int turnsStunned = 0;

        public StunnedState(Card card, int stunDuration) : base(card)
        {
            this.stunDuration = stunDuration;
        }

        public override void EnterState()
        {
            turnsStunned = 0;

            EventManager.Subscribe(GameEventsEnum.TurnStarted, TurnStarted);
        }

        public override void ExitState()
        {
            EventManager.Unsubscribe(GameEventsEnum.TurnStarted, TurnStarted);

            card.StateMachine.RemoveState(this);
        }

        private void TurnStarted(object _) 
        {
            turnsStunned++;

            if (turnsStunned == stunDuration)
                Cleanse();
        }

        private void Cleanse()
        {
            card.StateMachine.SetState(CardStateType.InPlay);
        }

        public override void UpdateState() { }
        public override void OnAction(CardActions action) { }
    }
}