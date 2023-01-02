using System;
using System.IO;
using System.Windows.Forms;

namespace StriBot
{
    internal class TwitchInfo
    {
        internal readonly string Channel;
        internal readonly string BotAccessToken;
        internal readonly string ChannelAccessToken;
        internal readonly string BotName;
        internal readonly string BotClientId;
        internal readonly string ChannelId;

        private readonly string fileName = "TwitchInfo.txt";
        private readonly string error = "Error";

        internal TwitchInfo()
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    var line = sr.ReadToEnd().Split(new []{ '\r','\n' }, StringSplitOptions.RemoveEmptyEntries);

                    Channel = line[0].Split(new []{ ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    BotAccessToken = line[1].Split(new []{ ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    ChannelAccessToken = line[2].Split(new []{ ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    BotName = line[3].Split(new []{ ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    BotClientId = line[4].Split(new []{ ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    ChannelId = line[5].Split(new []{ ':', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
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