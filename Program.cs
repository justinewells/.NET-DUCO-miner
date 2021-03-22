using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

//C# DUCO Miner by JustiNewells

namespace DUCOMiner
{
    class Program
    {
        static Socket DUCOSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static IPEndPoint DUCOIP = new IPEndPoint(IPAddress.Parse("51.15.127.80"), 2811);
        static string username;
        static int BAD;
        static int GOOD;
        static int BLOCK;
        static string HASHRATE;

        static string Pre;
        static string Obj;
        static int Diff; 
        static int balcounter = 30; //counter to avoid keeping balance updating
        static string userBal; //stores the user balance
        static void Main(string[] args)
        {
            ShowTitle();
            ConMsg("Trying to connect...");
            try
            {
                DUCOSocket.Connect(DUCOIP);
                byte[] SVerEnc = new byte[255];
                int SVerInt = DUCOSocket.Receive(SVerEnc, 0, SVerEnc.Length, 0);
                Array.Resize(ref SVerEnc, SVerInt);
                string SVer;
                SVer = Encoding.Default.GetString(SVerEnc);
                ConMsg("Connected! Server version: "+SVer);
                Console.SetCursorPosition((Console.WindowWidth - 20) / 2, 8);
                Console.WriteLine("Enter your username:");
                Console.SetCursorPosition((Console.WindowWidth - 20) / 2, 9);
                username = Console.ReadLine();
            }
            catch(InvalidCastException e)
            {
                ShowError("Error while trying connection. Code: "+e);
            }
            DrawBox();
            Mine();
        }

        static void ShowTitle()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition((Console.WindowWidth - 24) / 2, Console.CursorTop);
            Console.WriteLine("╔══════════════════════╗");
            Console.SetCursorPosition((Console.WindowWidth - 24) / 2, Console.CursorTop);
            Console.WriteLine("║    C#  DUCO Miner    ║");
            Console.SetCursorPosition((Console.WindowWidth - 24) / 2, Console.CursorTop);
            Console.WriteLine("║          by          ║");
            Console.SetCursorPosition((Console.WindowWidth - 24) / 2, Console.CursorTop);
            Console.WriteLine("║     JustiNewells     ║");
            Console.SetCursorPosition((Console.WindowWidth - 24) / 2, Console.CursorTop);
            Console.WriteLine("╚══════════════════════╝");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void ShowError(string ErrorMsg)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((Console.WindowWidth - 30) / 2, 15);
            Console.WriteLine("                              ");
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition((Console.WindowWidth - 30) / 2, 16);
            Console.WriteLine("                              ");
            Console.SetCursorPosition((Console.WindowWidth - 30) / 2, 17);
            Console.WriteLine("                              ");
            Console.SetCursorPosition((Console.WindowWidth - 30) / 2, 18);
            Console.WriteLine("                              ");
            
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(((Console.WindowWidth - 30) / 2) + 2, 15);
            Console.WriteLine("ERROR");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(((Console.WindowWidth - 30) / 2) + 2, 17);
            Console.WriteLine(ErrorMsg);
            Console.ReadKey();
            Environment.Exit(0);
        }

        static void ConMsg(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 20);
            Console.Write(new string(' ', Console.WindowWidth)); 
            Console.SetCursorPosition((Console.WindowWidth - 9 - msg.Length) / 2, 20);
            Console.WriteLine("Console: "+msg);
        }

        static void DrawBox()
        {
            Console.Clear();
            Console.SetCursorPosition((Console.WindowWidth - 67) / 2, 7);
            Console.WriteLine("╔═════════════════════════════════════════════════════════════════╗");
            Console.SetCursorPosition((Console.WindowWidth - 67) / 2, 8);
            Console.WriteLine("║                                                                 ║");
            Console.SetCursorPosition((Console.WindowWidth - (12 + username.Length)) / 2, 8);
            Console.WriteLine("Mining for: "+username);
            Console.SetCursorPosition((Console.WindowWidth - 67) / 2, 9);
            Console.WriteLine("╠═════════════════════╦═════════════════════╦═════════════════════╣");
            Console.SetCursorPosition((Console.WindowWidth - 67) / 2, 10);
            Console.WriteLine("║                     ║                     ║                     ║");
            Console.SetCursorPosition((Console.WindowWidth - 67) / 2, 11);
            Console.WriteLine("╠═════════════════════╩══════════╦══════════╩═════════════════════╣");
            Console.SetCursorPosition((Console.WindowWidth - 67) / 2, 12);
            Console.WriteLine("║                                ║                                ║");
            Console.SetCursorPosition((Console.WindowWidth - 67) / 2, 13);
            Console.WriteLine("╚════════════════════════════════╩════════════════════════════════╝");

            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 2, 10);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Bad: ");
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 24, 10);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Good: ");
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 46, 10);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Block: ");
            Console.ForegroundColor = ConsoleColor.White;  

            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 2, 12);
            Console.WriteLine("Balance: ");
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 35, 12);
            Console.WriteLine("Hashrate: ");
        }
        static void UpdateData()
        {
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 11, 12);
            Console.WriteLine("                      ");
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 45, 12);
            Console.WriteLine("                   ");

            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 11, 12);
            Console.WriteLine(GetBalance(username));
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 45, 12);
            Console.WriteLine(HASHRATE);

            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 7, 10);
            Console.WriteLine(BAD);
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 30, 10);
            Console.WriteLine(GOOD);
            Console.SetCursorPosition(((Console.WindowWidth - 67) / 2) + 53, 10);
            Console.WriteLine(BLOCK);
        }
        static void Mine()
        {
            UpdateData();
            ConMsg("Requesting job...");
            try
            {
                byte[] JobReq;
                JobReq = Encoding.Default.GetBytes("JOB," + username + ",MEDIUM");
                DUCOSocket.Send(JobReq, 0, JobReq.Length, 0);
                ConMsg("Job request sent!");
                try
                {
                    byte[] JobRecEnc = new byte[255];
                    int JobRecInt = DUCOSocket.Receive(JobRecEnc, 0, JobRecEnc.Length, 0);
                    Array.Resize(ref JobRecEnc, JobRecInt);
                    string JobRec;
                    JobRec = Encoding.Default.GetString(JobRecEnc);
                    
                    string[] job = JobRec.Split(',');
                    for (int i = 0; i < 3; i++)
                    {
                        job[i] = job[i].Trim();
                    }
                    Pre = job[0];
                    Obj = job[1];
                    bool success = Int32.TryParse(job[2], out int parsediff);
                    Diff = parsediff;
                    ConMsg("Received job!");
                    Calc();
                }
                catch(InvalidCastException e)
                {
                    ShowError("Error receiving job. Code: "+e);
                }
            }
            catch(InvalidCastException e)
            {
                ShowError("Error requesting job. Code: "+e);
            }
        }

        static void Calc()
        {
            ConMsg("Generating and comparing hashes...");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for(int i = 1; i<100*Diff+1; i++)
            {
                string hash;
                using (SHA1 sha1Hash = SHA1.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(Pre+i);
                    var sha1 = SHA1.Create();
                    byte[] hashBytes = sha1.ComputeHash(bytes);
                    hash = HexStringFromBytes(hashBytes);
                }
                if (String.Equals(Obj, hash))
                {
                    watch.Stop();
                    float ElapsedTime = Convert.ToSingle(watch.ElapsedMilliseconds / 1000);
                    double HR = i/ElapsedTime;
                    UpdateHR(HR);

                    try
                    {
                        byte[] Res;
                        Res = Encoding.Default.GetBytes(i + ",,.NET Miner v1.0");
                        DUCOSocket.Send(Res, 0, Res.Length, 0);
                        ConMsg("Number sent! Number: "+i);
                        try
                        {
                            byte[] ResultRecEnc = new byte[255];
                            int ResultRecInt = DUCOSocket.Receive(ResultRecEnc, 0, ResultRecEnc.Length, 0);
                            Array.Resize(ref ResultRecEnc, ResultRecInt);
                            string ResultRec;
                            ResultRec = Encoding.Default.GetString(ResultRecEnc);
                            if(String.Equals(ResultRec, "BAD"))
                            {
                                BAD += 1;
                            }
                            if(String.Equals(ResultRec, "GOOD"))
                            {
                                GOOD += 1;
                            }
                            if(String.Equals(ResultRec, "BLOCK"))
                            {
                                BLOCK += 1;
                            }
                            Mine();
                            ConMsg("Result received! Result: "+ResultRec);
                            break;
                        }
                        catch(InvalidCastException e)
                        {
                            ShowError("Error receiving result. Code: "+e);
                        }
                        break;
                    }
                    catch(InvalidCastException e)
                    {
                        ShowError("Error sending number. Code: "+e);
                        break;
                    }
                }
            }
        }
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static void UpdateHR(double HR)
        {
            if(HR>1000)
            {
                if(HR>1000000)
                {
                    if(HR>1000000000)
                    {
                        HR = HR/1000000000;
                        HR = Math.Round(HR, 2);
                        HASHRATE = (HR + " GH/S"); 
                    }
                    else
                    {
                        HR = HR/1000000;
                        HR = Math.Round(HR, 2);
                        HASHRATE = (HR + " MH/S");
                    }
                }
                else
                {
                    HR = HR/1000;
                    HR = Math.Round(HR, 2);
                    HASHRATE = (HR + " kH/S");
                }
            }
            else
            {
                HR = Math.Round(HR, 2);
                HASHRATE = (HR + " H/S");
            }
        }
        public static string GetBalance(string Name)
        {
            if(balcounter == 30)
            {
                balcounter = 0;
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        var obj = wc.DownloadString("http://51.15.127.80/balances.json");
                        var balJson = JObject.Parse(obj);
                        userBal = balJson[Name].ToString();
                        return userBal;
                    }
                    catch
                    {
                        ShowError("Error reading balance JSon");
                        return null;
                    }
                }
            }
            else
            {
                balcounter += 1;
                return userBal;
            }

        }
    }
}
