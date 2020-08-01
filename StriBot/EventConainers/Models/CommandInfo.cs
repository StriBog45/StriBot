﻿using StriBot.Bots.Enums;
using System.Collections.Generic;

namespace StriBot.EventConainers.Models
{
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

        public CommandInfo(List<string> argumentsAsList, string argumentsAsString, string commandText, string message, string displayName, string userName, Platform platform,
            bool? isVip = null, bool? isTurbo = null, bool? isSubscriber = null, bool? isModerator = null, bool? isMe = null, bool? isBroadcaster = null )
        {
            ArgumentsAsList = argumentsAsList;
            ArgumentsAsString = argumentsAsString;
            CommandText = commandText.ToLower();
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
}