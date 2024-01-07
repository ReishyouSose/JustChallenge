using JustChallenge.Content.UI;
using NetSimplified;
using NetSimplified.Syncing;

namespace JustChallenge.Content.Packages
{
    [AutoSync]
    public class ChangeAdmin : NetModule
    {
        private byte admin;
        private bool report;
        public static void Send(byte admin, bool report = false)
        {
            var p = NetModuleLoader.Get<ChangeAdmin>();
            p.admin = admin;
            p.report = report;
            p.Send();
        }
        public override void Receive()
        {
            if (Main.dedServ)
            {
                ScoreSystem.admin = admin;
                Console.WriteLine("管理员变更：" + Main.player[admin].name);
                Send(admin, report);
            }
            else
            {
                if (report) Main.NewText("管理员变更为 " + Main.player[admin].name);
                ScoreTable.SUI.RefreshScore(admin);
            }
        }
    }
}
