//using JustChallenge.UISupport;
using NetSimplified;
using System.IO;

namespace JustChallenge
{
    public class JustChallenge : Mod
    {
        public override void Load()
        {
            AddContent<NetModuleLoader>();
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);
    }
}