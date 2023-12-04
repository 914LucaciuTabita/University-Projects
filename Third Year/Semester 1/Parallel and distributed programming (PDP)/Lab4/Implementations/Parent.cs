using Lab4.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Implementations
{
    internal abstract class Parent
    {
        private List<string> Urls { get; }

        protected abstract string ParserType { get; }

        protected Parent(List<string> urls)
        {
            Urls = urls;
            Run();
        }

        protected abstract void Run();

        // Method to iterate over URLs and perform an action for each
        protected void ForEach(Action<int, string, string> action)
        {
            var count = 0;
            Urls.ForEach(url =>
            {
                var savePath = $"DownloadedFile_{count++}.txt"; // Change the file name as needed
                action(count, url, savePath);
            });
        }

        // Method to map URLs to a list of items using a provided mapper function
        protected List<T> Map<T>(Func<int, string, T> mapper)
        {
            var count = 0;
            return Urls.Select(url => mapper(count++, url)).ToList();
        }

        // Log a message when a socket is connected
        protected void logConnected(SocketController socket)
        {
            Console.WriteLine($"{ParserType} - {socket.Id}: Socket connected to {socket.BaseUrl} ({socket.UrlPath})");
        }

        // Log a message when data is sent through the socket
        protected void logSent(SocketController socket, int nrOfSentBytes)
        {
            Console.WriteLine($"{ParserType} - {socket.Id}: Sent {nrOfSentBytes} bytes to the server");
        }

        // Log a message when data is received through the socket
        protected void logReceive(SocketController socket)
        {
            Console.WriteLine($"{ParserType} - {socket.Id}: Received:\n\n {socket.GetResponseContent}");
        }

        // Log a message when data is downloaded through the socket
        protected void logDownload(SocketController socket, string savePath)
        {
            Console.WriteLine($"{ParserType} - {socket.Id}: Downloaded file saved to {savePath}");
        }
    }
}
