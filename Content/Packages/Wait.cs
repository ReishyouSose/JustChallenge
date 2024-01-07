using JustChallenge.Content.Challenges;
using JustChallenge.Content.UI;
using NetSimplified;
using System.IO;
using static JustChallenge.Content.ScoreSystem;

namespace JustChallenge.Content.Packages
{
    internal class Wait : NetModule
    {
        private byte whoAmI;
        private byte count;
        private bool success;
        public static void Send(byte whoAmI, byte count = 0, bool success = false)
        {
            var p = NetModuleLoader.Get<Wait>();
            p.whoAmI = whoAmI;
            p.count = count;
            p.success = success;
            p.Send();
        }
        public override void Send(ModPacket p)
        {
            p.Write(whoAmI);
            if (Main.dedServ)
            {
                p.Write(success);
                if (!success)
                {
                    p.Write(count);
                }
            }
        }
        public override void Read(BinaryReader r)
        {
            whoAmI = r.ReadByte();
            if (!Main.dedServ)
            {
                success = r.ReadBoolean();
                count = success ? (byte)0 : r.ReadByte();
            }
        }
        public override void Receive()
        {
            if (Main.dedServ)
            {
                tempWaiter[whoAmI] = !tempWaiter[whoAmI];
                Console.WriteLine($"{whoAmI}号玩家{(tempWaiter[whoAmI] ? "发起" : "撤销")}重置请求");
                byte waiter = (byte)tempWaiter.Values.Count(x => x);
                Console.WriteLine("等待人数" + waiter);
                if (waiter >= activcPlayer)
                {
                    completed = new();
                    foreach ((uint uid, var data) in scoresData)
                    {
                        foreach (string name in data.Keys)
                        {
                            data[name] = 0;
                        }
                    }
                    foreach (byte whoAmI in tempScore.Keys)
                    {
                        tempScore[whoAmI] = 0;
                        tempWaiter[whoAmI] = false;
                    }
                    challenges = Challenge.RollChallenge();
                    SyncChallenges.Send(4);
                    SyncScore.Send();
                    Send(whoAmI, 0, true);
                }
                else Send(whoAmI, waiter, false);
            }
            else
            {
                int old = ScoreTable.waiter;
                ScoreTable.waiter = count;
                if (Main.myPlayer != whoAmI)
                {
                    Main.NewText($"玩家 {Main.player[whoAmI].name} {(count > old || success ? "发起" : "撤销")}重置请求");
                }
                if (success) Main.NewText("所有挑战已重置");
                ScoreTable.SUI.RefreshWaiter(success);
            }
        }
    }
}
