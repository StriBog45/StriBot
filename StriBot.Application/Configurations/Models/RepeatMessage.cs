namespace StriBot.Application.Configurations.Models;

public class RepeatMessage
{
    public string Message { get; set; }

    public int FrequencyInMinutes { get; set; }

    public int DelayBeforeFirstDispatchInMinutes { get; set; }
}