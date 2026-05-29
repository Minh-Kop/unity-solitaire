using System.Collections.Generic;
using Core;
using UI;
using UnityEngine;

namespace Piles
{
    public class TableauPile : Pile
    {
        private int _firstFaceUpIndex;

        public int FirstFaceUpIndex
        {
            get => _firstFaceUpIndex;
            set => _firstFaceUpIndex = value >= 0 ? value : 0;
        }

        // Mỗi pile cần BoxCollider2D để nhận drop
        // Size collider nên lớn để dễ drop vào cột rỗng
        public override void ArrangeCards()
        {
            for (var i = 0; i < _firstFaceUpIndex; i++)
            {
                cards[i].transform.localPosition = new Vector2(
                    0,
                    -i * GameManager.Instance.cardYOffset
                );
                cards[i].SetSortingOrder(i);
                cards[i].EnableCollider(false);
            }

            for (var i = _firstFaceUpIndex; i < cards.Count; i++)
            {
                cards[i].transform.localPosition = new Vector2(
                    0,
                    -i * GameManager.Instance.cardYOffset
                        - GameManager.Instance.extraYOffset * (i - FirstFaceUpIndex)
                );
                cards[i].SetSortingOrder(i);
                if (i == cards.Count - 1)
                {
                    cards[i].Refresh(CardView.CardState.Flipped);
                    cards[i].EnableCollider(true);
                }
                else
                {
                    cards[i].Refresh(CardView.CardState.StackedVertically);
                    cards[i].EnableCollider(false);
                }
            }

            if (TopCard?.CardData is CoverCard coverCard)
            {
                coverCard.Count = cards.Count - 1 - FirstFaceUpIndex;
            }
        }

        public override bool CanAccept(CardView incoming)
        {
            if (incoming == null || TopCard?.CardData is CoverCard)
            {
                return false;
            }

            if (IsEmpty)
            {
                return true;
            }

            return TopCard?.CardData.Type == incoming.CardData.Type;
        }

        public List<CardView> GetCardsFromLastToFirstFaceDown()
        {
            var index = cards.Count - 1;
            while (index >= 0 && cards[index].CardData.IsFaceUp)
            {
                index--;
            }

            index++;

            return cards.GetRange(index, cards.Count - index);
        }
    }
}
