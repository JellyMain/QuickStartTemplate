namespace Infrastructure.GameStates.Interfaces
{
    public interface IExitableState : IState
    {
        public void Exit();
    }
}
