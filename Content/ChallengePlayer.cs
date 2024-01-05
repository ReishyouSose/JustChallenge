using JustChallenge.Content.Challenges;
using JustChallenge.Content.Packages;
using JustChallenge.Content.UI;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace JustChallenge.Content
{
    public class ChallengePlayer : ModPlayer
    {
        public static void Complete(Dictionary<int, int> target, int type)
        {
            if (target.TryGetValue(type, out int id))
            {
                ChallengeTable.TryComplete(id);
            }
        }
        public override void OnEnterWorld()
        {
            FirstRequest.Send(UserID, Player.name, Player.whoAmI);
        }
        public override void UpdateEquips()
        {
            if (Player.whoAmI != Main.myPlayer) return;
            for (int i = 0; i < 10; i++)
            {
                Complete(ChallengeID.equip, Player.armor[i].type);
            }
            int type = Player.HeldItem.type;
            Complete(ChallengeID.heldItem, type);
            if (Player.chest > -1)
            {
                Complete(ChallengeID.takeItem, type);
            }
        }
        public override void PreUpdateMovement()
        {
            if (Player.whoAmI != Main.myPlayer) return;
            Point pos = ((Player.Center + new Vector2(0, Player.height / 2 + 8)) / 16f).ToPoint();
            Tile tile = Main.tile[pos.X, pos.Y];
            if (tile.HasTile)
            {
                Complete(ChallengeID.onTile, tile.TileType);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
            {
                Complete(ChallengeID.killNPC, target.type);
                if ((target.type == NPCID.EaterofWorldsTail) && target.boss)
                {
                    ChallengeTable.TryComplete(ChallengeID.击败克苏鲁之脑);
                }
            }
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Player.whoAmI != Main.myPlayer) return;
            if (damageSource.SourceOtherIndex > -1)
            {
                Complete(ChallengeID.deathByOther, damageSource.SourceOtherIndex);
            }
            if (damageSource.SourceNPCIndex > NPCID.None)
            {
                Complete(ChallengeID.killByNPC, Main.npc[damageSource.SourceNPCIndex].type);
            }
        }
        public override void ModifyCaughtFish(Item fish)
        {
            Complete(ChallengeID.fishItem, fish.type);
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            ScoreTable SUI = UIS?[ScoreTable.Namekey] as ScoreTable;
            SUI.Info.IsVisible = ScoreSystem.CheckScore.Current;
        }
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            Complete(ChallengeID.buyItem, item.type);
        }
        public override bool OnPickup(Item item)
        {
            if (Player.HeldItem.type is 1991 or 4821 or 3183)
            {
                Complete(ChallengeID.capture, item.type);
            }
            Complete(ChallengeID.pickItem, item.type);
            return base.OnPickup(item);
        }
    }
}
