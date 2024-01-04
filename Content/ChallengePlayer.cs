using JustChallenge.Content.Challenges;
using JustChallenge.Content.Packages;
using JustChallenge.Content.UI;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace JustChallenge.Content
{
    public class ChallengePlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            FirstRequest.Send(UserID, Player.name, Player.whoAmI);
        }
        public override void UpdateEquips()
        {
            if (Main.netMode == NetmodeID.Server) return;
            foreach (Item equip in Player.armor)
            {
                switch (equip.type) 
                {
                    case ItemID.PlatinumChainmail:
                    case ItemID.GoldChainmail:
                        ChallengeTable.TryComplete(ChallengeID.穿金胸甲);
                        break;
                }
            }
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Main.netMode == NetmodeID.Server) return;
            switch (damageSource.SourceOtherIndex)
            {
                case 1: ChallengeTable.TryComplete(ChallengeID.淹死); break;
                case 2: ChallengeTable.TryComplete(ChallengeID.死于岩浆); break;
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            ScoreTable SUI = UIS?[ScoreTable.Namekey] as ScoreTable;
            SUI.Info.IsVisible = ScoreSystem.CheckScore.Current;
        }
    }
}
