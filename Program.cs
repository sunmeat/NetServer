using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main()
    {
        const int port = 5000;
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Сервер запущен на порту {port}");

        while (true)
        {
            using var client = listener.AcceptTcpClient();
            using var stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (int.TryParse(receivedData, out int number))
            {
                int result = number + 1;
                byte[] response = Encoding.UTF8.GetBytes(result.ToString());
                stream.Write(response, 0, response.Length);
                Console.WriteLine($"Получено: {number}, отправлено: {result}");
            }
        }
    }
}