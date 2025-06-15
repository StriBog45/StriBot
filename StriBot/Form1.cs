using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using StriBot.Application.Bot;
using StriBot.Application.Commands.Handlers;
using StriBot.Application.Commands.Handlers.Progress;
using StriBot.Application.Commands.Handlers.Raffle;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.FileManager;
using StriBot.Application.FileManager.Models;
using StriBot.Application.Localization.Implementations;
using StriBot.Application.Localization.Models;
using StriBot.Application.Platforms.Enums;
using StriBot.Application.Twitch;
using StriBot.Application.Twitch.Interfaces;

namespace StriBot;

[SupportedOSPlatform("windows")]
public partial class Form1 : Form
{
    private delegate void SafeCallDelegate();
    private delegate void SafeCallDelegateOrders(List<(string, string, int)> orders);
    private readonly MMRHandler _handlerMMR;
    private readonly OrderHandler _orderHandler;
    private readonly CurrencyBaseHandler _currencyBaseHandler;
    private readonly BetsHandler _betsHandler;
    private readonly ChatBot _chatBot;
    private readonly ProgressHandler _progressHandler;
    private readonly RememberHandler _rememberHandler;
    private readonly Currency _currency;
    private readonly SettingsFileManager _settingsFileManager;
    private readonly RaffleHandler _raffleHandler;
    private readonly IDataBase _dataBase;
    private readonly ITwitchInfo _twitchInfo;
    private readonly TwitchAuthorization _twitchAuthorization;
    private readonly TwitchApiClient _twitchApiClient;

    public Form1(MMRHandler handlerMmr,
        OrderHandler orderHandler,
        CurrencyBaseHandler currencyBaseHandler,
        BetsHandler betsHandler,
        ChatBot chatBot,
        ProgressHandler progressHandler,
        RememberHandler rememberHandler,
        Currency currency,
        SettingsFileManager settingsFileManager,
        RaffleHandler raffleHandler,
        IDataBase dataBase,
        ITwitchInfo twitchInfo,
        TwitchAuthorization twitchAuthorization,
        TwitchApiClient twitchApiClient)
    {
        _handlerMMR = handlerMmr;
        _orderHandler = orderHandler;
        _currencyBaseHandler = currencyBaseHandler;
        _betsHandler = betsHandler;
        _chatBot = chatBot;
        _progressHandler = progressHandler;
        _rememberHandler = rememberHandler;
        _currency = currency;
        _settingsFileManager = settingsFileManager;
        _raffleHandler = raffleHandler;
        _dataBase = dataBase;
        _twitchInfo = twitchInfo;
        _twitchAuthorization = twitchAuthorization;
        _twitchApiClient = twitchApiClient;

        InitializeComponent();

        _progressHandler.SetConstructorSettings(BossUpdate, DeathUpdate);
        _orderHandler.SafeCallConnector(UpdateOrderList);

        EventContainer.MessageToApplicationEvent += ShowMessage;

        Task.Run(async () =>
        {
            try
            {
                await _twitchAuthorization.RefreshTokens();
                _chatBot.Connect([Platform.Twitch]);

                // Для вызова 0-й минуты
                _chatBot.TimerTick();
            }
            catch (Exception)
            {
                // ignored
            }

            EnableButtons();
        });
    }

    private static void ShowMessage(string message, string title)
        => MessageBox.Show(message, title);

    private void Form1_Load(object sender, EventArgs e)
    {
        textBoxMMR.Text = _handlerMMR.MMR.ToString();
        comboBoxCurrency.DataSource = Currency.GetCurrencies();

        if (!string.IsNullOrEmpty(_settingsFileManager.CurrencyName))
        {
            comboBoxCurrency.SelectedItem = _settingsFileManager.CurrencyName;
            LoadCurrency();
        }
    }

    private void LoadCurrency()
    {
        if (!string.IsNullOrEmpty(comboBoxCurrency.Text))
        {
            _currency.LoadCurrency(comboBoxCurrency.Text);

            textBoxNominative.Text = _currency.Nominative;
            textBoxGenitive.Text = _currency.Genitive;
            textBoxDative.Text = _currency.Dative;
            textBoxAccusative.Text = _currency.Accusative;
            textBoxInstrumental.Text = _currency.Instrumental;
            textBoxPrepositional.Text = _currency.Prepositional;

            textBoxNominativeMultiple.Text = _currency.NominativeMultiple;
            textBoxGenitiveMultiple.Text = _currency.GenitiveMultiple;
            textBoxDativeMultiple.Text = _currency.DativeMultiple;
            textBoxAccusativeMultiple.Text = _currency.AccusativeMultiple;
            textBoxInstrumentalMultiple.Text = _currency.InstrumentalMultiple;
            textBoxPrepositionalMultiple.Text = _currency.PrepositionalMultiple;
        }
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        => OpenLink("https://www.twitch.tv/stribog45");

    private static void OpenLink(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else
            {
                throw;
            }
        }
    }

    private void timer1_Tick(object sender, EventArgs e)
        => _chatBot.TimerTick();

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        _settingsFileManager.SaveSettings(comboBoxCurrency.Text, new UserCredentials
        {
            Channel = _twitchInfo.Channel,
            ChannelId = _twitchInfo.ChannelId,
            ChannelAccessToken = _twitchInfo.ChannelAccessToken,
            ChannelRefreshToken = _twitchInfo.ChannelRefreshToken,
            ChannelExpiresIn = _twitchInfo.ChannelExpiresIn,
            BotName = _twitchInfo.BotName,
            BotAccessToken = _twitchInfo.BotAccessToken,
            BotRefreshToken = _twitchInfo.BotRefreshToken,
            BotExpiresIn = _twitchInfo.BotExpiresIn
        });

        Reporter.CreateCommands(_chatBot.Commands);
    }

    private void buttonDistribution_Click(object sender, EventArgs e)
        => _currencyBaseHandler.DistributionMoney(Convert.ToInt32(DistributionMoneyPerUser.Text), Convert.ToInt32(DistributionMaxUsers.Text), Platform.Twitch);

    private void buttonCreateOptions_Click(object sender, EventArgs e)
    {
        var options = TextBoxOptions.Text.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries);
        _betsHandler.CreateBets(options, [Platform.Twitch]);
    }

    private void buttonBetsOfManiac_Click(object sender, EventArgs e)
        => _betsHandler.CreateBets(["Количество повешанных", "0", "1", "2", "3", "4"], [Platform.Twitch]);

    private void buttonBetsOfSurvivors_Click(object sender, EventArgs e)
        => _betsHandler.CreateBets(["Количество сбежавших", "0", "1", "2", "3", "4"], [Platform.Twitch]);

    private void buttonBetsOfSurvivor_Click(object sender, EventArgs e)
        => _betsHandler.CreateBets(["Выживание стримера", "выжил", "погиб"], [Platform.Twitch]);

    private void buttonBetsDota2_Click(object sender, EventArgs e)
        => _betsHandler.CreateBets(["Победа команды", "radiant", "dire"], [Platform.Twitch]);

    private void buttonStopBets_Click(object sender, EventArgs e)
        => _betsHandler.StopBetsProcess([Platform.Twitch]);

    private void buttonSelectWinner_Click(object sender, EventArgs e)
        => _betsHandler.SetBetsWinner(Convert.ToInt32(numericUpDownWinnerSelcter.Value), [Platform.Twitch]);

    private void UpdateOrderList(List<(string, string, int)> orders)
    {

        if (listViewOrder.InvokeRequired)
        {
            var d = new SafeCallDelegateOrders(UpdateOrderList);
            textBox1.Invoke(d, orders);
        }
        else
        {
            listViewOrder.Items.Clear();
            foreach (var order in orders)
                listViewOrder.Items.Add(new ListViewItem([order.Item1, order.Item2, order.Item3.ToString()]));
        }
    }

    private void buttonOrderAccept_Click(object sender, EventArgs e)
    {
        foreach (ListViewItem selected in listViewOrder.SelectedItems)
        {
            if (selected.SubItems[0].Text.Contains("youtube"))
                webBrowser.Navigate(selected.SubItems[0].Text);
            _orderHandler.OrderRemove(selected.SubItems[0].Text, selected.SubItems[1].Text, int.Parse(selected.SubItems[2].Text));
            _dataBase.AddMoney(selected.SubItems[1].Text, -int.Parse(selected.SubItems[2].Text));
            EventContainer.Message($"Заказ @{selected.SubItems[1].Text} на {selected.SubItems[0].Text} принят", Platform.Twitch);
            listViewOrder.Items.Remove(selected);
        }
    }
    private void buttonOrderCancel_Click(object sender, EventArgs e)
    {
        foreach (ListViewItem selected in listViewOrder.SelectedItems)
        {
            _orderHandler.OrderRemove(selected.SubItems[0].Text, selected.SubItems[1].Text, int.Parse(selected.SubItems[2].Text));
            EventContainer.Message($"Заказ @{selected.SubItems[1].Text} отменен", Platform.Twitch);
            listViewOrder.Items.Remove(selected);
        }
    }
    private void buttonMMRSet_Click(object sender, EventArgs e)
        => _handlerMMR.MMR = Convert.ToInt32(textBoxMMR.Text);

    private void buttonMMRCheck_Click(object sender, EventArgs e)
        => textBoxMMR.Text = _handlerMMR.MMR.ToString();

    private void buttonWinsCheck_Click(object sender, EventArgs e)
    {
        textBox1.Text = _handlerMMR.Wins.ToString();
        textBox2.Text = _handlerMMR.Losses.ToString();
    }

    private void buttonWinsSet_Click(object sender, EventArgs e)
    {
        _handlerMMR.Wins = Convert.ToInt32(textBox1.Text);
        _handlerMMR.Losses = Convert.ToInt32(textBox2.Text);
    }

    private void BossUpdate()
    {
        var d = new SafeCallDelegate(BossUpdate);

        if (listView1.InvokeRequired)
            listView1.Invoke(d);
        else
        {
            listView1.Items.Clear();
            foreach (var boss in _progressHandler.GetBosses())
                listView1.Items.Add(new ListViewItem(boss));
        }
    }

    private void buttonBossDelete_Click(object sender, EventArgs e)
    {
        foreach (int item in listView1.SelectedIndices)
            _progressHandler.BossRemoveByIndex(item);
    }

    private void buttonDeathAdd_Click(object sender, EventArgs e)
    {
        _progressHandler.Deaths++;
        DeathUpdate();
    }

    private void buttonDeathReduce_Click(object sender, EventArgs e)
    {
        _progressHandler.Deaths--;
        DeathUpdate();
    }

    private void DeathUpdate()
    {
        var d = new SafeCallDelegate(DeathUpdate);

        if (listView1.InvokeRequired)
            listView1.Invoke(d);
        else
        {
            label1.Text = $"Смертей: {_progressHandler.Deaths}";
        }
    }

    private void listViewOrder_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if(listViewOrder.SelectedItems.Count > 0)
        //    linkLabelOrder.Text = listViewOrder.SelectedItems[0].SubItems[0].Text;
    }

    private void buttonReminderClear_Click(object sender, EventArgs e)
    {
        _rememberHandler.TextReminder = string.Empty;
        EventContainer.Message("Напоминание удалено", Platform.Twitch);
    }

    private void buttonSmileMode_Click(object sender, EventArgs e)
    {
        //chatBot.SmileMode();
    }

    private void SubMode_Click(object sender, EventArgs e)
    {
        //chatBot.SubMode();
    }

    private void buttonFollowMode_Click(object sender, EventArgs e)
    {
        //chatBot.FollowersMode();
    }

    private void buttonUnfollowMode_Click(object sender, EventArgs e)
    {
        //chatBot.FollowersModeOff();
    }

    private void buttonCreateCurrency_Click(object sender, EventArgs e)
    {
        var currencyName = comboBoxCurrency.Text;
        if (!string.IsNullOrWhiteSpace(textBoxCreateCurrency.Text))
        {
            currencyName = textBoxCreateCurrency.Text;
        }

        var cases = new Cases
        {
            Nominative = textBoxNominative.Text,
            Genitive = textBoxGenitive.Text,
            Dative = textBoxDative.Text,
            Accusative = textBoxAccusative.Text,
            Instrumental = textBoxInstrumental.Text,
            Prepositional = textBoxPrepositional.Text,

            NominativeMultiple = textBoxNominativeMultiple.Text,
            GenitiveMultiple = textBoxGenitiveMultiple.Text,
            DativeMultiple = textBoxDativeMultiple.Text,
            AccusativeMultiple = textBoxAccusativeMultiple.Text,
            InstrumentalMultiple = textBoxInstrumentalMultiple.Text,
            PrepositionalMultiple = textBoxPrepositionalMultiple.Text
        };

        if (_currency.CreateCurrency(currencyName, cases))
        {
            var phrase = comboBoxCurrency.Text != currencyName
                ? $"Валюта {currencyName} создана!"
                : $"Валюта {currencyName} перезаписана";
            comboBoxCurrency.DataSource = Currency.GetCurrencies();
            comboBoxCurrency.SelectedItem = currencyName;
            MessageBox.Show(phrase);
        }
        else
        {
            MessageBox.Show("Некорректное название валюты!");
        }
    }

    private void comboBoxCurrency_SelectedIndexChanged(object sender, EventArgs e)
        => LoadCurrency();

    private void buttonGiveaway_Click(object sender, EventArgs e)
    {
        var result = _raffleHandler.Giveaway();
        labelRaffleWinner.Text = result.Nick;
        labelRaffleLink.Text = result.Link;
        textBoxRaffle.Text = string.Empty;

        var currentCommandName = _raffleHandler.GetCurrentCommandName();
        if (!string.IsNullOrEmpty(currentCommandName))
            _chatBot.Commands.Remove(currentCommandName);
    }

    private void labelRaffleLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            if (labelRaffleLink.Text.Contains("http"))
            {
                var url = labelRaffleLink.Text;
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void buttonRaffleStart_Click(object sender, EventArgs e)
    {
        var commandName = textBoxRaffle.Text;

        if (!string.IsNullOrEmpty(commandName) && commandName != "!")
        {
            if (commandName[0] == '!')
                commandName = commandName.Replace("!", string.Empty);

            var currentCommandName = _raffleHandler.GetCurrentCommandName();

            if (!string.IsNullOrEmpty(currentCommandName))
                _chatBot.Commands.Remove(currentCommandName);

            int.TryParse(textBoxGiveawayPrice.Text, out var giveawayPrice);
            _raffleHandler.RaffleStart(commandName, giveawayPrice);

            if (!string.IsNullOrEmpty(commandName) && !_chatBot.Commands.ContainsKey(commandName))
                _chatBot.Commands.Add(commandName, _raffleHandler.Participate());
        }
    }

    private async void buttonAuth_Click(object sender, EventArgs e)
    {
        try
        {
            OpenLink(_twitchAuthorization.GetAuthorizationCodeUrl());
        
            var authCodeResponse = await _twitchAuthorization.StartAuth();
            _twitchInfo.SetChannelToken(authCodeResponse);

            var streamer = await _twitchApiClient.GetAuthorizedUser();
            _twitchInfo.SetStreamerInfo(streamer);

            _chatBot.Connect([Platform.Twitch]);

            EnableButtons();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.ToString(), "Ошибка");
        }
    }

    private async void buttonAuthBot_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(_twitchAuthorization.GetAuthorizationCodeUrl(), TextDataFormat.Text);
            MessageBox.Show(
                "Ссылка для авторизации скопирована в буфер обмена (ctrl+v). Убедитесь что выбран профиль для бота",
                "Авторизация бота");

            var authCodeResponse = await _twitchAuthorization.StartAuth();
            var botUser = await _twitchApiClient.GetAuthorizedUser();
            _twitchInfo.SetBot(authCodeResponse, botUser);
            _chatBot.Connect([Platform.Twitch]);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.ToString(), "Ошибка");
        }
    }

    private void EnableButtons()
    {
        buttonAuth.Invoke(() => buttonAuth.Enabled = true);

        if (_chatBot.IsConnected())
        {
            buttonAuthBot.Invoke(() => buttonAuthBot.Enabled = true);
            tabControl1.Invoke(() => tabControl1.Enabled = true);
        }
    }

    private async void buttonCreateRewardConvertPoints_Click(object sender, EventArgs e)
    {
        try
        {
            await _twitchApiClient.CreateReward(buttonCreateRewardConvertPoints.Text, 1500);
            MessageBox.Show($"Награда \"{buttonCreateRewardConvertPoints.Text}\" создана!", "Сообщение");
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Возможно награда уже создана. {exception.Message}", "Ошибка");
        }
    }

    private async void buttonCreateRewardFirstViewer_Click(object sender, EventArgs e)
    {
        try
        {
            await _twitchApiClient.CreateReward(buttonCreateRewardFirstViewer.Text, 1500);
            MessageBox.Show($"Награда \"{buttonCreateRewardFirstViewer.Text}\" создана!", "Сообщение");
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Возможно награда уже создана. {exception.Message}", "Ошибка");
        }
    }

    private async void buttonCreateRewardIncreaseBanana_Click(object sender, EventArgs e)
    {
        try
        {
            await _twitchApiClient.CreateReward(buttonCreateRewardIncreaseBanana.Text, 1500);
            MessageBox.Show($"Награда \"{buttonCreateRewardIncreaseBanana.Text}\" создана!", "Сообщение");
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Возможно награда уже создана. {exception.Message}", "Ошибка");
        }
    }
}