namespace Infrastructure.GameStates.Interfaces
{
    public interface IPayloadState<in TPayload> : IState
    {
        public void Enter(TPayload payload);
    }
}
