using Lab4.Socket;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Lab4.Implementations
{
    internal class CallbackSolution : Parent
    {
        protected override string ParserType => "Callback";

        public CallbackSolution(List<string> urls) : base(urls) { }

        protected override void Run()
        {
            ForEach((index, url, savePath) => Start(SocketController.create(url, index), savePath));
        }

        private void Start(SocketController socket, string savePath)
        {
            socket.BeginConnect(HandleConnected);
            do
            {
                Thread.Sleep(100);
            } while (socket.Connected);
        }

        private void HandleConnected(SocketController socket)
        {
            logConnected(socket);
            socket.BeginSend(HandleSent);
        }

        private void HandleSent(SocketController socket, int nrOfBytes)
        {
            logSent(socket, nrOfBytes);
            socket.BeginReceive(HandleReceived);
        }

        private void HandleReceived(SocketController socketController)
        {
            logReceive(socketController);

            // Extract content from the HTTP response
            var responseContent = socketController.GetResponseContent;

            // Save content to a file (you may need to modify the file path and name)
            var filePath = $"C:\\UBB INFO ENGL\\University-Projects\\Third Year\\Semester 1\\Parallel and distributed programming (PDP)\\Lab4\\downloaded_file_{socketController.Id}.html";
            System.IO.File.WriteAllText(filePath, responseContent);

            // Shutdown and close the socket
            socketController.ShutdownAndClose();
        }
    }
}
