using System;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.GameContent.Creative;

namespace NewbeeProtection
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        public override string Author => "Dr.Toxic";// 插件作者
        public override string Description => "注册后指定时间内无敌";// 插件说明
        public override string Name => "Newbee Protection";// 插件名字
        public override Version Version => new Version(1, 0, 0, 0);// 插件版本

        // 存取配置文件
        public static ConfigFile GodConfig { get; set; }
        internal static string GodConfigPath { get { return Path.Combine(TShock.SavePath, "新手无敌时间.json"); } }

        // 变量定义
        static readonly Timer Update = new Timer(1000);

        public MainPlugin(Main game) : base(game) //插件处理
        {
            GodConfig = new ConfigFile();
        }

        public override void Initialize()
        {
            Update.Elapsed += OnUpdate;
            Update.Start();

            ReadConfig();
            Commands.ChatCommands.Add(new Command("NewbeeProtection.reload", CMD, "nbpreload"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Update.Elapsed -= OnUpdate;
                Update.Stop();
            }
            base.Dispose(disposing);
        }

        private void OnUpdate(object sender, ElapsedEventArgs e)
        {
            foreach (var player in TShock.Players)
            {
                if (player.HasPermission(Permissions.bypassssc))
                {
                    continue;
                }
                if (player.Active && !player.GodMode &&
                    (DateTime.Now - DateTime.Parse(player.Account.Registered).ToLocalTime()).TotalMinutes <= GodConfig.GodTime && NPC.downedBoss3)
                {
                    player.GodMode = !player.GodMode;
                    var godPower = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
                    godPower.SetEnabledState(player.Index, player.GodMode);
                    player.SendSuccessMessage($"你有{GodConfig.GodTime - (int)Math.Floor((DateTime.Now - DateTime.Parse(player.Account.Registered).ToLocalTime()).TotalMinutes)}分钟的新手保护时间！");
                }
                if ((DateTime.Now - DateTime.Parse(player.Account.Registered).ToLocalTime()).TotalMinutes > GodConfig.GodTime &&
                    (DateTime.Now - DateTime.Parse(player.Account.Registered).ToLocalTime()).TotalMinutes < GodConfig.GodTime + 1
                    && player.GodMode)
                {
                    player.GodMode = !player.GodMode;
                    var godPower = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
                    godPower.SetEnabledState(player.Index, player.GodMode);
                    player.SendInfoMessage("你的新手保护时间已结束！");
                }
            }
        }

        private void ReadConfig()
        {
            try
            {
                if (!File.Exists(GodConfigPath))
                    TShock.Log.ConsoleError("未找到新手无敌时间配置，已为您创建！"); // 检测提示
                GodConfig = ConfigFile.Read(GodConfigPath); // 读取配置并且自动补全配置
                GodConfig.Write(GodConfigPath);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("新手无敌时间配置读取错误:" + ex.ToString());
            }
        }

        private void CMD(CommandArgs args)
        {
            ReadConfig();
            args.Player.SendSuccessMessage("重读新手无敌时间");
        }
    }
}
