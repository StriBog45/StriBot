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

        public TwitchBot MyBot { get;set;}
        public Form1()
        {
            InitializeComponent();

            MyBot = new TwitchBot(UpdateOrderList, BossUpdate, DeathUpdate);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxMMR.Text = MyBot.CoreMMR.ToString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.twitch.tv/stribog45");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MyBot.TimerTick();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Reporter.CreateCommands(MyBot.Commands);
        }

        private void buttonDistribution_Click(object sender, EventArgs e)
        {
            MyBot.DistributionMoney(Convert.ToInt32(DistributionMoneyPerUser.Text), Convert.ToInt32(DistributionMaxUsers.Text));
        }
        private void buttonCreateOptions_Click(object sender, EventArgs e)
        {
            var options = TextBoxOptions.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            MyBot.CreateBets(options);
        }
        private void buttonBetsOfManiac_Click(object sender, EventArgs e)
        {
            MyBot.CreateBets(new string[] { "Количество повешанных","0", "1", "2", "3", "4" });
        }
        private void buttonBetsOfSurvivors_Click(object sender, EventArgs e)
        {
            MyBot.CreateBets(new string[] { "Количество сбежавших","0", "1", "2", "3", "4" });
        }
        private void buttonBetsOfSurvivor_Click(object sender, EventArgs e)
        {
            MyBot.CreateBets(new string[] { "Выживание стримера","выжил", "погиб" });
        }
        private void buttonBetsDota2_Click(object sender, EventArgs e)
        {
            MyBot.CreateBets(new string[] { "Победа в матче","radiant", "dire" });
        }
        private void buttonStopBets_Click(object sender, EventArgs e)
        {
            MyBot.StopBetsProcess();
        }
        private void buttonSelectWinner_Click(object sender, EventArgs e)
        {
            MyBot.SetBetsWinner(Convert.ToInt32(numericUpDownWinnerSelcter.Value));
        }

        void UpdateOrderList(List<(string, string, int)> orders)
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
                    listViewOrder.Items.Add(new ListViewItem(new string[] { order.Item1, order.Item2.ToString(), order.Item3.ToString() }));
            }
        }

        private void buttonOrderAccept_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selected in listViewOrder.SelectedItems)
            {
                if(selected.SubItems[0].Text.Contains("youtube"))
                    webBrowser.Navigate(selected.SubItems[0].Text);
                MyBot.ListOrders.Remove((selected.SubItems[0].Text, selected.SubItems[1].Text, Int32.Parse(selected.SubItems[2].Text)));
                DataBase.AddMoneyToUser(selected.SubItems[1].Text, -Int32.Parse(selected.SubItems[2].Text));
                MyBot.SendMessage(String.Format("Заказ @{0} на {1} принят", selected.SubItems[1].Text, selected.SubItems[0].Text));
                listViewOrder.Items.Remove(selected);
            }
        }
        private void buttonOrderCancel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selected in listViewOrder.SelectedItems)
            {
                MyBot.ListOrders.Remove((selected.SubItems[0].Text, selected.SubItems[1].Text, Int32.Parse(selected.SubItems[2].Text)));
                MyBot.SendMessage(String.Format("Заказ @{0} отменен", selected.SubItems[1].Text));
                listViewOrder.Items.Remove(selected);
            }
        }
        private void buttonMMRSet_Click(object sender, EventArgs e)
        {
            MyBot.CoreMMR = Convert.ToInt32(textBoxMMR.Text);
        }
        private void buttonMMRCheck_Click(object sender, EventArgs e)
        {
            textBoxMMR.Text = MyBot.CoreMMR.ToString();
        }
        private void buttonWinsCheck_Click(object sender, EventArgs e)
        {
            textBox1.Text = MyBot.Wins.ToString();
            textBox2.Text = MyBot.Losses.ToString();
        }
        private void buttonWinsSet_Click(object sender, EventArgs e)
        {
            MyBot.Wins = Convert.ToInt32(textBox1.Text);
            MyBot.Losses = Convert.ToInt32(textBox2.Text);
        }

        void BossUpdate()
        {
            var d = new SafeCallDelegate(BossUpdate);

            if (listView1.InvokeRequired)
                listView1.Invoke(d);
            else
            {
                listView1.Items.Clear();
                foreach (var boss in MyBot.Bosses)
                    listView1.Items.Add(new ListViewItem(boss));
            }
        }

        private void buttonBossDelete_Click(object sender, EventArgs e)
        {
            foreach (int item in listView1.SelectedIndices)
                MyBot.Bosses.RemoveAt(item);
        }

        private void buttonDeathAdd_Click(object sender, EventArgs e)
            => MyBot.Deaths++;

        private void buttonDeathReduce_Click(object sender, EventArgs e)
            => MyBot.Deaths--;

        void DeathUpdate()
        {
            var d = new SafeCallDelegate(DeathUpdate);

            if (listView1.InvokeRequired)
                listView1.Invoke(d);
            else
            {
                label1.Text = String.Format("Смертей: {0}", MyBot.Deaths);
            }
        }

        private void listViewOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(listViewOrder.SelectedItems.Count > 0)
            //    linkLabelOrder.Text = listViewOrder.SelectedItems[0].SubItems[0].Text;
        }

        private void buttonReminderClear_Click(object sender, EventArgs e)
        {
            MyBot.TextReminder = "";
            MyBot.SendMessage("Напоминание удалено");
        }

        private void buttonReconnect_Click(object sender, EventArgs e)
            => MyBot.Reconnect();

        private void buttonSmileMode_Click(object sender, EventArgs e)
            => MyBot.SmileMode();

        private void SubMode_Click(object sender, EventArgs e)
            => MyBot.SubMode();

        private void buttonFollowMode_Click(object sender, EventArgs e)
            => MyBot.FollowersMode();

        private void buttonUnfollowMode_Click(object sender, EventArgs e)
            => MyBot.FollowersModeOff();
    }
}
