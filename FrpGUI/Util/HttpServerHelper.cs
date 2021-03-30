using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FrpGUI.Util
{
    public class HttpServerHelper
    {
        private HttpListener listener;

        public async Task Start()
        {
            try
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add($"http://{Config.Instance.AdminAddress}:{Config.Instance.AdminPort}/");
                listener.Start();

                var requests = new HashSet<Task>();
                for (int i = 0; i < 10; i++)
                    requests.Add(listener.GetContextAsync());

                while (true)
                {
                    Task t = await Task.WhenAny(requests);
                    requests.Remove(t);

                    if (t is Task<HttpListenerContext>)
                    {
                        var context = (t as Task<HttpListenerContext>).Result;
                        requests.Add(ProcessRequestAsync(context));
                        requests.Add(listener.GetContextAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                (App.Current.MainWindow as MainWindow).AddLogOnMainThread("启动远程管理错误：" + ex.Message, "E");
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            try
            {
                var response = context.Response;
                var request = context.Request;
                string responseString = "";
                switch (request.HttpMethod)
                {
                    case "POST":
                        using (Stream body = request.InputStream)
                        {
                            using StreamReader reader = new StreamReader(body, request.ContentEncoding);
                            string value = reader.ReadToEnd();
                            JObject json = JObject.Parse(value);
                            bool b = json["action"].Value<bool>();
                            string id = json["id"].Value<string>();
                            string password = json["password"].Value<string>();
                            if (!string.IsNullOrEmpty(password) || !string.IsNullOrEmpty(Config.Instance.AdminPassword))
                            {
                                if (password != Config.Instance.AdminPassword)
                                {
                                    (App.Current.MainWindow as MainWindow).AddLogOnMainThread(request.RemoteEndPoint.Address.ToString() + "：密码错误", "W");

                                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    return;
                                }
                            }

                            (App.Current.MainWindow as MainWindow).AddLogOnMainThread($"远程管理：POST id={id}, action={b}", "I");
                            var server = Config.Instance.FrpConfigs.FirstOrDefault(p => p.ID.ToString().Equals(id));
                            if (server == null)
                            {
                                (App.Current.MainWindow as MainWindow).AddLogOnMainThread("远程管理：" + request.RemoteEndPoint.Address.ToString() + "：需要操纵的服务端ID不存在", "W");
                                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                return;
                            }
                            if (b && !server.Process.IsRunning)
                            {
                                (App.Current.MainWindow as MainWindow).AddLogOnMainThread(request.RemoteEndPoint.Address.ToString() + "：启动了服务器", "I");

                                server.Start();
                            }
                            else if (!b && server.Process.IsRunning)
                            {
                                (App.Current.MainWindow as MainWindow).AddLogOnMainThread(request.RemoteEndPoint.Address.ToString() + "：关闭了服务器", "I");

                                await server.StopAsync();
                            }
                        }
                        break;

                    case "GET":

                        (App.Current.MainWindow as MainWindow).AddLogOnMainThread($"远程管理：GET {context.Request.RawUrl}", "I");
                        if (context.Request.RawUrl == "/")
                        {
                            responseString = File.ReadAllText("html/admin.html").Replace("{{url}}", $"http://{Config.Instance.AdminAddress}:{Config.Instance.AdminPort}");
                            StringBuilder items = new StringBuilder();
                            foreach (var server in Config.Instance.FrpConfigs.OfType<ServerConfig>())
                            {
                                var notRun = server.ProcessStatus == ProcessStatus.NotRun;
                                items.Append($"<a class=\"text-center label\">{server.Name}：{(notRun ? "没有运行" : "正在运行")}</a> ")
                                    .Append($"<button class=\"btn btn-primary \" data-id=\"{server.ID}\" data-action=\"{(notRun ? "true" : "false")}\">{(notRun ? "启动" : "停止")}</button>")
                                    .Append("<p></p>");
                            }
                            responseString = responseString.Replace("{{items}}", items.ToString());
                            (App.Current.MainWindow as MainWindow).AddLogOnMainThread("远程管理：" + request.RemoteEndPoint.Address.ToString() + "：访问远程管理网页", "I");
                        }
                        else
                        {
                            responseString = File.ReadAllText("html" + context.Request.RawUrl);
                        }
                        break;
                }

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                (App.Current.MainWindow as MainWindow).AddLogOnMainThread("远程管理服务器错误：" + ex.Message, "E");
            }
            finally
            {
                context.Response.OutputStream.Close();
            }
        }

        private void AddNewListener()
        {
            var prefixes = new[] { $"http://{Config.Instance.AdminAddress}:{Config.Instance.AdminPort}/" };
            if (listener != null)
            {
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request.
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
            listener.Stop();
        }
    }
}