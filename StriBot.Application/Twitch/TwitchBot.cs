using System.Linq;
using StriBot.Application.Events;
using StriBot.Application.Events.Enums;
using StriBot.Application.Events.Models;
using StriBot.Application.Platforms.Enums;
using StriBot.Application.Twitch.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace StriBot.Application.Twitch;

public class TwitchBot
{
    private readonly ITwitchInfo _twitchInfo;
    private TwitchClient _twitchClient;

    public TwitchBot(ITwitchInfo twitchInfo)
    {
        _twitchInfo = twitchInfo;
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(_twitchInfo.Channel))
        {
            _twitchClient = new TwitchClient();
            return;
        }

        if (_twitchClient?.IsConnected == true)
        {
            EventContainer.SendMessage -= SendMessage;
            _twitchClient.Disconnect();
        }

        _twitchClient = new TwitchClient();
        _twitchClient.OnChatCommandReceived += OnChatCommandReceived;
        _twitchClient.OnWhisperCommandReceived += OnWhisperCommandReceived;
        _twitchClient.OnJoinedChannel += OnJoinedChannel;
        _twitchClient.OnNewSubscriber += OnNewSubscriber;
        _twitchClient.OnReSubscriber += OnReSubscriber;
        _twitchClient.OnGiftedSubscription += OnGiftedSubscription;
        _twitchClient.OnRaidNotification += OnRaidNotification;
        _twitchClient.OnMessageReceived += OnMessageReceived;

        var connectionCredentials = new ConnectionCredentials(_twitchInfo.BotName, 
            _twitchInfo.BotAccessToken);
        _twitchClient.Initialize(connectionCredentials, _twitchInfo.Channel);
        _twitchClient.Connect();

        EventContainer.SendMessage += SendMessage;
    }

    private void SendMessage(Platform[] platforms, string message)
    {
        if (platforms.Contains(Platform.Twitch))
            SendMessage(message);
    }

    private static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        EventContainer.Event(e.ChatMessage.IsHighlighted
            ? new PlatformEventInfo(PlatformEventType.HighlightedMessage, Platform.Twitch,
                displayName: e.ChatMessage.DisplayName, message: e.ChatMessage.Message)
            : new PlatformEventInfo(PlatformEventType.Message, Platform.Twitch,
                displayName: e.ChatMessage.DisplayName, message: e.ChatMessage.Message));
    }

    private void SendMessage(string message)
    {
        if (_twitchClient.IsConnected)
            _twitchClient.SendMessage(_twitchInfo.Channel, $"/me {message}");
        else
        {
            Reconnect();

            if (_twitchClient.IsConnected)
                _twitchClient.SendMessage(_twitchInfo.Channel, $"/me {message}");
        }
    }

    public bool IsConnected()
        => _twitchClient.IsConnected;

    public void Reconnect()
    {
        _twitchClient.Disconnect();
        _twitchClient.Reconnect();
    }

    /// <summary>
    /// e.GiftedSubscription.DisplayName - кто подарил "Добро пожаловать OrloffNY"
    /// e.GiftedSubscription.MsgParamRecipientUserName - кому подарили "Добро пожаловать syndicatereara!"
    /// </summary>
    private static void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        => EventContainer.Event(new PlatformEventInfo(PlatformEventType.GiftSubscription, Platform.Twitch,
            displayName: e.GiftedSubscription.DisplayName,
            secondName: e.GiftedSubscription.MsgParamRecipientUserName));

    private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        => SendMessage("Бот успешно подключился!");

    private static void OnRaidNotification(object sender, OnRaidNotificationArgs e)
        => EventContainer.Event(new PlatformEventInfo(PlatformEventType.Raid, Platform.Twitch, e.RaidNotification.DisplayName));

    private static void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        => EventContainer.Event(new PlatformEventInfo(PlatformEventType.NewSubscription, Platform.Twitch, e.Subscriber.DisplayName));

    private static void OnReSubscriber(object sender, OnReSubscriberArgs e)
        => EventContainer.Event(new PlatformEventInfo(PlatformEventType.NewSubscription, Platform.Twitch, e.ReSubscriber.DisplayName));

    private static void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        EventContainer.CreateEventCommandCall(new CommandInfo(
            Platform.Twitch,
            e.Command.ArgumentsAsList, 
            e.Command.ArgumentsAsString, 
            e.Command.CommandText, 
            e.Command.ChatMessage.Message, 
            e.Command.ChatMessage.DisplayName, 
            e.Command.ChatMessage.Username,
            e.Command.ChatMessage.IsVip, 
            e.Command.ChatMessage.IsTurbo, 
            e.Command.ChatMessage.IsSubscriber, 
            e.Command.ChatMessage.IsModerator, 
            e.Command.ChatMessage.IsMe, 
            e.Command.ChatMessage.IsBroadcaster));
    }

    private static void OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
    {
        //TODO Определение роли пользователя для канала
        EventContainer.CreateEventCommandCall(new CommandInfo(
            Platform.Twitch,
            e.Command.ArgumentsAsList,
            e.Command.ArgumentsAsString,
            e.Command.CommandText,
            e.Command.WhisperMessage.Message,
            e.Command.WhisperMessage.DisplayName,
            e.Command.WhisperMessage.Username,
            isTurbo: e.Command.WhisperMessage.IsTurbo));
    }
}