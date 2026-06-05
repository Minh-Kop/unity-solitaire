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
            var showCount = Mathf.Min(3, _cards.Count);
            var startIndex = _cards.Count - showCount;

            // Hiện 3 lá trên cùng, offset nhẹ để thấy có nhiều bài
            for (var i = 0; i < startIndex; i++)
            {
                _cards[i].gameObject.SetActive(false);
                _cards[i].EnableCollider(false);
            }

            for (var i = 0; i < showCount; i++)
            {
                var cardIndex = _cards.Count - 1 - i;
                _cards[cardIndex].gameObject.SetActive(true);
                _cards[cardIndex].SetSortingOrder(cardIndex);
                _cards[cardIndex].EnableCollider(false);
                _cards[cardIndex].transform.localPosition = new Vector2(
                    i * GameManager.Instance.cardXOffset,
                    0
                );

                _cards[cardIndex]
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

        public List<CardView> TakeAll()
        {
            var taken = new List<CardView>(_cards);
            taken.Reverse(); // Ngược lại để Stock nhận đúng thứ tự
            _cards.Clear();
            return taken;
        }
    }
}
