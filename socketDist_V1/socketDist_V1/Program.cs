using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Globalization;

namespace socketDist_V1
{
    class Program
    {
        static bool stop = false;
        static int startPort;
        static int endPort;

        static List<int> openPorts = new List<int>();

        static object consoleLock = new object();

        static int waitingForResponses;

        static int maxQueriesAtOneTime = 100;


        private const int listenPort = 11000;
        static ArrayList interval = new ArrayList();
        static UdpClient listener = new UdpClient(listenPort);
        private static void StartListener()
        {

            bool done = false;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, listenPort);
            int contadorS = 0;
            byte[] datos;
            byte[] bytes = new byte[1024];
            try
            {

                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    bytes = listener.Receive(ref groupEP);
                    Console.WriteLine("Received broadcast from {0} :\n {1}\n", groupEP.ToString(), Encoding.ASCII.GetString(bytes, 0, bytes.Length));

                    //interval.Add(groupEP.ToString());
                    contadorS = contadorS + 1;
                    Console.WriteLine("Disponibles " + contadorS);

                    string welcome = "espera te envio los puertos.....!!!";
                    datos = Encoding.ASCII.GetBytes(welcome);
                    listener.Send(datos, datos.Length, groupEP);




                    byte[] dat;
                    dat = Encoding.ASCII.GetBytes("192.168.1.66");//envio puertos
                    listener.Send(dat, dat.Length, groupEP);






                    byte[] myReadBuffer = listener.Receive(ref groupEP);
                    Console.WriteLine("puertos abiertos: " + "\n" + Encoding.ASCII.GetString(myReadBuffer, 0, myReadBuffer.Length));


                }

                //Conexiones_Disponibles(contadorS);


                //string intervalos = "1999";
                //datos = Encoding.ASCII.GetBytes(intervalos);
                //listener.Send(datos, datos.Length, groupEP);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

        public static int Main()
        {
            StartListener();

            return 0;
        }


        public static void Conexiones_Disponibles(int con)
        {

            if (con >= 3)
            {
                Console.Clear();

                Console.WriteLine("te tocan :" + puertos(3));

            }
        }

        public static double puertos(double con)
        {
            double division = 5000 / con;
            double teToca = Math.Floor(division);

            int i = (int)con;
            switch (i)
            {
                case 1:

                    break;
                case 2:

                    break;
                case 3:
                    string intervalo = "1666";
                    string intervalo2 = "3332";
                    string intervalo3 = "5000";
                    intervalos(intervalo, intervalo2, intervalo3);
                    break;
                default:

                    break;
            }

            return teToca;
        }
        public static void intervalos(string uno, string dos, string tres)
        {
            List<string> list = new List<string>();
            list.Add(uno);
            list.Add(dos);
            list.Add(tres);
            list.Add("");
            byte[] dat;

            Console.Write("Content: ");
            for (int i = 0; i < interval.Count; i++)
            {
                dat = Encoding.ASCII.GetBytes(list[i]);
                Console.Write("ip:   " + interval[i].ToString() + "\n");
                CreateIPEndPoint(interval[i].ToString());
                Console.Write("IPENPONTSSS  " + CreateIPEndPoint(interval[i].ToString()) + "\n");
                listener.Send(dat, dat.Length, CreateIPEndPoint(interval[i].ToString()));

            }

        }

        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length != 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
            int port;
            if (!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }





    }
}