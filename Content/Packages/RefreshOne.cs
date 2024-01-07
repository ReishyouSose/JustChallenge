using JustChallenge.Content.Challenges;
using NetSimplified;
using NetSimplified.Syncing;
using static JustChallenge.Content.ScoreSystem;

namespace JustChallenge.Content.Packages
{
    [AutoSync]
    internal class RefreshOne : NetModule
    {
        private byte index;
        public static void Send(byte index)
        {
            var p = NetModuleLoader.Get<RefreshOne>();
            p.index = index;
            p.Send();
        }
        public override void Receive()
        {
            challenges = Challenge.RollChallenge(index, challenges[index]);
            complete[index] = false;
            SyncChallenges.Send(index);
        }
    }
}
