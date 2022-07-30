using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Api;
using TwitchLib.PubSub;
using TwitchLib.Client.Extensions;
using StriBot.EventConainers;
using StriBot.Bots.Enums;
using System.Linq;
using StriBot.EventConainers.Models;
using StriBot.EventConainers.Enums;
using System.Net;
using System.Text;

namespace StriBot.Bots
{
    public class TwitchBot
    {
        private readonly TwitchClient _twitchClient;
        private readonly TwitchAPI _api;
        private readonly TwitchInfo _twitchInfo;
        private readonly TwitchPubSub _twitchPub;

        private bool _chatModeEnabled = false;

        public TwitchBot()
        {
            _twitchInfo = new TwitchInfo();

            var connectionCredentials = new ConnectionCredentials(_twitchInfo.BotName, _twitchInfo.BotAcessToken);

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

            _api = new TwitchAPI();
            _api.Settings.ClientId = _twitchInfo.BotClientId;
            _api.Settings.AccessToken = _twitchInfo.BotAcessToken;

            _twitchPub = new TwitchPubSub();
            _twitchPub.OnPubSubServiceConnected += TwitchPub_OnPubSubServiceConnected;
            _twitchPub.OnListenResponse += OnListenResponse;
            _twitchPub.OnRewardRedeemed += OnRewardRedeemed;
            _twitchPub.OnChannelCommerceReceived += TwitchPub_OnChannelCommerceReceived;
            _twitchPub.ListenToRewards(_twitchInfo.ChannelId);
            _twitchPub.ListenToCommerce(_twitchInfo.ChannelId);

            _twitchPub.ListenToVideoPlayback($"{{{_twitchInfo.Channel}}}");
            //_twitchPub.Connect();

            //ExampleCallsAsync();
            GlobalEventContainer.SendMessage += SendMessage;

            //GetTwitchTokens();
        }

        private void GetTwitchTokens()
        {
            // Create a request using a URL that can receive a post.
            var linkParameters = $"?client_id={_twitchInfo.BotClientId}&redirect_uri=http://localhost&response_type=code&scope=viewing_activity_read";
            WebRequest request = WebRequest.Create("https://id.twitch.tv/oauth2/authorize" + linkParameters);
            // Set the Method property of the request to POST.
            request.Method = "GET";

            // Create POST data and convert it to a byte array.
            var postData = "This is a test that posts this string to a Web server.";
            var byteArray = Encoding.UTF8.GetBytes(postData);

            //// Set the ContentType property of the WebRequest.
            //request.ContentType = "application/x-www-form-urlencoded";
            //// Set the ContentLength property of the WebRequest.
            //request.ContentLength = byteArray.Length;

            //// Get the request stream.
            //Stream dataStream = request.GetRequestStream();
            //// Write the data to the request stream.
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //// Close the Stream object.
            //dataStream.Close();

            // Get the response.
            var response = request.GetResponse();

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            //using (dataStream = response.GetResponseStream())
            //{
            //    // Open the stream using a StreamReader for easy access.
            //    StreamReader reader = new StreamReader(dataStream);
            //    // Read the content.
            //    string responseFromServer = reader.ReadToEnd();
            //    // Display the content.
            //    Console.WriteLine(responseFromServer);
            //}

            var request2 = WebRequest.Create("https://id.twitch.tv/oauth2/token" + $"?client_id={_twitchInfo.BotClientId}&client_secret={"8bmk14cxel7fd39412va5wtdky2b6p"}&code={"3Dknbxrofioasvmo625pfga4ccbs3nee"}&grant_type=authorization_code&redirect_uri=http://localhost");
            request.Method = "POST";
            var response2 = request2.GetResponse();

            // Close the response.
            response.Close();
        }

        private void OnListenResponse(object sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
        {
            //SendMessage($"Topic: {e.Topic} Success: {e.Successful} SuccessByResponse: {e.Response.Successful}  Error: {e.Response.Error} Nonce {e.Response.Nonce}");
        }

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

        private void TwitchPub_OnChannelCommerceReceived(object sender, TwitchLib.PubSub.Events.OnChannelCommerceReceivedArgs e)
            => SendMessage($"Тест: произошла коммерция {e.DisplayName} {e.ItemDescription} {e.Username} {e.PurchaseMessage}");

        private void TwitchPub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            //SendMessage("PubSub Connected");
            // SendTopics accepts an oauth optionally, which is necessary for some topics
            _twitchPub.SendTopics(_twitchInfo.BotAcessToken);
        }

        private void OnRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
        {// SendMessage($"Тест: произошла награда {e.DisplayName} {e.RewardTitle} {e.RewardCost} {e.RewardPrompt} {e.RewardId}");
        }

        private static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsHighlighted)
                GlobalEventContainer.Event(new PlatformEventInfo(PlatformEventType.HighlightedMessage, Platform.Twitch, displayName: e.ChatMessage.DisplayName, message: e.ChatMessage.Message));
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

        public void UserTimeout(string userName, TimeSpan timeSpan, string timeoutText)
            => TimeoutUserExt.TimeoutUser(_twitchClient, _twitchInfo.Channel, userName, timeSpan, timeoutText);

        public void Reconnect()
        {
            _twitchClient.Disconnect();
            _twitchClient.Reconnect();
        }

        //private void ExampleCallsAsync()
        //{
        //    //Checks subscription for a specific user and the channel specified
        //    Subscription subscription = _api.V5.Channels.CheckChannelSubscriptionByUserAsync(_twitchInfo.ChannelId, _twitchInfo.ChannelId).Result;

        //    //Return bool if channel is online/offline.
        //    bool isStreaming = _api.V5.Streams.BroadcasterOnlineAsync(_twitchInfo.ChannelId).Result;

        //    ////Gets a list of all the subscritions of the specified channel.
        //    var allSubscriptions = _api.V5.Channels.GetAllSubscribersAsync(_twitchInfo.ChannelId).Result;

        //    //Get channels a specified user follows.
        //    //GetUsersFollowsResponse userFollows = api.Helix.Users.GetUsersFollowsAsync(twitchInfo.ChannelId).Result;

        //    //Get Specified Channel Follows
        //    //var channelFollowers = api.V5.Channels.GetChannelFollowersAsync(twitchInfo.ChannelId).Result;

        //    //Update Channel Title/Game
        //    //await api.V5.Channels.UpdateChannelAsync("channel_id", "New stream title", "Stronghold Crusader");
        //    if (isStreaming)
        //        isStreaming = !isStreaming;
        //}

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
        //string GetUptime()
        //{
        //    string userId = GetUserId(_twitchInfo.Channel);

        //    return string.IsNullOrEmpty(userId) ? "Offline" : _api.V5.Streams.GetUptimeAsync(userId).Result.Value.ToString(@"hh\:mm\:ss");
        //}

        //string GetUserId(string username)
        //{
        //    var userList = _api.V5.Users.GetUserByNameAsync(username).Result.Matches;

        //    return userList[0]?.Id;
        //}
    }
}