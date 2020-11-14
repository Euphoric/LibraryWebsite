using System;

namespace BlazorEventsTodo.EventStorage
{
    public class AggregateChangeException : Exception
    {
        public AggregateChangeException(string message)
            : base(message)
        {

        }
    }
}
