using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using System.Text;

using System.Text.RegularExpressions;
using System.Threading;
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
                        using (Stream body = request.InputStream) // here we have data
                        {
                            using StreamReader reader = new StreamReader(body, request.ContentEncoding);
                            string value = reader.ReadToEnd();
                            JObject json = JObject.Parse(value);
                            bool b = json["value"].Value<bool>();
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
                            if (b && !ProcessHelper.Server.IsRunning)
                            {
                                (App.Current.MainWindow as MainWindow).AddLogOnMainThread(request.RemoteEndPoint.Address.ToString() + "：启动了服务器", "I");

                                await ProcessHelper.Server.StartServerAsync(Config.Instance.Server);
                            }
                            else if (!b && ProcessHelper.Server.IsRunning)
                            {
                                (App.Current.MainWindow as MainWindow).AddLogOnMainThread(request.RemoteEndPoint.Address.ToString() + "：关闭了服务器", "I");

                                await ProcessHelper.Server.StopAsync();
                            }
                        }
                        break;

                    case "GET":
                        if (context.Request.RawUrl == "/")
                        {
                            responseString = File.ReadAllText("html/admin.html").Replace("{{url}}", $"http://{Config.Instance.AdminAddress}:{Config.Instance.AdminPort}");
                            if (ProcessHelper.Server.IsRunning)
                            {
                                responseString = responseString.Replace("{{status}}", "正在运行").Replace("{{button}}", "停止").Replace("{{data}}", "false");
                            }
                            else
                            {
                                responseString = responseString.Replace("{{status}}", "没有运行").Replace("{{button}}", "启动").Replace("{{data}}", "true");
                            }

                            (App.Current.MainWindow as MainWindow).AddLogOnMainThread(request.RemoteEndPoint.Address.ToString() + "：访问远程管理网页", "I");
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