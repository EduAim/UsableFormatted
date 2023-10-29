﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using WatsonWebsocket;
using System.IO;
using UsfoModels;
using System.Configuration;

namespace UsableFormatted.Controller
{
    delegate void DocumentReceivedHandler(object sender, EventArgs e);

    internal static class MoodleController
    {
        public static event DocumentReceivedHandler OnDocumentReceived;

        private const string SOCKET_ADDRESS = "127.0.0.1";
        private const int SOCKET_PORT = 18080;

        internal static bool InitSocketServer()
        {
            var isConfigured = ConfigurationManager.AppSettings.Get("moodleSockets") == "1";
            if (!isConfigured)
                return false;

            Task.Run(() =>
            {
                WsServer();
            });
            return true;
        }

        internal static void WsServer()
        {
            WatsonWsServer server = new WatsonWsServer(SOCKET_ADDRESS, SOCKET_PORT, false);
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.MessageReceived += Server_MessageReceived;
            server.Start();
        }

        private static void Server_MessageReceived(object? sender, MessageReceivedEventArgs args)
        {
            try
            {
                Debug.WriteLine("Message received from " + args.Client.ToString() + ": " + args.Data.Count);
                byte[] dataArray = args.Data.Array;
                int newlineIndex = Array.FindIndex(dataArray, args.Data.Offset, args.Data.Count, b => b == (byte)'\n');
                if (newlineIndex == -1)
                {
                    Debug.WriteLine($"Error: No newline in data!");
                    return;
                }

                var fileName = Encoding.UTF8.GetString(args.Data.Take(newlineIndex).ToArray());
                var data = args.Data.Skip(newlineIndex + 1).ToArray();
                var fullPath = Path.Combine(FileOperations.GetTempPath(), $"usfo-{DateTime.Now:yyyyMMddHHmmssfff}-{fileName}");
                File.WriteAllBytes(fullPath, data);

                Debug.WriteLine($"Filename: {fullPath}; Length: {data.Length}");

                if (OnDocumentReceived != null)
                    OnDocumentReceived(fullPath, new EventArgs());
            }
            catch (Exception ex)
            {
                ex.TraceEx();
            }
        }

        private static void Server_ClientDisconnected(object? sender, DisconnectionEventArgs args)
        {
            Debug.WriteLine("Server_ClientDisconnected");
        }

        private static void Server_ClientConnected(object? sender, ConnectionEventArgs args)
        {
            Debug.WriteLine("Server_ClientConnected");
        }
    }
}
