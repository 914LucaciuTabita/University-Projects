using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Socket
{
    internal class SocketController : System.Net.Sockets.Socket
    {
        public int Id { get; }

        public string BaseUrl { get; }

        public string UrlPath { get; }

        private IPEndPoint EndPoint { get; }

        private StringBuilder ResponseBuilder { get; }

        private const int DefaultHttpPort = 80;
        private const int BufferSize = 1024;

        private async Task<List<string>> SendHttpRequestAsync(string method)
        {
            var request = $"{method} {UrlPath} HTTP/1.1\r\n" +
                          $"Host: {BaseUrl}\r\n" +
                          "Connection: close\r\n" +
                          "\r\n";

            var requestData = Encoding.ASCII.GetBytes(request);
            await SendDataAsync(requestData);

            var response = await ReceiveHttpResponseAsync();
            return ParseHttpHeader(response);
        }

        private async Task<string> ReceiveHttpResponseAsync()
        {
            var responseBuilder = new StringBuilder();
            var buffer = new byte[BufferSize];

            while (true)
            {
                var bytesRead = await ReceiveDataAsync(buffer, 0, BufferSize);
                if (bytesRead == 0)
                {
                    break;
                }

                responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            }

            return responseBuilder.ToString();
        }

        private async Task SendDataAsync(byte[] data)
        {
            await Task.Factory.FromAsync(
                (callback, state) => BeginSend(data, 0, data.Length, SocketFlags.None, callback, state),
                EndSend,
                null);
        }

        private List<string> ParseHttpHeader(string response)
        {
            var lines = response.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var emptyLineIndex = lines.FindIndex(string.IsNullOrWhiteSpace);

            if (emptyLineIndex >= 0)
            {
                lines.RemoveRange(emptyLineIndex, lines.Count - emptyLineIndex);
            }

            return lines;
        }

        private int ParseContentLength(List<string> headerLines)
        {
            const string ContentLengthHeader = "Content-Length:";

            foreach (var line in headerLines)
            {
                if (line.StartsWith(ContentLengthHeader, StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(line.Substring(ContentLengthHeader.Length).Trim(), out var contentLength))
                {
                    return contentLength;
                }
            }

            return 0;
        }

        private Task<int> ReceiveDataAsync(byte[] buffer, int offset, int count)
        {
            return Task<int>.Factory.FromAsync(
                (callback, state) => BeginReceive(buffer, offset, count, SocketFlags.None, callback, state),
                EndReceive,
                null);
        }

        public async Task<int> BeginDownloadAsync(string method, string savePath)
        {
            var headerLines = await SendHttpRequestAsync(method);
            var contentLength = ParseContentLength(headerLines);

            if (contentLength > 0)
            {
                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    var buffer = new byte[BufferSize];
                    var totalBytesRead = 0;
                    int bytesRead;

                    while (totalBytesRead < contentLength &&
                           (bytesRead = await ReceiveDataAsync(buffer, 0, BufferSize)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;
                    }

                    return totalBytesRead;
                }
            }

            return 0;
        }

        // Factory method to create a new SocketController instance
        public static SocketController create(string url, int id)
        {
            // Extract base URL and URL path from the input URL
            var index = url.IndexOf('/');
            var baseUrl = index < 0 ? url : url.Substring(0, index);
            var urlPath = index < 0 ? "/" : url.Substring(index);

            // Get IP address for the base URL
            var ipHostInfo = Dns.GetHostEntry(baseUrl);
            var ipAddress = ipHostInfo.AddressList[0];

            // Create and return a new instance of SocketController
            return new SocketController(baseUrl, urlPath, ipAddress, id);
        }

        // Constructor initializes the SocketController with necessary parameters
        private SocketController(string baseUrl, string urlPath, IPAddress iPAddress, int id)
            : base(iPAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp)
        {
            Id = id;
            BaseUrl = baseUrl;
            UrlPath = urlPath;
            EndPoint = new IPEndPoint(iPAddress, DefaultHttpPort);
            ResponseBuilder = new StringBuilder();
        }

        // Initiates a connection to the server asynchronously
        public void BeginConnect(Action<SocketController> onConnected)
        {
            BeginConnect(EndPoint, asyncResult =>
            {
                EndConnect(asyncResult);
                // Invoke the callback method when connected
                onConnected(this);
            }, null);
        }

        // Initiates sending data to the server asynchronously
        public void BeginSend(Action<SocketController, int> onSent)
        {
            // Construct the HTTP GET request string
            var stringToSend = $"GET {UrlPath} HTTP/1.1\r\n" +
                $"Host: {BaseUrl}\r\n" +
                "Content-Length: 0\r\n\r\n";
            var encodedString = Encoding.ASCII.GetBytes(stringToSend);

            // Begin sending data asynchronously
            BeginSend(encodedString, 0, encodedString.Length, SocketFlags.None, asyncResult =>
            {
                // Complete the send operation and invoke the callback method
                var numberOfSentBytes = EndSend(asyncResult);
                onSent(this, numberOfSentBytes);
            }, null);
        }

        // Initiates receiving data from the server asynchronously
        public void BeginReceive(Action<SocketController> onReceived)
        {
            // Initialize a buffer to receive data
            var buffer = new byte[BufferSize];
            ResponseBuilder.Clear();

            // Begin receiving data asynchronously
            BeginReceive(buffer, 0, BufferSize, SocketFlags.None, asyncResult => 
                            HandleReceiveResult(asyncResult, buffer, onReceived), null);
        }

        // Initiates a connection to the server asynchronously using Task
        public Task BeginConnectAsync() => Task.Run(() =>
        {
            // Create a task that completes when the connection is established
            var taskCompletion = new TaskCompletionSource<object>();

            BeginConnect(_ => { taskCompletion.TrySetResult(null); });

            return taskCompletion.Task;
        });

        // Initiates sending data to the server asynchronously using Task
        public Task<int> BeginSendAsync() => Task.Run(() =>
        {
            // Create a task that completes when the send operation is complete
            var taskCompletion = new TaskCompletionSource<int>();

            BeginSend((_, numberOfSentBytes) => taskCompletion.TrySetResult(numberOfSentBytes));

            return taskCompletion.Task;
        });

        // Initiates receiving data from the server asynchronously using Task
        public Task BeginReceiveAsync() => Task.Run(() =>
        {
            // Create a task that completes when the receive operation is complete
            var taskCompletion = new TaskCompletionSource<object>();

            BeginReceive(_ => taskCompletion.TrySetResult(null));

            return taskCompletion.Task;
        });

        public void ShutdownAndClose()
        {
            Shutdown(SocketShutdown.Both);
            Close();
        }

        // Gets the accumulated response content as a string
        public string GetResponseContent => ResponseBuilder.ToString();

        // Handles the result of asynchronous receive operations
        private void HandleReceiveResult(
            IAsyncResult asyncResult,
            byte[] buffer,
            Action<SocketController> onReceived)
        {
            // Complete the receive operation and append received data to ResponseBuilder
            var numberOfReadBytes = EndReceive(asyncResult);
            ResponseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, numberOfReadBytes));

            // If the response does not contain "</html>", initiate another receive operation
            if (!ResponseBuilder.ToString().Contains("</html>"))
            {
                BeginReceive(buffer, 0, BufferSize, SocketFlags.None, asyncResult2 => HandleReceiveResult(asyncResult2, buffer, onReceived), null);
                return;
            }

            // Invoke the callback method when the complete response is received
            onReceived(this);
        }
    }
}
