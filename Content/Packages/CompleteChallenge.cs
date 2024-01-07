using JustChallenge.Content.Challenges;
using JustChallenge.Content.UI;
using NetSimplified;
using NetSimplified.Syncing;
using static JustChallenge.Content.ScoreSystem;

namespace JustChallenge.Content.Packages
{
    [AutoSync]
    internal class CompleteChallenge : NetModule
    {
        private byte index;
        private byte whoAmI;
        private uint uid;
        private string PlayerName => Main.player[whoAmI].name;
        public static void Send(byte index, byte whoAmI, uint uid)
        {
            var p = NetModuleLoader.Get<CompleteChallenge>();
            p.index = index;
            p.whoAmI = whoAmI;
            p.uid = uid;
            p.Send();
        }
        public override void Receive()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                if (!complete[index])
                {
                    Console.WriteLine($"{whoAmI}号玩家完成{index}号挑战");
                    completed.Add(challenges[index]);
                    complete[index] = true;
                    scoresData[uid][PlayerName]++;
                    tempScore[whoAmI]++;
                    SyncScore.Send();
                    Send();
                }
                if (!complete.Contains(false))
                {
                    Console.WriteLine("三个挑战均已完成，请求刷新挑战");
                    challenges = Challenge.RollChallenge();
                    SyncChallenges.Send(4);
                }
            }
            else
            {
                Main.NewText($"{index + 1}号挑战已被玩家 {PlayerName} 完成");
                ChallengeTable.ChangeState(index);
            }
        }
    }
}
