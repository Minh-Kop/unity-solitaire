using UI;
using UnityEngine.EventSystems;

namespace Piles
{
    public class StockPile : Pile, IPointerClickHandler
    {
        private WastePile _wastePile;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsEmpty)
            {
                // Lấy lá trên cùng → chuyển sang Waste
                var card = TopCard;
                RemoveCard(card);
                card.gameObject.SetActive(true);
                _wastePile.ReceiveFromStock(card);
            }
        }

        protected override void ArrangeCards()
        {
            // Stock chỉ hiện lá trên cùng (mặt úp)
            foreach (var c in cards)
            {
                c.gameObject.SetActive(false);
                c.CardData.IsFaceUp = false;
                c.Refresh();
            }

            if (TopCard != null)
            {
                TopCard.gameObject.SetActive(true);
            }
        }

        public override bool CanAccept(CardView card)
        {
            return false;
        }
    }
}
