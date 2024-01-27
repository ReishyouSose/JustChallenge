using JustChallenge.Content.Packages;

namespace JustChallenge.Content.UI
{
    public class ScoreTable : ContainerElement
    {
        internal static string Namekey = "JustChallenge.Content.UI.ScoreTable";
        internal static ScoreTable SUI => UIS?[Namekey] as ScoreTable;
        internal static int waiter;
        internal static int UISwitchType;
        private UIPanel bg;
        private UIBottom panel;
        private UIText reset;
        private Texture2D[] adminTex;
        public bool Wating { get; private set; }
        private static float scoreOffset;
        public override void OnInitialization()
        {
            base.OnInitialization();

            bg = new(400, 50 + 28 * 3, color: Color.White)
            {
                CanDrag = true
            };
            bg.SetPos(-bg.Width / 2, -bg.Height / 2, 0.5f, 0.4f);
            Register(bg);

            UIText name = new("玩家", drawStyle: 0);
            name.SetPos(10, 10);
            bg.Register(name);

            reset = new("发起重置请求", drawStyle: 0);
            reset.SetSize(reset.TextSize);
            reset.SetPos(70, 10);
            reset.Events.OnMouseOver += evt => reset.color = Color.Gold;
            reset.Events.OnMouseOut += evt => reset.color = Color.White;
            reset.Events.OnLeftDown += evt =>
            {
                Wait.Send((byte)Main.LocalPlayer.whoAmI);
                Wating = !Wating;
            };
            reset.ReDraw += sb =>
            {
                reset.DrawSelf(sb);
                if (reset.Info.IsMouseHover)
                {
                    Main.hoverItemName += "清除分数并重置所有挑战（包括已完成的）";
                }
            };
            bg.Register(reset);

            panel = new(0, -38, 1, 1);
            panel.SetPos(0, 38);
            bg.Register(panel);

            UIText score = new("分数", drawStyle: 0);
            scoreOffset = score.TextSize.X + 10;
            score.SetPos(-scoreOffset, 10, 1);
            bg.Register(score);

            Info.IsVisible = false;
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (Main.netMode == NetmodeID.SinglePlayer) Info.IsVisible = false;
        }
        public new void ResetPos() => bg.ResetPos();
        public void SwitchState()
        {
            Info.IsVisible = !Info.IsVisible;
        }
        public void RefreshScore(byte admin)
        {
            ScoreSystem.admin = admin;
            panel.RemoveAll();
            if (adminTex == null)
            {
                adminTex = new Texture2D[4];
                adminTex[0] = T2D("JustChallenge/UISupport/Asset/Admin");
                adminTex[1] = T2D("JustChallenge/UISupport/Asset/AdminBg");
                adminTex[2] = T2D("JustChallenge/UISupport/Asset/ChangeAdmin");
                adminTex[3] = T2D("JustChallenge/UISupport/Asset/ChangeAdmin_Light");
            }
            int y = 10;
            foreach ((byte whoAmI, int point) in ScoreSystem.tempScore)
            {
                Player player = Main.player[whoAmI];
                if (player.active)
                {
                    UIText name = new(player.name, drawStyle: 0);
                    name.SetPos(70, y + 6);
                    name.SetSize(name.TextSize);
                    panel.Register(name);

                    UIImage adminLogo = null;
                    if (whoAmI == admin)
                    {
                        adminLogo = new(adminTex[1]);
                        adminLogo.ReDraw += sb =>
                        {
                            adminLogo.DrawSelf(sb);
                            sb.Draw(adminTex[0], adminLogo.Center(), null, Color.White, 0, new(16), 1f, 0, 0);
                        };
                    }
                    else if (Main.myPlayer == admin)
                    {
                        adminLogo = new(adminTex[2]);
                        adminLogo.Events.OnMouseOver += evt => adminLogo.Tex = adminTex[3];
                        adminLogo.Events.OnMouseOut += evt => adminLogo.Tex = adminTex[2];
                        adminLogo.ReDraw += sb =>
                        {
                            adminLogo.DrawSelf(sb);
                            if (adminLogo.Info.IsMouseHover)
                            {
                                Main.hoverItemName += "双击转移管理员权限";
                            }
                        };
                        byte index = whoAmI;
                        adminLogo.Events.OnLeftDoubleClick += evt =>
                        {
                            ChangeAdmin.Send(index, true);
                        };
                    }
                    adminLogo?.SetPos(13, y);
                    panel.Register(adminLogo);

                    UIText score = new(point.ToString(), drawStyle: 0);
                    score.SetPos(-scoreOffset, y + 6, 1);
                    panel.Register(score);

                    y += 50;
                }
            }
            panel.Info.Height.Pixel = y + 20;
            bg.Info.Height.Pixel = 20 + panel.Info.Height.Pixel;
            //bg.SetPos(-bg.Width / 2, 0, 0.5f, 0.4f);
            bg.Calculation();
        }
        public void RefreshWaiter(bool success)
        {
            if (success) Wating = false;
            reset.ChangeText((Wating ? "撤销重置请求" : "发起重置请求")
                + "(" + waiter + "/" + ScoreSystem.activcPlayer + ")");
        }
    }
}
