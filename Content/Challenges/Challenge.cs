using Terraria.Localization;

namespace JustChallenge.Content.Challenges
{
    public abstract class Challenge : ModType
    {
        public bool IsComplete;
        public Challenge()
        {
            ChallengeID.challenges?.Add(this);
        }
        public Challenge Copy(bool isComplete)
        {
            IsComplete = isComplete;
            return this;
        }
        public static Challenge NewChallenge(int id, bool isComplete) => ChallengeID.challenges[id].Copy(isComplete);
        public abstract int ID { get; }
        public virtual LocalizedText Description => Language.GetText($"Mods.JustChallenge.Challenges.{GetType().Name}");
        public virtual void Draw(SpriteBatch spb, float x, float y)
        {
            ChatManager.DrawColorCodedString(spb, FontAssets.MouseText.Value, Description.Value,
                new Vector2(x, y), Color.White, 0, Vector2.Zero, Vector2.One);
        }
        protected override void Register() { }
        public static int[] RollChallenge()
        {
            List<int> rng = new();
            while (rng.Count < 3)
            {
                int r = Main.rand.Next(3);
                if (!rng.Contains(r))
                {
                    rng.Add(r);
                }
            }
            return rng.ToArray();
        }
    }
}
