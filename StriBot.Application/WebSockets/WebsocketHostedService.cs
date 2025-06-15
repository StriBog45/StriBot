using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Platforms.Enums;
using StriBot.Application.Twitch;
using StriBot.Application.Twitch.Interfaces;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace StriBot.Application.WebSockets;

public class WebsocketHostedService : IHostedService
    {
        private readonly ILogger<WebsocketHostedService> _logger;
        private readonly EventSubWebsocketClient _eventSubWebsocketClient;
        private readonly TwitchApiClient _twitchApiClient;
        private readonly ITwitchInfo _twitchInfo;

        public WebsocketHostedService(ILogger<WebsocketHostedService> logger,
            EventSubWebsocketClient eventSubWebsocketClient,
            TwitchApiClient twitchApiClient,
            ITwitchInfo twitchInfo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventSubWebsocketClient = eventSubWebsocketClient ?? throw new ArgumentNullException(nameof(eventSubWebsocketClient));
            _twitchApiClient = twitchApiClient;
            _twitchInfo = twitchInfo;
            _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
            _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
            _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
            _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;
            _eventSubWebsocketClient.ChannelPointsCustomRewardRedemptionAdd += EventSubWebsocketClientOnChannelPointsCustomRewardRedemptionAdd;
        }

        private static Task EventSubWebsocketClientOnChannelPointsCustomRewardRedemptionAdd(object sender, ChannelPointsCustomRewardRedemptionArgs e)
        {
            EventContainer.RewardEvent(new RewardInfo
            {
                Platform = Platform.Twitch,
                RewardMessage = e.Notification.Payload.Event.UserInput,
                RewardName = e.Notification.Payload.Event.Reward.Title,
                UserName = e.Notification.Payload.Event.UserName,
                RewardId = e.Notification.Payload.Event.Reward.Id,
                RedemptionId = e.Notification.Payload.Event.Id
            });

            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_twitchInfo.ChannelAuthorized)
            {
                await _eventSubWebsocketClient.ConnectAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.DisconnectAsync();
        }

        private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
        {
            _logger.LogInformation($"Websocket {_eventSubWebsocketClient.SessionId} connected!");

            if (!e.IsRequestedReconnect)
            {
                // subscribe to topics
                // Create and send EventSubscription
                await _twitchApiClient.SubscribeChannelPointsCustomRewardRedemptionAddAsync(_eventSubWebsocketClient.SessionId);
                // If you want to get Events for special Events you need to additionally add the AccessToken of the ChannelOwner to the request.
                // https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/
            }
        }

        private async Task OnWebsocketDisconnected(object sender, EventArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

            // Don't do this in production. You should implement a better reconnect strategy with exponential backoff
            while (!await _eventSubWebsocketClient.ReconnectAsync())
            {
                _logger.LogError("Websocket reconnect failed!");
                await Task.Delay(1000);
            }
        }

        private Task OnWebsocketReconnected(object sender, EventArgs e)
        {
            _logger.LogWarning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
            return Task.CompletedTask;
        }

        private Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
        {
            _logger.LogError(e.Exception, "Websocket {SessionId} - Error occurred!", _eventSubWebsocketClient.SessionId);
            return Task.CompletedTask;
        }
    }