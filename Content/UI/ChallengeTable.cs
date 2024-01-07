using JustChallenge.Content.Challenges;
using JustChallenge.Content.Packages;

namespace JustChallenge.Content.UI
{
    public class ChallengeTable : ContainerElement
    {
        internal static ChallengeTable CUI => UIS?[Namekey] as ChallengeTable;
        internal static string Namekey = "JustChallenge.Content.UI.ChallengeTable";
        public Challenge[] challenges;
        public UIPanel bg;
        public UIImage[] refreshs;
        private bool needReCal;
        private bool allCompleted;
        private static readonly string allText = GTV("AllCompleted");
        private static Texture2D[] resetTex;
        public override void OnInitialization()
        {
            base.OnInitialization();

            bg = new(100, 20 + 28 * 3, color: Color.White);
            bg.SetPos(20, -bg.Height / 2f, 0, 0.5f);
            Register(bg);

            challenges = new Challenge[3];

            refreshs = new UIImage[3];
            resetTex = new Texture2D[2];
            resetTex[0] = T2D("JustChallenge/UISupport/Asset/Refresh");
            resetTex[1] = T2D("JustChallenge/UISupport/Asset/Refresh_Light");
            int y = 10;
            for (byte i = 0; i < 3; i++)
            {
                UIImage r = new(resetTex[0]);
                r.SetPos(5, y);
                byte index = i;
                r.Events.OnMouseOver += evt => r.Tex = resetTex[1];
                r.Events.OnMouseOut += evt => r.Tex = resetTex[0];
                r.Events.OnLeftDown += evt =>
                {
                    if (challenges[index] != null)
                    {
                        if (Main.myPlayer == ScoreSystem.admin)
                        {
                            RefreshOne.Send(index);
                        }
                        else Main.NewText("只有管理员可以刷新挑战");
                    }
                };
                refreshs[i] = r;
                bg.Register(refreshs[i]);
                y += 32;
            }
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (Main.netMode == NetmodeID.SinglePlayer) Info.IsVisible = false;
            if (needReCal)
            {
                bg.Info.Width.Pixel = allCompleted ? (20 + FontAssets.MouseText.Value.MeasureString(allText).X)
                    : (45 + challenges.Select(x => x?.Width ?? 0).Max());
                bg.Calculation();
                needReCal = false;
            }
        }
        public override void DrawChildren(SpriteBatch sb)
        {
            base.DrawChildren(sb);
            if (allCompleted)
            {
                ChatManager.DrawColorCodedStringShadow(sb, FontAssets.MouseText.Value, allText,
                    refreshs[1].HitBox().TopRight(), Color.White, 0, Vector2.Zero, Vector2.One);
            }
            else
            {
                int x = bg.Width - 40;
                for (int i = 0; i < 3; i++)
                {
                    Challenge challenge = challenges[i];
                    Vector2 pos = refreshs[i].HitBox().TopRight();
                    if (challenge == null)
                    {
                        ChatManager.DrawColorCodedStringShadow(sb, FontAssets.MouseText.Value, "无",
                            pos, Color.White, 0, Vector2.Zero, Vector2.One, spread: 1);
                    }
                    else
                    {
                        challenge.Draw(sb, new Vector2(pos.X, pos.Y + 5));
                        if (challenge.IsComplete)
                        {
                            sb.Draw(TextureAssets.MagicPixel.Value, new Vector2(pos.X, pos.Y + 13),
                                new Rectangle(0, 0, x, 1), Color.White, 0, Vector2.Zero, new Vector2(1, 3), 0, 0);
                        }
                    }
                }
            }
        }
        public void SyncChallenges(byte index, int[] ids, bool[] complete)
        {
            challenges = new Challenge[3];
            if (index == 3)
            {
                allCompleted = true;
            }
            else
            {
                allCompleted = false;
                for (int i = 0; i < 3; i++)
                {
                    challenges[i] = ids[i] == -1 ? null : Challenge.NewChallenge(ids[i], complete[i]);
                }
            }
            if (index < 3)
            {
                Main.NewText((index + 1) + "号挑战已刷新");
            }
            needReCal = true;
        }
        public static void ChangeState(byte index)
        {
            CUI.challenges[index].IsComplete = true;
        }
        public static void TryComplete(int id)
        {
            //Main.NewText("try" + ChallengeID.challenges[id].Description.Value);
            if (CUI.challenges == null) return;
            for (int i = 0; i < 3; i++)
            {
                Challenge c = CUI.challenges[i];
                if (c != null && c.type == id && !c.IsComplete)
                {
                    //c.IsComplete = true;
                    CompleteChallenge.Send((byte)i, (byte)Main.LocalPlayer.whoAmI, UserID);
                    return;
                }
            }
        }
    }
}
