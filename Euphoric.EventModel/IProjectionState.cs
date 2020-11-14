using System;

namespace Euphoric.EventModel
{
    public interface IProjectionState<TState>
    {
        TState State { get; }
    }
}
