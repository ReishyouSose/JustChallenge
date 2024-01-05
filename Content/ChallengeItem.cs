using JustChallenge.Content.Challenges;
using JustChallenge.Content.UI;
using Terraria.DataStructures;

namespace JustChallenge.Content
{
    public class ChallengeItem : GlobalItem
    {
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (ChallengeID.create.TryGetValue(item.type, out int id))
            {
                ChallengeTable.TryComplete(id);
            }
        }
    }
}
