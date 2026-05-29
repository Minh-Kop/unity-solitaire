using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Piles
{
    public class StockPile : Pile, IPointerClickHandler
    {
        [SerializeField]
        private WastePile _wastePile;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsEmpty)
            {
                // Lấy lá trên cùng → chuyển sang Waste
                var card = TopCard;
                RemoveCard(card);
                ArrangeCards();
                _wastePile.ReceiveFromStock(card);
            }
            else
            {
                RecycleFromWaste();
            }
        }

        public override void ArrangeCards()
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
                TopCard.gameObject.transform.localPosition = Vector3.zero;
            }
        }

        public override bool CanAccept(CardView card)
        {
            return false;
        }

        private void RecycleFromWaste()
        {
            // Lấy toàn bộ waste theo thứ tự ngược lại
            var wasteCards = _wastePile.TakeAll();

            foreach (var card in wasteCards)
            {
                AddCard(card); // ArrangeCards sẽ úp bài lại
            }

            ArrangeCards();
        }
    }
}
