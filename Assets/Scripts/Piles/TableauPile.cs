using System.Collections.Generic;
using Core;
using UI;
using UnityEngine;

namespace Piles
{
    public class TableauPile : Pile
    {
        [SerializeField]
        private float _cardYOffset = -0.3f;

        private int _firstFaceUpIndex;

        // Mỗi pile cần BoxCollider2D để nhận drop
        // Size collider nên lớn để dễ drop vào cột rỗng
        public override void ArrangeCards()
        {
            for (var i = 0; i < cards.Count; i++)
            {
                cards[i].transform.localPosition = new Vector2(0, i * _cardYOffset);
                cards[i].SetSortingOrder(i);
                cards[i].EnableCollider(false);
            }

            TopCard?.EnableCollider(true);
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

            return TopCard.CardData.Type == incoming.CardData.Type;
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
