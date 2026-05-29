using System.Collections.Generic;
using Core;
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

        public override void ArrangeCards()
        {
            // Hiện tối đa 3 lá trên cùng
            var showCount = Mathf.Min(3, cards.Count);
            var startIndex = cards.Count - showCount;

            // Hiện 3 lá trên cùng, offset nhẹ để thấy có nhiều bài
            for (var i = 0; i < startIndex; i++)
            {
                cards[i].gameObject.SetActive(false);
                cards[i].EnableCollider(false);
            }

            for (var i = 0; i < showCount; i++)
            {
                var cardIndex = cards.Count - 1 - i;
                cards[cardIndex].gameObject.SetActive(true);
                cards[cardIndex].SetSortingOrder(cardIndex);
                cards[cardIndex].EnableCollider(false);
                cards[cardIndex].transform.localPosition = new Vector2(
                    i * GameManager.Instance.cardXOffset,
                    0
                );

                cards[cardIndex]
                    .Refresh(
                        i != 0 ? CardView.CardState.StackedHorizontally : CardView.CardState.Flipped
                    );
            }

            TopCard?.EnableCollider(true);
        }

        public void ReceiveFromStock(CardView card)
        {
            card.FlipFaceUp();
            AddCard(card);
            ArrangeCards();
        }

        // Chỉ lá trên cùng mới được kéo
        public bool IsTopCard(CardView card)
        {
            return TopCard == card;
        }

        public List<CardView> TakeAll()
        {
            var taken = new List<CardView>(cards);
            taken.Reverse(); // Ngược lại để Stock nhận đúng thứ tự
            cards.Clear();
            return taken;
        }
    }
}
