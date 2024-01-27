using JustChallenge.Content.UI;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace JustChallenge
{
    public class ChallengeConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Slider]
        [Range(0, 1)]
        public int UISwitchType;
        public override void OnChanged()
        {
            ScoreTable.UISwitchType = UISwitchType;
        }
    }
}
