using Core;
using UI;

namespace Piles
{
    public class FoundationPile : Pile
    {
        private Suit? acceptedSuit = null;

        protected override void ArrangeCards()
        {
            // Foundation chỉ hiện lá trên cùng
            foreach (var c in cards)
            {
                c.gameObject.SetActive(false);
            }

            if (TopCard != null)
            {
                TopCard.gameObject.SetActive(true);
            }
        }

        public override bool CanAccept(CardView incoming)
        {
            if (incoming == null)
            {
                return false;
            }

            var inc = incoming.CardData;

            // Chỉ nhận 1 lá tại 1 thời điểm (không kéo nhóm vào Foundation)
            if (IsEmpty)
            {
                return inc.rank == 1; // Phải bắt đầu bằng Ace
            }

            var top = TopCard.CardData;
            return inc.suit == top.suit && inc.rank == top.rank + 1;
        }
    }
}
