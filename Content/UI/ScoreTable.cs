namespace JustChallenge.Content.UI
{
    public class ScoreTable : ContainerElement
    {
        internal static string Namekey = "JustChallenge.Content.UI.ScoreTable";
        private UIBottom panel;
        private static float scoreOffset;
        public override void OnInitialization()
        {
            base.OnInitialization();

            UIImage bg = new(T2D("JustChallenge/UISupport/Asset/ClothesStyleBack"));
            bg.SetSize(0, 0, 0.2f, 0.5f);
            bg.SetCenter(0, 0, 0.5f, 0.5f);
            Register(bg);

            UIText name = new("玩家", drawStyle: 0);
            name.SetPos(10, 10);
            bg.Register(name);

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
            foreach ((byte whoAmI,int point) in ScoreSystem.tempScore)
            {
                UIText name = new(Main.player[whoAmI].name, drawStyle: 0);
                name.SetPos(10, y);
                SUI.panel.Register(name);

                UIText score = new(point.ToString(), drawStyle: 0);
                score.SetPos(-scoreOffset, y, 1);
                SUI.panel.Register(score);

                y += 38;
            }
        }
    }
}
