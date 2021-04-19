using DryIoc;
using StriBot.ApplicationSettings;
using StriBot.Bots;
using StriBot.Bots.Enums;
using StriBot.Commands;
using StriBot.Commands.Raffle;
using StriBot.DateBase;
using StriBot.DryIoc;
using StriBot.EventConainers;
using StriBot.Language.Implementations;
using StriBot.Language.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StriBot
{
    public partial class Form1 : Form
    {
        private delegate void SafeCallDelegate();
        private delegate void SafeCallDelegateOrders(List<(string, string, int)> orders);
        private readonly MMRManager _managerMMR;
        private readonly OrderManager _orderManager;
        private readonly CurrencyBaseManager _currencyBaseManager;
        private readonly BetsManager _betsManager;
        private readonly ChatBot _chatBot;
        private readonly ProgressManager _progressManager;
        private readonly RememberManager _rememberManager;
        private readonly Currency _currency;
        private readonly SettingsFileManager _settingsFileManager;
        private readonly RaffleManager _raffleManager;

        public Form1()
        {
            InitializeComponent();

            _chatBot = GlobalContainer.Default.Resolve<ChatBot>();
            _chatBot.CreateCommands();
            _chatBot.Connect(new[] {Platform.Twitch});
            _settingsFileManager = GlobalContainer.Default.Resolve<SettingsFileManager>();
            _currency = GlobalContainer.Default.Resolve<Currency>();
            _progressManager = GlobalContainer.Default.Resolve<ProgressManager>();
            _progressManager.SetConstructorSettings(BossUpdate, DeathUpdate);
            _managerMMR = GlobalContainer.Default.Resolve<MMRManager>();
            _orderManager = GlobalContainer.Default.Resolve<OrderManager>();
            _currencyBaseManager = GlobalContainer.Default.Resolve<CurrencyBaseManager>();
            _betsManager = GlobalContainer.Default.Resolve<BetsManager>();
            _rememberManager = GlobalContainer.Default.Resolve<RememberManager>();
            _raffleManager = GlobalContainer.Default.Resolve<RaffleManager>();
            _orderManager.SafeCallConnector(UpdateOrderList);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxMMR.Text = _managerMMR.MMR.ToString();
            comboBoxCurrency.DataSource = _currency.GetCurrencies();

            if (!string.IsNullOrEmpty(_settingsFileManager.CurrencyName))
            {
                comboBoxCurrency.SelectedItem = _settingsFileManager.CurrencyName;
            }
            LoadCurrency();
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
            => System.Diagnostics.Process.Start("https://www.twitch.tv/stribog45");

        private void timer1_Tick(object sender, EventArgs e)
            => _chatBot.TimerTick();

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Reporter.CreateCommands(_chatBot.Commands);
            _settingsFileManager.SetCurrencyName(comboBoxCurrency.Text);
            _settingsFileManager.SaveSettings();
        }

        private void buttonDistribution_Click(object sender, EventArgs e)
            => _currencyBaseManager.DistributionMoney(Convert.ToInt32(DistributionMoneyPerUser.Text), Convert.ToInt32(DistributionMaxUsers.Text), Bots.Enums.Platform.Twitch);
        private void buttonCreateOptions_Click(object sender, EventArgs e)
        {
            var options = TextBoxOptions.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            _betsManager.CreateBets(options, new[] { Platform.Twitch });
        }
        private void buttonBetsOfManiac_Click(object sender, EventArgs e)
            => _betsManager.CreateBets(new[] { "Количество повешанных", "0", "1", "2", "3", "4" }, new[] { Platform.Twitch });

        private void buttonBetsOfSurvivors_Click(object sender, EventArgs e)
            => _betsManager.CreateBets(new[] { "Количество сбежавших", "0", "1", "2", "3", "4" }, new[] { Platform.Twitch });

        private void buttonBetsOfSurvivor_Click(object sender, EventArgs e)
            => _betsManager.CreateBets(new[] { "Выживание стримера", "выжил", "погиб" }, new[] { Platform.Twitch });

        private void buttonBetsDota2_Click(object sender, EventArgs e)
            => _betsManager.CreateBets(new[] { "Победа команды", "radiant", "dire" }, new[] { Platform.Twitch });

        private void buttonStopBets_Click(object sender, EventArgs e)
            => _betsManager.StopBetsProcess(new[] { Platform.Twitch });

        private void buttonSelectWinner_Click(object sender, EventArgs e)
            => _betsManager.SetBetsWinner(Convert.ToInt32(numericUpDownWinnerSelcter.Value), new[] { Platform.Twitch });

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
                    listViewOrder.Items.Add(new ListViewItem(new[] { order.Item1, order.Item2.ToString(), order.Item3.ToString() }));
            }
        }

        private void buttonOrderAccept_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selected in listViewOrder.SelectedItems)
            {
                if (selected.SubItems[0].Text.Contains("youtube"))
                    webBrowser.Navigate(selected.SubItems[0].Text);
                _orderManager.OrderRemove(selected.SubItems[0].Text, selected.SubItems[1].Text, int.Parse(selected.SubItems[2].Text));
                DataBase.AddMoney(selected.SubItems[1].Text, -int.Parse(selected.SubItems[2].Text));
                GlobalEventContainer.Message(string.Format("Заказ @{0} на {1} принят", selected.SubItems[1].Text, selected.SubItems[0].Text), Platform.Twitch);
                listViewOrder.Items.Remove(selected);
            }
        }
        private void buttonOrderCancel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selected in listViewOrder.SelectedItems)
            {
                _orderManager.OrderRemove(selected.SubItems[0].Text, selected.SubItems[1].Text, int.Parse(selected.SubItems[2].Text));
                GlobalEventContainer.Message(string.Format("Заказ @{0} отменен", selected.SubItems[1].Text), Platform.Twitch);
                listViewOrder.Items.Remove(selected);
            }
        }
        private void buttonMMRSet_Click(object sender, EventArgs e)
            => _managerMMR.MMR = Convert.ToInt32(textBoxMMR.Text);
        private void buttonMMRCheck_Click(object sender, EventArgs e)
            => textBoxMMR.Text = _managerMMR.MMR.ToString();
        private void buttonWinsCheck_Click(object sender, EventArgs e)
        {
            textBox1.Text = _managerMMR.Wins.ToString();
            textBox2.Text = _managerMMR.Losses.ToString();
        }
        private void buttonWinsSet_Click(object sender, EventArgs e)
        {
            _managerMMR.Wins = Convert.ToInt32(textBox1.Text);
            _managerMMR.Losses = Convert.ToInt32(textBox2.Text);
        }

        private void BossUpdate()
        {
            var d = new SafeCallDelegate(BossUpdate);

            if (listView1.InvokeRequired)
                listView1.Invoke(d);
            else
            {
                listView1.Items.Clear();
                foreach (var boss in _progressManager.GetBosses())
                    listView1.Items.Add(new ListViewItem(boss));
            }
        }

        private void buttonBossDelete_Click(object sender, EventArgs e)
        {
            foreach (int item in listView1.SelectedIndices)
                _progressManager.BossRemoveByIndex(item);
        }

        private void buttonDeathAdd_Click(object sender, EventArgs e)
            => _progressManager.Deaths++;

        private void buttonDeathReduce_Click(object sender, EventArgs e)
            => _progressManager.Deaths--;

        private void DeathUpdate()
        {
            var d = new SafeCallDelegate(DeathUpdate);

            if (listView1.InvokeRequired)
                listView1.Invoke(d);
            else
            {
                label1.Text = $"Смертей: {_progressManager.Deaths}";
            }
        }

        private void listViewOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(listViewOrder.SelectedItems.Count > 0)
            //    linkLabelOrder.Text = listViewOrder.SelectedItems[0].SubItems[0].Text;
        }

        private void buttonReminderClear_Click(object sender, EventArgs e)
        {
            _rememberManager.TextReminder = string.Empty;
            GlobalEventContainer.Message("Напоминание удалено", Platform.Twitch);
        }

        private void buttonReconnect_Click(object sender, EventArgs e)
            => _chatBot.Reconnect(new[] { Platform.Twitch });

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
                comboBoxCurrency.DataSource = _currency.GetCurrencies();
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
            var result = _raffleManager.Giveaway();
            labelRaffleWinner.Text = result.Nick;
            labelRaffleLink.Text = result.Link;
            textBoxRaffle.Text = string.Empty;

            var currentCommandName = _raffleManager.GetCurrentCommandName();
            if (!string.IsNullOrEmpty(currentCommandName))
                _chatBot.Commands.Remove(currentCommandName);
        }

        private void labelRaffleLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (labelRaffleLink.Text.Contains("http"))
                System.Diagnostics.Process.Start(labelRaffleLink.Text);
        }

        private void buttonRaffleStart_Click(object sender, EventArgs e)
        {
            var commandName = textBoxRaffle.Text;

            if (!string.IsNullOrEmpty(commandName) && commandName != "!")
            {
                if (commandName[0] == '!')
                    commandName = commandName.Replace("!", string.Empty);

                var currentCommandName = _raffleManager.GetCurrentCommandName();

                if (!string.IsNullOrEmpty(currentCommandName))
                    _chatBot.Commands.Remove(currentCommandName);

                _raffleManager.RaffleStart(commandName);

                if (!string.IsNullOrEmpty(commandName) && !_chatBot.Commands.ContainsKey(commandName))
                    _chatBot.Commands.Add(commandName, _raffleManager.Participate());
            }
        }
    }
}