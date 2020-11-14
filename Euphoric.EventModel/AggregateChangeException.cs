using System;

namespace Euphoric.EventModel
{
    public class AggregateChangeException : Exception
    {
        public AggregateChangeException(string message)
            : base(message)
        {

        }
    }
}
