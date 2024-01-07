using JustChallenge.Content.UI;
using NetSimplified;
using System.IO;

namespace JustChallenge.Content.Packages
{
    internal class SyncScore : NetModule
    {
        private byte[] players;
        private int[] scores;
        public static void Send()
        {
            var p = NetModuleLoader.Get<SyncScore>();
            var data = ScoreSystem.tempScore;
            Console.WriteLine($"写入玩家数据，{data.Count}人");
            p.players = data.Keys.ToArray();
            p.scores = data.Values.ToArray();
            p.Send(-1, -1);
        }
        public override void Send(ModPacket p)
        {
            byte count = (byte)players.Length;
            p.Write(count);
            for (int i = 0; i < count; i++)
            {
                p.Write(players[i]);
                p.Write(scores[i]);
            }
        }
        public override void Read(BinaryReader r)
        {
            int count = r.ReadByte();
            players = new byte[count];
            scores = new int[count];
            for (int i = 0; i < count; i++)
            {
                players[i] = r.ReadByte();
                scores[i] = r.ReadInt32();
            }
        }
        public override void Receive()
        {
            int count = players.Length;
            //Main.NewText($"接收来自服务器的分数,{count}人");

            ScoreSystem.tempScore = new();
            for (int i = 0; i < count; i++)
            {
                ScoreSystem.tempScore[players[i]] = scores[i];
            }
            ScoreSystem.activcPlayer = count;
            ScoreTable.SUI.RefreshWaiter(false);
            ScoreTable.SUI.RefreshScore(ScoreSystem.admin);
        }
    }
}
