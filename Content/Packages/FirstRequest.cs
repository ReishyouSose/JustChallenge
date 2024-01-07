using JustChallenge.Content.Challenges;
using NetSimplified;
using NetSimplified.Syncing;
using static JustChallenge.Content.ScoreSystem;

namespace JustChallenge.Content.Packages
{
    [AutoSync]
    internal class FirstRequest : NetModule
    {
        private uint uid;
        private string name;
        private byte whoAmI;
        public static void Send(uint uid, string name, int whoAmi)
        {
            var p = NetModuleLoader.Get<FirstRequest>();
            p.uid = uid;
            p.name = name;
            p.whoAmI = (byte)whoAmi;
            p.Send();
        }
        public override void Receive()
        {
            Console.WriteLine($"收到{whoAmI}号玩家登录数据请求");
            tempScore[whoAmI] = RegisterPlayer(uid, name);
            tempWaiter[whoAmI] = false;
            activcPlayer = tempScore.Count;
            if (activcPlayer == 1)
            {
                admin = whoAmI;
                ChangeAdmin.Send(admin);
            }
            Challenge.CheckFirst();
            SyncScore.Send();
            SyncChallenges.Send((byte)(challenges == null ? 3 : 4), whoAmI);
        }
    }
}
