using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StriBot.Application.Server.Models;

namespace StriBot.Application.Server
{
    public class WebServer
    {
        private readonly HttpListener _listener;

        public WebServer(string uri)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(uri);
        }

        public async Task<AuthorizationModel> Listen()
        {
            _listener.Start();

            return await OnRequest();
        }

        public void Stop()
            => _listener.Stop();

        private async Task<AuthorizationModel> OnRequest()
        {
            while (_listener.IsListening)
            {
                var listenerContext = await _listener.GetContextAsync();
                var listenerRequest = listenerContext.Request;
                var listenerResponse = listenerContext.Response;

                using (var writer = new StreamWriter(listenerResponse.OutputStream))
                {
                    if (listenerRequest.QueryString.AllKeys.Any("code".Contains))
                    {
                        await writer.WriteLineAsync("Authorization started! Check your application!");
                        await writer.FlushAsync();

                        return new AuthorizationModel(listenerRequest.QueryString["code"]);
                    }

                    await writer.WriteLineAsync("No code found in query string!");
                    await writer.FlushAsync();
                }
            }

            return null;
        }
    }
}