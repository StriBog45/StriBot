using System;
using System.IO;
using System.Windows.Forms;

namespace StriBot
{
    internal class TwitchInfo
    {
        internal readonly string Channel;
        internal readonly string AccessToken;
        internal readonly string BotName;
        internal readonly string ClientId;
        internal readonly string ChannelId;

        private string fileName = "TwitchInfo.txt";
        private string error = "Error";

        internal TwitchInfo()
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    var line = sr.ReadToEnd().Split(new char[] { '\r','\n' }, StringSplitOptions.RemoveEmptyEntries);

                    Channel = line[0].Split(new char[] { ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    AccessToken = line[1].Split(new char[] { ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    BotName = line[2].Split(new char[] { ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    ClientId = line[3].Split(new char[] { ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    ChannelId = line[4].Split(new char[] { ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                }

            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message, error);
            }
            catch(IndexOutOfRangeException e)
            {
                MessageBox.Show($"{fileName} {e.Message}", error);
            }
        }
    }
}