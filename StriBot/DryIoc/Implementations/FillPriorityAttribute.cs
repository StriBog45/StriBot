using System;

namespace StriBot.DryIoc.Implementations
{
    public class FillPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public FillPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}