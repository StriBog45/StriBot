using System;
using System.Linq;
using System.Security.Authentication;
using StriBot.Bots.Enums;
using StriBot.EventConainers;
using StriBot.EventConainers.Enums;
using StriBot.EventConainers.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace StriBot.Bots
{
    public class TwitchBot
    {
        private readonly TwitchClient _twitchClient;
        private readonly TwitchInfo _twitchInfo;
        private readonly TwitchPubSub _twitchPub;

        public TwitchBot()
        {
            _twitchInfo = new TwitchInfo();

            var connectionCredentials = new ConnectionCredentials(_twitchInfo.BotName, _twitchInfo.BotAccessToken);

            _twitchClient = new TwitchClient();
            _twitchClient.Initialize(connectionCredentials, _twitchInfo.Channel);
            _twitchClient.OnChatCommandReceived += OnChatCommandReceived;
            _twitchClient.OnWhisperCommandReceived += OnWhisperCommandReceived;
            _twitchClient.OnJoinedChannel += OnJoinedChannel;
            _twitchClient.OnNewSubscriber += OnNewSubscriber;
            _twitchClient.OnReSubscriber += OnReSubscriber;
            _twitchClient.OnGiftedSubscription += OnGiftedSubscription;
            _twitchClient.OnRaidNotification += OnRaidNotification;
            _twitchClient.OnMessageReceived += OnMessageReceived;

            _twitchPub = new TwitchPubSub();
            _twitchPub.OnPubSubServiceConnected += TwitchPub_OnPubSubServiceConnected;
            _twitchPub.OnListenResponse += TwitchPubOnOnListenResponse;
            _twitchPub.OnChannelPointsRewardRedeemed += TwitchPubOnOnChannelPointsRewardRedeemed;

            _twitchPub.ListenToChannelPoints(_twitchInfo.ChannelId);
            _twitchPub.Connect();

            GlobalEventContainer.SendMessage += SendMessage;
        }

        private static void TwitchPubOnOnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new AuthenticationException("TwitchPub OAuth failed");
        }

        private static void TwitchPubOnOnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
            => GlobalEventContainer.RewardEvent(new RewardInfo
            {
                Platform = Platform.Twitch,
                RewardMessage = e.RewardRedeemed.Redemption.UserInput,
                RewardName = e.RewardRedeemed.Redemption.Reward.Title,
                UserName = e.RewardRedeemed.Redemption.User.DisplayName,
                RewardId = e.RewardRedeemed.Redemption.Reward.Id,
                RedemptionId = e.RewardRedeemed.Redemption.Id
            });

        public void Connect()
        { 
            _twitchClient.Connect();
            _twitchPub.Connect();
        }

        public bool IsConnected()
            => _twitchClient.IsConnected;

        public void Disconnect()
            => _twitchClient.Disconnect();

        private void SendMessage(Platform[] platforms, string message)
        {
            if (platforms.Contains(Platform.Twitch))
                SendMessage(message);
        }

        private void TwitchPub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            // SendTopics accepts an oauth optionally, which is necessary for some topics
            _twitchPub.SendTopics(_twitchInfo.ChannelAccessToken);
        }

        private static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            GlobalEventContainer.Event(e.ChatMessage.IsHighlighted
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
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.GiftSubscription, Platform.Twitch,
                displayName: e.GiftedSubscription.DisplayName,
                secondName: e.GiftedSubscription.MsgParamRecipientUserName));

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
            => SendMessage("Бот успешно подключился!");

        private static void OnRaidNotification(object sender, OnRaidNotificationArgs e)
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.Raid, Platform.Twitch, e.RaidNotification.DisplayName));

        private static void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.NewSubscription, Platform.Twitch, e.Subscriber.DisplayName));

        private static void OnReSubscriber(object sender, OnReSubscriberArgs e)
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.NewSubscription, Platform.Twitch, e.ReSubscriber.DisplayName));

        private static void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            GlobalEventContainer.CreateEventCommandCall(new CommandInfo(
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
            GlobalEventContainer.CreateEventCommandCall(new CommandInfo(
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
}