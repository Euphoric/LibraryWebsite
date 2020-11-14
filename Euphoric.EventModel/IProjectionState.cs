namespace BlazorEventsTodo.EventStorage
{
    public interface IProjectionState<TState>
    {
        TState State { get; }
    }
}
