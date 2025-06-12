using System.Collections.Generic;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Events.Models;

public class CommandInfo
{
    public List<string> ArgumentsAsList { get; }
    public string ArgumentsAsString { get; }
    public string CommandText { get; }
    public string Message { get; }
    public string DisplayName { get; }
    public string UserName { get; }
    public Platform Platform { get; }
    public bool? IsVip { get; }
    public bool? IsTurbo { get; }
    public bool? IsSubscriber { get; }
    public bool? IsModerator { get; }
    public bool? IsMe { get; }
    public bool? IsBroadcaster { get; }

    public CommandInfo(Platform platform, List<string> argumentsAsList, string argumentsAsString, string commandText, string message, string displayName, string userName,
        bool? isVip = null, bool? isTurbo = null, bool? isSubscriber = null, bool? isModerator = null, bool? isMe = null, bool? isBroadcaster = null )
    {
        ArgumentsAsList = argumentsAsList;
        ArgumentsAsString = argumentsAsString;
        CommandText = commandText;
        Message = message;
        DisplayName = displayName;
        UserName = userName;
        Platform = platform;
        IsVip = isVip;
        IsTurbo = isTurbo;
        IsSubscriber = isSubscriber;
        IsModerator = isModerator;
        IsMe = isMe;
        IsBroadcaster = isBroadcaster;
    }
}