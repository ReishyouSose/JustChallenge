using JustChallenge.Content.UI;
using NetSimplified;
using System.IO;

namespace JustChallenge.Content.Packages
{
    /// <summary>
    /// 用于玩家登录时从服务器获取当前挑战
    /// </summary>
    internal class SyncChallenges : NetModule
    {
        private byte index;
        private int[] ids;
        private bool[] complete;
        /// <summary>
        /// 0-2是单独刷新，3全完成，4非全完成
        /// </summary>
        public static void Send(byte index, int toClient = -1)
        {
            var p = NetModuleLoader.Get<SyncChallenges>();
            p.index = index;
            p.Send(toClient);
        }
        public override void Send(ModPacket p)
        {
            p.Write(index);
            if (index != 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    p.Write(ScoreSystem.challenges[i]);
                    p.Write(ScoreSystem.complete[i]);
                }
            }
        }
        public override void Read(BinaryReader r)
        {
            index = r.ReadByte();
            if (index != 3)
            {
                ids = new int[3];
                complete = new bool[3];
                for (int i = 0; i < 3; i++)
                {
                    ids[i] = r.ReadInt32();
                    complete[i] = r.ReadBoolean();
                }
            }
        }
        public override void Receive()
        {
            ChallengeTable.CUI.SyncChallenges(index, ids, complete);
        }
    }
}
