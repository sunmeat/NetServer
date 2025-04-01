using System;
using System.Net;
using System.Text;

class Program
{
    static void Main()
    {
        string url = "http://*:5000/";
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        Console.WriteLine($"Сервер запущен на {url}");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string numberStr = request.QueryString["number"];

            if (int.TryParse(numberStr, out int number))
            {
                int result = number + 1;
                byte[] buffer = Encoding.UTF8.GetBytes(result.ToString());

                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                Console.WriteLine($"Получено: {number}, отправлено: {result}");
            }

            response.OutputStream.Close();
        }
    }
}
