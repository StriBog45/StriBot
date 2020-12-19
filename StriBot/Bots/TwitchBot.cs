﻿using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Api;
using TwitchLib.PubSub;
using TwitchLib.Client.Extensions;
using StriBot.EventConainers;
using StriBot.Bots.Enums;
using System.Linq;
using StriBot.EventConainers.Models;
using StriBot.EventConainers.Enums;

namespace StriBot.Bots
{
    public class TwitchBot
    {
        private ConnectionCredentials _connectionCredentials;
        private TwitchClient _twitchClient;
        private TwitchAPI _api;
        private TwitchInfo _twitchInfo;
        private TwitchPubSub _twitchPub;

        private bool _chatModeEnabled = false;

        public TwitchBot()
        {
            _twitchInfo = new TwitchInfo();

            _connectionCredentials = new ConnectionCredentials(_twitchInfo.BotName, _twitchInfo.AccessToken);

            _twitchClient = new TwitchClient();
            _twitchClient.Initialize(_connectionCredentials, _twitchInfo.Channel);
            _twitchClient.OnChatCommandReceived += OnChatCommandReceived;
            _twitchClient.OnJoinedChannel += OnJoinedChannel;
            _twitchClient.OnNewSubscriber += OnNewSubscriber;
            _twitchClient.OnReSubscriber += OnReSubscriber;
            _twitchClient.OnGiftedSubscription += OnGiftedSubscription;
            _twitchClient.OnRaidNotification += OnRaidNotification;
            _twitchClient.OnMessageReceived += OnMessageReceived;

            _api = new TwitchAPI();
            _api.Settings.ClientId = _twitchInfo.ClientId;
            _api.Settings.AccessToken = _twitchInfo.AccessToken;

            //twitchPub = new TwitchPubSub();
            //twitchPub.OnPubSubServiceConnected += TwitchPub_OnPubSubServiceConnected; ;
            //twitchPub.OnRewardRedeemed += OnRewardRedeemed;
            //twitchPub.OnChannelCommerceReceived += TwitchPub_OnChannelCommerceReceived;
            //twitchPub.ListenToRewards(twitchInfo.Channel);
            //twitchPub.ListenToCommerce(twitchInfo.Channel);
            //twitchPub.Connect();

            //ExampleCallsAsync();
            GlobalEventContainer.SendMessage += SendMessage;
        }

        public void Connect()
            => _twitchClient.Connect();

        public bool IsConnected()
            => _twitchClient.IsConnected;

        public void Disconnect()
            => _twitchClient.Disconnect();

        private void SendMessage(Platform[] platforms, string message)
        {
            if (platforms.Contains(Platform.Twitch))
                SendMessage(message);
        }

        private void TwitchPub_OnChannelCommerceReceived(object sender, TwitchLib.PubSub.Events.OnChannelCommerceReceivedArgs e)
            => SendMessage($"Тест: произошла коммерция {e.DisplayName} {e.ItemDescription} {e.Username} {e.PurchaseMessage}");

        private void TwitchPub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            ;
        }

        private void OnRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
            => SendMessage($"Тест: произошла награда {e.DisplayName} {e.RewardTitle} {e.RewardCost} {e.RewardPrompt} {e.RewardId}");

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsHighlighted)
                GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.HighlightedMessage, Platform.Twitch, displayName: e.ChatMessage.DisplayName, message: e.ChatMessage.Message));
        }

        public void SendMessage(string message)
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

        public void UserTimeout(string userName, TimeSpan timeSpan, string timeoutText)
            => TimeoutUserExt.TimeoutUser(_twitchClient, _twitchInfo.Channel, userName, timeSpan, timeoutText);

        public void Reconnect()
        {
            _twitchClient.Disconnect();
            _twitchClient.Reconnect();
        }

        private void ExampleCallsAsync()
        {
            //Checks subscription for a specific user and the channel specified
            Subscription subscription = _api.V5.Channels.CheckChannelSubscriptionByUserAsync(_twitchInfo.ChannelId, _twitchInfo.ChannelId).Result;

            //Return bool if channel is online/offline.
            bool isStreaming = _api.V5.Streams.BroadcasterOnlineAsync(_twitchInfo.ChannelId).Result;

            ////Gets a list of all the subscritions of the specified channel.
            var allSubscriptions = _api.V5.Channels.GetAllSubscribersAsync(_twitchInfo.ChannelId).Result;

            //Get channels a specified user follows.
            //GetUsersFollowsResponse userFollows = api.Helix.Users.GetUsersFollowsAsync(twitchInfo.ChannelId).Result;

            //Get Specified Channel Follows
            //var channelFollowers = api.V5.Channels.GetChannelFollowersAsync(twitchInfo.ChannelId).Result;

            //Update Channel Title/Game
            //await api.V5.Channels.UpdateChannelAsync("channel_id", "New stream title", "Stronghold Crusader");
            if (isStreaming)
                isStreaming = !isStreaming;
        }

        /// <summary>
        /// e.GiftedSubscription.DisplayName - кто подарил "Добро пожаловать OrloffNY"
        /// e.GiftedSubscription.MsgParamRecipientUserName - кому подарили "Добро пожаловать syndicatereara!"
        /// </summary>
        private void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.GiftSubscription, Platform.Twitch,
                displayName: e.GiftedSubscription.DisplayName,
                secondName: e.GiftedSubscription.MsgParamRecipientUserName));

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
            => SendMessage("Бот успешно подключился!");

        private void OnRaidNotification(object sender, OnRaidNotificationArgs e)
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.Raid, Platform.Twitch, displayName: e.RaidNotification.DisplayName));

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.NewSubscription, Platform.Twitch, displayName: e.Subscriber.DisplayName));

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
            => GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.NewSubscription, Platform.Twitch, displayName: e.ReSubscriber.DisplayName));

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
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

        public void SmileMode()
        {
            if (_chatModeEnabled)
            {
                _twitchClient.EmoteOnlyOff(_twitchInfo.Channel);
                _chatModeEnabled = false;
            }
            else
            {
                _twitchClient.EmoteOnlyOn(_twitchInfo.Channel);
                _chatModeEnabled = true;
            }
        }

        public void SubMode()
        {
            if (_chatModeEnabled)
            {
                _twitchClient.SubscribersOnlyOff(_twitchInfo.Channel);
                _chatModeEnabled = false;
            }
            else
            {
                _twitchClient.SubscribersOnlyOn(_twitchInfo.Channel);
                _chatModeEnabled = true;
            }
        }

        public void FollowersMode()
            => _twitchClient.FollowersOnlyOn(_twitchInfo.Channel, new TimeSpan());

        public void FollowersModeOff()
            => _twitchClient.FollowersOnlyOff(_twitchInfo.Channel);

#warning GetUptime создан, но не привязан к команде
        string GetUptime()
        {
            string userId = GetUserId(_twitchInfo.Channel);

            return string.IsNullOrEmpty(userId) ? "Offline" : _api.V5.Streams.GetUptimeAsync(userId).Result.Value.ToString(@"hh\:mm\:ss");
        }

        string GetUserId(string username)
        {
            var userList = _api.V5.Users.GetUserByNameAsync(username).Result.Matches;

            return userList[0]?.Id;
        }
    }
}