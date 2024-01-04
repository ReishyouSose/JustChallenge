﻿using JustChallenge.Content.Challenges;
using JustChallenge.Content.Packages;

namespace JustChallenge.Content.UI
{
    public class ChallengeTable : ContainerElement
    {
        internal static ChallengeTable CUI => UIS?[Namekey] as ChallengeTable;
        internal static string Namekey = "JustChallenge.Content.UI.ChallengeTable";
        public Challenge[] challenges;
        public UIImage bg;
        private bool needReCal;
        private bool wait;
        public UIText refresh;
        public override void OnInitialization()
        {
            base.OnInitialization();

            bg = new(T2D("JustChallenge/UISupport/Asset/ClothesStyleBack"));
            bg.SetPos(20, -bg.Height / 2f, 0, 0.5f);
            bg.color = Color.White * 0.5f;
            Register(bg);

            challenges = new Challenge[3];

            refresh = new("发起刷新请求", drawStyle: 0);
            refresh.SetPos(10, 10);
            refresh.SetSize(refresh.TextSize);
            refresh.Events.OnLeftDown += evt =>
            {
                Wait.Send((byte)Main.LocalPlayer.whoAmI, wait);
                wait = !wait;
                ScoreSystem.waitPlayer += wait ? 1 : -1;
                RefreshWaiter();
            };
            bg.Register(refresh);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (Main.netMode == NetmodeID.SinglePlayer) Info.IsVisible = false;
            if (needReCal)
            {
                bg.Info.Width.Pixel = 20 + Math.Max(refresh.TextSize.X,
                    challenges.Select(x => FontAssets.MouseText.Value.MeasureString(x.Description.Value).X).Max());
                bg.Calculation();
                needReCal = false;
            }
        }
        public override void DrawChildren(SpriteBatch sb)
        {
            base.DrawChildren(sb);
            if (challenges.Any())
            {
                Vector2 pos = bg.HitBox(false).TopLeft() + new Vector2(10, 10);
                int x = bg.Width - 20;
                for (int i = 0; i < challenges.Length; i++)
                {
                    Challenge challenge = challenges[i];
                    challenge.Draw(sb, pos.X, pos.Y + 3 + 28 * (i + 1));
                    if (challenge.IsComplete)
                    {
                        sb.Draw(TextureAssets.MagicPixel.Value, new Vector2(pos.X, pos.Y + 12 + 28 * (i + 1)),
                            new Rectangle(0, 0, x, 1), Color.White, 0, Vector2.Zero, new Vector2(1, 2), 0, 0);
                    }
                }
            }
        }
        public void RefreshChallenges(int[] ids)
        {
            for (int i = 0; i < 3; i++)
            {
                challenges[i] = Challenge.NewChallenge(ids[i], false);
            }
            wait = false;
            RefreshWaiter();
        }
        public static void SyncChallenges(int[] ids, bool[] complete)
        {
            CUI.challenges = new Challenge[3];
            for (int i = 0; i < 3; i++)
            {
                CUI.challenges[i] = Challenge.NewChallenge(ids[i], complete[i]);
            }
            CUI.needReCal = true;
        }
        public static void ChangeState(byte index)
        {
            CUI.challenges[index].IsComplete = true;
        }
        public void RefreshWaiter()
        {
            refresh.ChangeText((wait ? "撤销刷新请求" : "发起刷新请求")
                + $" ({ScoreSystem.waitPlayer}/{ScoreSystem.NeedPlayer})");
            needReCal = true;
        }
        public static void TryComplete(int id)
        {
            if (CUI.challenges == null) return;
            for (int i = 0; i < 3; i++)
            {
                Challenge c = CUI.challenges[i];
                if (c == null) return;
                if (c.ID == id && !c.IsComplete)
                {
                    c.IsComplete = true;
                    CompleteChallenge.Send((byte)i, (byte)Main.LocalPlayer.whoAmI, UserID);
                    return;
                }
            }
        }
    }
}
