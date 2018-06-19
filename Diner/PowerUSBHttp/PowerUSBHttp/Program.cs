using PowerUSB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerUSBHttp
{
    class Program
    {
        static long lastTick = 0;
        static int secondTolerance = 2;
        static string lastSwitches = "222";
        static void Main(string[] args)
        {
            string prefix = "http://*:8088/";
            if (args.Length > 1)
                prefix = args[1];
            if (args.Length > 2)
            {
                if (!int.TryParse(args[2], out secondTolerance))
                    secondTolerance = 3;
            }
                
            var web = new HttpListener();
            web.Prefixes.Add(prefix);
            web.Start();
            setSwitch("000");
            Console.WriteLine("Listening...");
            Task.Run(() =>
            {
                while (true)
                {
                    var val = Interlocked.Read(ref lastTick);
                    if (TimeSpan.FromTicks(DateTime.Now.Ticks - val).TotalSeconds >= secondTolerance)
                    {
                        setSwitch("000");
                    }
                    Thread.Sleep(1000);
                }
            });
            while (true)
            {
                HttpListenerContext context = web.GetContext();
                ProcessRequest(context);
            }
        }
        static void ProcessRequest(HttpListenerContext context)
        {
            string ret = "OK";

            string request = context.Request.QueryString["switches"];
            
            if (request != null)
                setSwitchNew(request);
            else
                ret = "?switches=[1/0][1/0][1/0]";

            byte[] bytes = Encoding.UTF8.GetBytes(ret);
            context.Response.ContentLength64 = bytes.Length;
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Close();

        }
        static void setSwitchNew(string request)
        {
            Interlocked.Exchange(ref lastTick, DateTime.Now.Ticks);
            if (lastSwitches != "100")
            {
                setSwitch("100");
            }
        }
        static void setSwitch(string request)
        {
            if (request == lastSwitches)
                return;

            lastSwitches = request;

            int p1, p2, p3;

            int model, pwrUsbConnected = 0;
            StringBuilder firmware = new StringBuilder(128); ;

            p1 = p2 = p3 = 0;

            p1 = int.Parse(request[0].ToString());
            p2 = int.Parse(request[1].ToString());
            p3 = int.Parse(request[2].ToString());


            Console.WriteLine("Inializing PowerUSB");
            if (PwrUSBWrapper.InitPowerUSB(out model, firmware) > 0)
            {
                Console.Write("PowerUSB Connected. Model:{0:D}  Firmware:", model);
                Console.WriteLine(firmware);
                pwrUsbConnected = PwrUSBWrapper.CheckStatusPowerUSB();
                PwrUSBWrapper.SetPortPowerUSB(p1, p2, p3);
                Console.WriteLine("{0},{1},{2}", p1, p2, p3);
            }
            else
                Console.WriteLine("PowerUSB could not be initialized");
        }
    }
}
