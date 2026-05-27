using UI;
using UnityEngine;

namespace Piles
{
    public class WastePile : Pile
    {
        // Waste chỉ cho phép lấy lá trên cùng
        public override bool CanAccept(CardView card)
        {
            return false;
        }

        protected override void ArrangeCards()
        {
            // Hiện tối đa 3 lá trên cùng
            var showCount = Mathf.Min(3, cards.Count);
            var startIndex = cards.Count - showCount;

            // Hiện 3 lá trên cùng, offset nhẹ để thấy có nhiều bài
            for (var i = 0; i < startIndex; i++)
            {
                cards[i].gameObject.SetActive(false);
            }

            for (var i = 0; i < showCount; i++)
            {
                var cardIndex = startIndex + i;
                cards[cardIndex].gameObject.SetActive(true);

                cards[cardIndex].transform.localPosition = new Vector2(i * 1.2f, 0);
            }
        }

        public void ReceiveFromStock(CardView card)
        {
            card.FlipFaceUp();
            AddCard(card);
        }

        // Chỉ lá trên cùng mới được kéo
        public bool IsTopCard(CardView card)
        {
            return TopCard == card;
        }
    }
}
