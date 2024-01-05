using JustChallenge.Content.Packages;

namespace JustChallenge.Content.UI
{
    public class ScoreTable : ContainerElement
    {
        internal static string Namekey = "JustChallenge.Content.UI.ScoreTable";
        internal static ScoreTable SUI => UIS?[Namekey] as ScoreTable;
        private UIBottom panel;
        private UIText reset;
        public bool Wating { get; private set; }
        private static float scoreOffset;
        public override void OnInitialization()
        {
            base.OnInitialization();

            UIPanel bg = new(100, 100, color: Color.White * 0.5f);
            bg.SetSize(0, 0, 0.35f, 0.5f);
            bg.SetCenter(-bg.Width / 2, -bg.Height / 2, 0.5f, 0.5f);
            Register(bg);

            UIText name = new("玩家", drawStyle: 0);
            name.SetPos(10, 10);
            bg.Register(name);

            reset = new("发起重置请求", drawStyle: 0);
            reset.SetSize(reset.TextSize);
            reset.SetPos(70, 10);
            reset.Events.OnLeftDown += evt =>
            {
                Wait.Send((byte)Main.LocalPlayer.whoAmI, Wating, true);
                Wating = !Wating;
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
        public void SwitchState()
        {
            Info.IsVisible = !Info.IsVisible;
        }
        public static void Refresh()
        {
            ScoreTable SUI = UIS?[Namekey] as ScoreTable;
            SUI.panel.RemoveAll();
            int y = 10;
            foreach ((byte whoAmI, int point) in ScoreSystem.tempScore)
            {
                Player player = Main.player[whoAmI];
                if (player.active)
                {
                    UIText name = new(player.name, drawStyle: 0);
                    name.SetPos(10, y);
                    SUI.panel.Register(name);

                    UIText score = new(point.ToString(), drawStyle: 0);
                    score.SetPos(-scoreOffset, y, 1);
                    SUI.panel.Register(score);

                    y += 28;
                }
            }
        }
        public void RefreshWaiter(bool success)
        {
            if (success) Wating = false;
            reset.ChangeText((Wating ? "撤销重置请求" : "发起重置请求")
                + "(" + ScoreSystem.waitResetPlayer + "/" + ScoreSystem.activcPlayer + ")");
        }
    }
}
