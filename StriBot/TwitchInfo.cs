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

                    Channel = line[0];
                    AccessToken = line[1];
                    BotName = line[2];
                    ClientId = line[3];
                    ChannelId = line[4];
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