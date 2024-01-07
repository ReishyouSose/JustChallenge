using Terraria.Localization;

namespace JustChallenge.Content.Challenges
{
    public abstract class Challenge : ModType
    {
        internal static HashSet<int> ignore = new() { 4, 18 };
        public bool IsComplete;
        private readonly HashSet<int> depends;
        public float Width => FontAssets.MouseText.Value.MeasureString(Description.Value).X;
        public readonly int type = -1;
        public readonly Color color = Color.White;
        public readonly int equip = -1;
        public readonly int killByNPC = -1;
        public readonly int create = -1;
        public readonly int killNPC = -1;
        public readonly int buyItem = -1;
        public readonly int pickItem = -1;
        public readonly int onTile = -1;
        public readonly int heldItem = -1;
        public readonly int fishItem = -1;
        public readonly int capture = -1;
        public readonly int takeItem = -1;
        public readonly int deathByOther = -1;
        public Challenge()
        {
            SetStaticDefault(ref type, ref color, ref equip, ref killByNPC, ref create, ref killNPC, ref buyItem,
                ref pickItem, ref onTile, ref heldItem, ref fishItem, ref capture, ref takeItem, ref deathByOther);
            depends = new();
            SetDepends(depends);
            ChallengeID.challenges.Add(type, this);
        }
        public virtual void SetDepends(HashSet<int> depends) { }
        public Challenge Copy(bool isComplete)
        {
            IsComplete = isComplete;
            return this;
        }
        public static Challenge NewChallenge(int id, bool isComplete) => ChallengeID.challenges[id].Copy(isComplete);
        public abstract void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC,
            ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem,
            ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther);
        public virtual LocalizedText Description => Language.GetText($"Mods.JustChallenge.Challenges.{GetType().Name}");
        public virtual void Draw(SpriteBatch spb, Vector2 pos)
        {
            ChatManager.DrawColorCodedStringWithShadow(spb, FontAssets.MouseText.Value, Description.Value,
               pos, color.SetAlpha(200), 0, Vector2.Zero, Vector2.One, spread: 1);
        }
        public virtual void ExtraCondition() { }
        protected override void Register() { }
        public override void SetupContent()
        {
            if (equip > ItemID.None) ChallengeID.equip.Add(equip, type);
            if (killByNPC > NPCID.None) ChallengeID.killByNPC.Add(killByNPC, type);
            if (create > ItemID.None) ChallengeID.create.Add(create, type);
            if (killNPC > NPCID.None) ChallengeID.killNPC.Add(killNPC, type);
            if (buyItem > ItemID.None) ChallengeID.buyItem.Add(buyItem, type);
            if (pickItem > ItemID.None) ChallengeID.pickItem.Add(pickItem, type);
            if (onTile > ItemID.None) ChallengeID.onTile.Add(onTile, type);
            if (heldItem > ItemID.None) ChallengeID.heldItem.Add(heldItem, type);
            if (fishItem > ItemID.None) ChallengeID.fishItem.Add(fishItem, type);
            if (capture > ItemID.None) ChallengeID.capture.Add(capture, type);
            if (takeItem > ItemID.None) ChallengeID.takeItem.Add(takeItem, type);
            if (deathByOther > -1) ChallengeID.deathByOther.Add(deathByOther, type);
            ExtraCondition();
        }
        public static int[] RollChallenge(byte? index = null, int except = -1)
        {
            List<int> canRoll = new();
            for (int i = 0; i < ChallengeID.challenges.Count; i++)
            {
                if (ignore.Contains(i) || ScoreSystem.completed.Contains(i) || i == except) continue;
                bool unlock = true;
                foreach (int depend in ChallengeID.challenges[i].depends)
                {
                    if (!ScoreSystem.completed.Contains(depend))
                    {
                        unlock = false;
                        break;
                    }
                }
                if (unlock) canRoll.Add(i);
            }
            if (!canRoll.Any()) return null;
            canRoll.Shuffle();
            int count = canRoll.Count;
            if (index.HasValue && count > 1)
            {
                int[] result = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    if (i == index.Value)
                    {
                        int r = canRoll[i];
                        while (ScoreSystem.challenges.Contains(r))
                        {
                            r = canRoll[Main.rand.Next(count)];
                        }
                        result[i] = r;
                        ScoreSystem.complete[i] = false;
                    }
                    else result[i] = ScoreSystem.challenges[i];
                }
                return result;
            }
            ScoreSystem.complete = new bool[3];
            while (count < 3) canRoll.Add(-1);
            Console.WriteLine(("随机挑战", canRoll[0], canRoll[1], canRoll[2]));
            return canRoll.GetRange(0, 3).ToArray();
        }
        public static void CheckFirst()
        {
            ScoreSystem.challenges ??= RollChallenge();
        }
    }
    public class 淹死 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 0;
            color = new(0, 0, 255);
            deathByOther = DeathReasonID.Drowned;
        }

    }
    public class 死于岩浆 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 1;
            color = new(229, 76, 94);
            deathByOther = DeathReasonID.Lava;
        }
    }
    public class 穿金胸甲 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 2;
            color = new(255, 192, 0);
            equip = ItemID.GoldChainmail;
        }
        public override void ExtraCondition() => ChallengeID.equip.Add(ItemID.PlatinumChainmail, type);
    }
    internal class 摔死 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 3;
            color = new(117, 189, 66);
            deathByOther = DeathReasonID.Fell;
        }
    }
    public class 在微光湖分解物品 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 4;
            color = new(112, 48, 160);
        }
    }
    public class 制作出草剑 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 5;
            color = new(24, 96, 90);
            create = ItemID.BladeofGrass;
        }
    }
    public class 成功打败骷髅王 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 6;
            color = new(229, 76, 94);
        }
    }
    public class 获得一个地牢蜡烛 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 7;
            color = new(0, 112, 192);
            pickItem = ItemID.WaterCandle;
        }
        public override void SetDepends(HashSet<int> depends) => depends.Add(ChallengeID.成功打败骷髅王);
    }
    public class 钓出天空匣 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 8;
            color = new(87, 141, 49);
            fishItem = ItemID.FloatingIslandFishingCrate;
        }
        public override void ExtraCondition() => ChallengeID.fishItem.Add(ItemID.FloatingIslandFishingCrateHard, type);
    }
    public class 做出蓝玉宝石钩 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 9;
            color = new(72, 116, 203);
            create = ItemID.SapphireHook;
        }
    }
    public class 获得僵尸旗帜 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 10;
            color = new(117, 189, 66);
            pickItem = ItemID.ZombieBanner;
        }
    }
    public class 被史莱姆杀死 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 11;
            color = new(72, 116, 203);
            killByNPC = NPCID.BlueSlime;
        }
        public override void ExtraCondition()
        {
            int[] slimes = new int[] { 1, 16, 59, 71, 81, 138, 121, 122, 141, 147, 183, 184,
            204, 225, 244, 302, 333, 335, 334, 336, 537, 676, 667};
            foreach (int slime in slimes)
            {
                ChallengeID.killByNPC[slime] = type; ;
            }
        }
    }
    public class 站在花岗岩上 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 12;
            color = new(8, 43, 124);
            onTile = TileID.Granite;
        }
    }
    public class 手持丛林火把 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 13;
            color = new(117, 189, 66);
            heldItem = ItemID.JungleTorch;
        }
    }
    public class 获得蜂巢 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 14;
            color = new(197, 94, 16);
            pickItem = ItemID.Hive;
        }
    }
    public class 制作出床 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 15;
            color = Color.Black;
            create = ItemID.Bed;
        }
        public override void ExtraCondition()
        {
            List<int> beds = new();
            foreach (var (type, item) in ContentSamples.ItemsByType)
            {
                if (item.createTile == TileID.Beds)
                {
                    beds.Add(type);
                }
            }
            foreach (int id in beds) ChallengeID.create[id] = type; ;
        }
    }
    public class 获得黑曜石 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 16;
            color = new(112, 48, 160);
            pickItem = ItemID.Obsidian;
        }
    }
    public class 成功打败史莱姆王 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 17;
            color = new(45, 84, 160);
            killNPC = NPCID.KingSlime;
        }
    }
    public class 炸一个猩红之地的心脏 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 18;
            color = new(229, 76, 94);
        }
    }
    public class 购买一个猪猪存钱罐 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 19;
            color = new(224, 183, 190);
            buyItem = ItemID.PiggyBank;
        }
    }
    public class 制作一个史莱姆块 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 20;
            color = new(72, 116, 203);
            create = ItemID.SlimeBlock;
        }
    }
    public class 被鸟妖杀死 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 21;
            color = new(72, 116, 203);
            killByNPC = NPCID.Harpy;
        }
    }
    public class 被鲨鱼咬死 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 22;
            color = new(0, 176, 240);
            killByNPC = NPCID.Shark;
        }
    }
    public class 窒息死亡 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 23;
            color = new(242, 186, 2);
            deathByOther = DeathReasonID.Suffocated;
        }
    }
    public class 打败克苏鲁之眼 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 24;
            color = new(199, 28, 49);
            killNPC = NPCID.EyeofCthulhu;
        }
    }
    public class 打败肉山 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 25;
            color = new(199, 28, 49);
            killNPC = NPCID.WallofFlesh;
        }
    }
    public class 获得黑曜石药水 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 26;
            color = new(112, 48, 160);
            buyItem = pickItem = create = ItemID.ObsidianSkinPotion;
        }
    }
    public class 造出仙人掌门 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 27;
            color = new(117, 89, 66);
            create = ItemID.CactusDoor;
        }
    }
    public class 造出蘑菇门 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 28;
            color = new(0, 112, 192);
            create = ItemID.MushroomDoor;
        }
    }
    public class 制作雪球 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 29;
            color = Color.White;
            create = ItemID.Snowball;
        }
    }
    public class 造出史莱姆门 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 30;
            color = new(45, 84, 160);
            create = ItemID.SlimeDoor;
        }
    }
    public class 抓一只沙漠黄蝎子 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 31;
            color = new(181, 139, 1);
            capture = ItemID.Scorpion;
        }
    }
    public class 获得标尺 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 32;
            color = new(255, 192, 0);
            buyItem = ItemID.Ruler;
        }
    }
    public class 制作铂金弓 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 33;
            color = new(145, 171, 223);
            create = ItemID.PlatinumBow;
        }
        public override void ExtraCondition() => ChallengeID.create.Add(ItemID.GoldBow, type);
    }
    public class 获得村正大刀 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 34;
            color = new(0, 112, 192);
            takeItem = ItemID.Muramasa;
        }
    }
    public class 制作太空枪 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 35;
            color = new(197, 94, 16);
            create = ItemID.SpaceGun;
        }
    }
    public class 杀死一只兔兔 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 36;
            color = new(244, 183, 190);
            killNPC = NPCID.Bunny;
        }
    }
    public class 击败克苏鲁之脑 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 37;
            color = new(199, 28, 49);
            killNPC = NPCID.BrainofCthulhu;
        }
    }
    public class 被蚁狮马杀死 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 38;
            color = new(181, 139, 1);
            killByNPC = NPCID.WalkingAntlion;
        }
        public override void ExtraCondition() => ChallengeID.killByNPC.Add(NPCID.GiantWalkingAntlion, type);
    }
    public class 制作猩红胸甲 : Challenge
    {
        public override void SetStaticDefault(ref int type, ref Color color, ref int equip, ref int killByNPC, ref int create, ref int killNPC, ref int buyItem, ref int pickItem, ref int onTile, ref int heldItem, ref int fishItem, ref int capture, ref int takeItem, ref int deathByOther)
        {
            type = 39;
            color = new(255, 0, 0);
            create = ItemID.CrimsonScalemail;
        }
        public override void ExtraCondition() => ChallengeID.create.Add(ItemID.ShadowScalemail, type);
        public override void SetDepends(HashSet<int> depends) => depends.Add(ChallengeID.击败克苏鲁之脑);
    }
}
