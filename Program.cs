using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
        string url = $"http://+:{port}/send/";
        
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        Console.WriteLine($"Сервер запущен на {url}");

        while (true)
        {
            var context = await listener.GetContextAsync();
            _ = Task.Run(() => ProcessRequest(context));
        }
    }

    static async Task ProcessRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        // add CORS headers to all responses (for JavaScript example)
        response.AddHeader("Access-Control-Allow-Origin", "*");
        response.AddHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
        response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

        // handle preflight OPTIONS request (for JavaScript example)
        if (request.HttpMethod == "OPTIONS")
        {
            response.StatusCode = 200;
            response.Close();
            return;
        }

        if (request.HttpMethod != "POST")
        {
            response.StatusCode = 405;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Только POST-запросы"));
            response.Close();
            return;
        }

        using var reader = new StreamReader(request.InputStream, Encoding.UTF8);
        string numberStr = await reader.ReadToEndAsync();

        if (int.TryParse(numberStr, out int number))
        {
            int result = number + 1;
            byte[] buffer = Encoding.UTF8.GetBytes(result.ToString());

            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            Console.WriteLine($"Получено: {number}, отправлено: {result}");
        }
        else
        {
            response.StatusCode = 400;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Ошибка: ожидалось число."));
        }

        response.Close();
    }
}
