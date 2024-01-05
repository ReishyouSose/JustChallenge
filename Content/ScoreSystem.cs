using JustChallenge.Content.Challenges;
using JustChallenge.Content.Packages;
using JustChallenge.Content.UI;
using JustChallenge.UISupport;
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader.IO;

namespace JustChallenge.Content
{
    public class ScoreSystem : ModSystem
    {
        internal static UISystem uis;
        internal static ModKeybind CheckScore;
        internal static bool first = true;
        internal static int[] scores = new int[255];
        internal static Dictionary<uint, Dictionary<string, int>> scoresData = new();
        internal static Dictionary<byte, int> tempScore;
        internal static int activcPlayer;
        internal static int waitRefreshPlayer;
        internal static int waitResetPlayer;
        internal static int NeedPlayer => activcPlayer > 1 ? Math.Max(2, (int)Math.Round(activcPlayer / 2f)) : 1;
        internal static int[] challenges;
        internal static bool[] complete;
        internal static HashSet<int> completed;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                uis = new();
                uis.Load();
                Main.OnResolutionChanged += (r) =>
                {
                    uis.OnResolutionChange();
                    uis.Calculation();
                };
            }
            else
            {
                challenges = new int[3];
                complete = new bool[3];
                completed = new();
            }
            tempScore = new();
            CheckScore = KeybindLoader.RegisterKeybind(Mod, "L", Keys.L);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            uis?.Update(gameTime);
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            uis?.Draw(spriteBatch);
        }
        public override void PreSaveAndQuit()
        {
            SyncQuit.Send((byte)Main.myPlayer, ChallengeTable.CUI.Wating, ScoreTable.SUI.Wating);
        }
        public override void SaveWorldData(TagCompound tag)
        {
            if (scoresData.Any())
            {
                TagCompound u = new();
                foreach ((uint uid, var data) in scoresData)
                {
                    TagCompound d = new();
                    foreach (var (name, score) in data)
                    {
                        d[name] = score;
                    }
                    u[uid.ToString()] = d;
                }
                tag["scores"] = u;
            }
            tag["challenges"] = challenges;
            TagCompound c = new();
            for (int i = 0; i < 3; i++)
            {
                c[i.ToString()] = complete[i];
            }
            tag["complete"] = c;
            tag["completed"] = completed.ToArray();
        }
        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.TryGet("scores", out TagCompound scores))
            {
                scoresData = new();
                foreach ((string uid, _) in scores)
                {
                    if (scores.TryGet(uid, out TagCompound u))
                    {
                        uint id = uint.Parse(uid);
                        scoresData[id] = new();
                        foreach ((string name, _) in u)
                        {
                            scoresData[id][name] = u.GetInt(name);
                        }
                    }
                }
            }
            challenges = tag.GetIntArray(nameof(challenges));
            completed = tag.GetIntArray(nameof(completed)).ToHashSet();
            if (!challenges.Any())
            {
                challenges = Challenge.RollChallenge();
                complete = new bool[3];
            }
            else if (tag.TryGet("complete", out TagCompound c))
            {
                complete = new bool[3];
                for (int i = 0; i < 3; i++)
                {
                    complete[i] = c.GetBool(i.ToString());
                }
            }
        }
        /// <summary>
        /// 玩家登录时向服务器查找数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int RegisterPlayer(uint uid, string name)
        {
            if (scoresData.TryGetValue(uid, out var data))
            {
                if (data.TryGetValue(name, out int score))
                {
                    return score;
                }
                else
                {
                    data[name] = 0;
                    return 0;
                }
            }
            else
            {
                scoresData[uid] = new() { [name] = 0 };
                return 0;
            }
        }
    }
}

