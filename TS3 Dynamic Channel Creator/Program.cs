using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using TS3Client;
using TS3Client.Full;
using TS3Client.Messages;
using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TS3Client;
using TS3Client.Helper;
using TS3Client.Full;
using TS3Client.Messages;
using ClientUidT = System.String;
using ClientDbIdT = System.UInt64;
using ClientIdT = System.UInt16;
using ChannelIdT = System.UInt64;
using ServerGroupIdT = System.UInt64;
using ChannelGroupIdT = System.UInt64;

namespace TS3_Dynamic_Channel_Creator
{
    class Program
    {
        public static IniData cfg;
        static ConnectionDataFull con;
        private static string cfgfile = "config.cfg";
        private static string gen = "general";
        private static string chan = "channel";
        private static string perm = "permissions";
        private static Ts3FullClient client = new Ts3FullClient(EventDispatchType.AutoThreadPooled);
        private static bool exit = false;
        static void Dispose()
        {
            exit = true;
            if (client.Connected)
            {
                client.OnConnected -= OnConnected;
                client.OnDisconnected -= OnDisconnected;
                client.OnErrorEvent -= OnErrorEvent;
                client?.Disconnect();
            }
            Console.ReadLine();
            //Console.ReadKey(true);
            Environment.Exit(0);
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Dispose();
        }
        static void LoadConfig(string file)
        {
            var parser = new FileIniDataParser();
            if (!File.Exists(file))
            {
                cfg = new IniData();
                var g = cfg[gen]; var c = cfg[chan]; var p = cfg[perm];
                g["QuitMessage"] = "Dynamic Channel Creator by Bluscream";
                g["Address"] = string.Empty;
                g["NickName"] = string.Empty;
                g["Identity"] = Ts3Crypt.GenerateNewIdentity().PrivateKeyString;
                g["Public Channels"] = "";
                c["Description"] = "";
                c["Name"] = "";
                c["Phonetic name"] = "";
                c["Topic"] = "";
                c["Codec"] = "";
                c["Codec quality"] = "10";
                c["Codec latency"] = "";
                c["Codec latency"] = "";
                c["Password"] = "";
                c["Delete delay"] = "";
                c["Max clients"] = "";
                c["Max family clients"] = "";
                c["type"] = "permanent";
                p["i_icon_id"] = "0";
                parser.WriteFile(file, cfg);
                Console.WriteLine("\"{0}\" created. Edit it and restart the bot!", file);
                Dispose();
            }
            cfg = parser.ReadFile(cfgfile);
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Console.CancelKeyPress += (s, e) =>
            {
                if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                {
                    e.Cancel = true;
                    Dispose();
                }
            };
            LoadConfig(cfgfile);
            client.QuitMessage = cfg[gen]["QuitMessage"];
            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnected;
            client.OnErrorEvent += OnErrorEvent;
            con = new ConnectionDataFull
            {
                Address = cfg[gen]["Address"],
                Username = cfg[gen]["NickName"],
                VersionSign = VersionSign.VER_WIN_3_1_8
            };
            var id = Ts3Crypt.LoadIdentityDynamic(cfg[gen]["Identity"]).Unwrap();
            Console.WriteLine("ID: {0}", id.PrivateKeyString);
            con.Identity = id;
            //Console.WriteLine("Connecting to {0} as \"{1}\" (UID: {2})", client.ConnectionData.Address, con.Username, con.Identity.ClientUid);
            client.Connect(con);
        }

        List<ChannelIdT> currentChannels;

        private static void OnClientMoved(object sender, IEnumerable<ClientMoved> e)
        {
            throw new NotImplementedException();
        }

        private static void OnChannelCreated(object sender, IEnumerable<ClientLeftView> e)
        {
            throw new NotImplementedException();
        }

        private static void OnChannelDeleted(object sender, IEnumerable<ClientEnterView> e)
        {
            throw new NotImplementedException();
        }

        private static void OnErrorEvent(object sender, CommandError e)
        {
            throw new NotImplementedException();
        }

        private static void OnDisconnected(object sender, DisconnectEventArgs e)
        {
            var client = (Ts3FullClient)sender;
            Console.WriteLine("Disconnected from {0} with clid {1}", client.ConnectionData.Address, client.ClientId);
            if (!exit) client.Connect(con);

        }

        private static void OnConnected(object sender, EventArgs e)
        {
            var client = (Ts3FullClient)sender;
            Console.WriteLine("Connected to {0} with clid {1}", client.ConnectionData.Address, client.ClientId);
        }
    }
}
