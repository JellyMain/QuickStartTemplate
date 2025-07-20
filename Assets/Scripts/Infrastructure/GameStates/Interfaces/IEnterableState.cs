namespace Infrastructure.GameStates.Interfaces
{
    public interface IState
    {
    }

    public interface IEnterableState : IState
    {
        public void Enter();
    }
}
