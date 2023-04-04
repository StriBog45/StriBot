// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace StriBot.Application.Configurations.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RepeatMessage
    {
        public string Message { get; set; }

        public int FrequencyInMinutes { get; set; }

        public int DelayBeforeFirstDispatchInMinutes { get; set; }
    }
}