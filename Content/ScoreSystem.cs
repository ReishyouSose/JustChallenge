using JustChallenge.Content.Packages;
using JustChallenge.UISupport;
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader.IO;

namespace JustChallenge.Content
{
    public class ScoreSystem : ModSystem
    {
        internal static UISystem uis;
        internal static ModKeybind CheckScore;
        internal static Dictionary<uint, Dictionary<string, int>> scoresData;
        internal static Dictionary<byte, int> tempScore;
        internal static Dictionary<byte, bool> tempWaiter;
        internal static HashSet<int> completed;
        internal static bool[] complete;
        internal static int[] challenges;
        internal static int activcPlayer;
        internal static byte admin;
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
                scoresData = new();
                complete = new bool[3];
                completed = new();
                tempWaiter = new();
            }
            tempScore = new();
            CheckScore = KeybindLoader.RegisterKeybind(Mod, "L", Keys.L);
        }
        public override void PostUpdatePlayers()
        {
            if (Main.dedServ)
            {
                byte? needRemove = null;
                foreach (byte whoAmI in tempScore.Keys)
                {
                    if (!Main.player[whoAmI].active)
                    {
                        needRemove = whoAmI;
                        break;
                    }
                }
                if (needRemove >= 0)
                {
                    tempScore.Remove(needRemove.Value);
                    tempWaiter.Remove(needRemove.Value);
                    foreach (byte whoAmI in tempWaiter.Keys)
                    {
                        tempWaiter[whoAmI] = false;
                    }
                    activcPlayer = tempScore.Count;
                    if (activcPlayer == 0) return;
                    if (activcPlayer == 1)
                    {
                        admin = tempScore.Keys.First();
                        ChangeAdmin.Send(admin, true);
                    }
                    SyncScore.Send();
                }
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            uis?.Update(gameTime);
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            uis?.Draw(spriteBatch);
        }
        public override void SaveWorldData(TagCompound tag)
        {
            if (Main.dedServ)
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
        }
        public override void LoadWorldData(TagCompound tag)
        {
            if (Main.dedServ)
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
                if (tag.TryGet("complete", out TagCompound c))
                {
                    complete = new bool[3];
                    for (int i = 0; i < 3; i++)
                    {
                        complete[i] = c.GetBool(i.ToString());
                    }
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

