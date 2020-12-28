using System;

namespace DistributedCancellationExample.Console.Domain
{
    public enum Action
    {
        TurnOn,
        Hibernate,
        Sleep,
        ShutDown,
        Restart
    }

    public class Device
    {
        public Guid Id { get; set; }
    }
}