using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClienteUdp
{
    class Program
    {
        static byte[] data;

        static bool done = false;
        static int recv;
        static bool stop = false;
        static int startPort;
        static int endPort;
        static List<int> openPorts = new List<int>();

        static object consoleLock = new object();

        static int waitingForResponses;

        static int maxQueriesAtOneTime = 100;
       static string ips;
        static void Main(string[] args)
        {
            //Send("TEST STRING");
            //Console.Read();
            while (!done)
            {
                UdpClient listener = new UdpClient();
                var remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, 11000);
                string welcome = "yo puedo.....!!!";

                data = Encoding.ASCII.GetBytes(welcome);
                listener.Send(data, data.Length, remoteEndPoint);
                var receivedResults = listener.Receive(ref remoteEndPoint);
                Console.WriteLine(Encoding.ASCII.GetString(receivedResults));
               
                byte[] datas = new byte[1000];
                datas = listener.Receive(ref remoteEndPoint);
                if (datas.Count() == 0)
                    break;
                Console.WriteLine(Encoding.ASCII.GetString(datas));//ip
               ips = Encoding.ASCII.GetString(datas).ToString();
                ////////////////////%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
                
                iniciar();
               

                StringBuilder builder = new StringBuilder();
                foreach (int safePrime in openPorts)
                {
                    // Append each int to the StringBuilder overload.
                    builder.Append(safePrime).Append(" ");
                }
                string result = builder.ToString();

                //Console.WriteLine(""+result);
                data = Encoding.ASCII.GetBytes(result);
                listener.Send(data, data.Length, remoteEndPoint);

                listener.Close();
                   Environment.Exit(0);
               

            }
        }//////////////////MAIN

        static void iniciar() {
            begin:
            Console.WriteLine("IP Address OK!:");
            // string ip = Console.ReadLine();
            string ip = ips;

            IPAddress ipAddress;

            if (!IPAddress.TryParse(ip, out ipAddress))
                goto begin;

            startP:

            Console.WriteLine("Port to start scanning OK!:");
           // string sp = Console.ReadLine();
            string sp = "0";
            if (!int.TryParse(sp, out startPort))
                goto startP;

            endP:

            Console.WriteLine("End port OK!:");
            //string ep = Console.ReadLine();
            string ep = "2500";

            if (!int.TryParse(ep, out endPort))
                goto endP;

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Press any key to 0%...");

            Console.WriteLine("");
            Console.WriteLine("");

            ThreadPool.QueueUserWorkItem(StartScan, ipAddress);

            Console.ReadKey();

            stop = true;

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        } 
        static void StartScan(object o)
        {
            IPAddress ipAddress = o as IPAddress;
            for (int i = startPort; i < endPort; i++)
            {
                lock (consoleLock)
                {
                    int top = Console.CursorTop;
                    Console.CursorTop = 7;
                    Console.WriteLine("Scanning port: {0}    ", i);
                    Console.CursorTop = top;
                }
                while (waitingForResponses >= maxQueriesAtOneTime)
                    Thread.Sleep(0);
                if (stop)
                    break;

                try
                {
                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    s.BeginConnect(new IPEndPoint(ipAddress, i), EndConnect, s);

                    Interlocked.Increment(ref waitingForResponses);
                }
                catch (Exception)
                {

                }
            }
        }

        static void EndConnect(IAsyncResult ar)
        {
            try
            {
                DecrementResponses();

                Socket s = ar.AsyncState as Socket;

                s.EndConnect(ar);

                if (s.Connected)
                {
                    int openPort = Convert.ToInt32(s.RemoteEndPoint.ToString().Split(':')[1]);
                    openPorts.Add(openPort);

                    lock (consoleLock)
                    {
                    //    Console.WriteLine("Connected TCP on port: {0}", openPort);
                    }

                    s.Disconnect(true);
                }
            }
            catch (Exception)
            {

            }
        }

        static void IncrementResponses()
        {
            Interlocked.Increment(ref waitingForResponses);

            PrintWaitingForResponses();
        }

        static void DecrementResponses()
        {
            Interlocked.Decrement(ref waitingForResponses);

            PrintWaitingForResponses();
        }

        static void PrintWaitingForResponses()
        {
            lock (consoleLock)
            {
                int top = Console.CursorTop;

                Console.CursorTop = 8;
                Console.WriteLine("Waiting for responses  {0} %       ", waitingForResponses);

                Console.CursorTop = top;
            }


        }
    }
}
