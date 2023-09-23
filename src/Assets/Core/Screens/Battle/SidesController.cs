namespace Assets.Core.Screens.Battle
{
    public class SidesController
    {
        public bool isPlayerStep { get; private set; }
        public ISide me => this.FromSide(this.isPlayerStep);
        public ISide enemy => this.FromSide(!this.isPlayerStep);

        public delegate void AfterEndStepArgs(ISide me, ISide enemy);
        public event AfterEndStepArgs OnAfterEndStep;
        public delegate void BeforeEndStepArgs(ISide me, ISide enemy);
        public event BeforeEndStepArgs OnBeforeEndStep;

        ISide playerSide;
        ISide enemySide;
        public SidesController(bool isPlayerFirst, ISide player, ISide enemy)
        {
            this.isPlayerStep = isPlayerFirst;
            this.playerSide = player;
            this.enemySide = enemy;
        }

        ISide FromSide(bool isPlayer)
        {
            return isPlayer ? this.playerSide : this.enemySide;
        }

        public void StartMatch()
        {
            this.playerSide.OnStartMatch(this);
            this.enemySide.OnStartMatch(this);
            this.me.OnStepBegins(this);
        }

        public void EndStep()
        {
            this.me.OnStepEnded(this);
            this.OnAfterEndStep?.Invoke(this.me, this.enemy);
            this.isPlayerStep = !this.isPlayerStep;
            this.OnBeforeEndStep?.Invoke(this.me, this.enemy);
            this.me.OnStepBegins(this);
        }
    }
    public interface ISide
    {
        void OnStartMatch(SidesController sidesController);
        void OnStepBegins(SidesController sidesController);
        void OnStepEnded(SidesController sidesController);
    }
    public interface A
    {
        void OnApplyCardToTarget(Entity.Entity Target);
    }

}