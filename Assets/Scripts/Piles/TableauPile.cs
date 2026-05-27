using System.Collections.Generic;
using Core;
using UI;
using UnityEngine;

namespace Piles
{
    public class TableauPile : Pile
    {
        [SerializeField]
        private float cardYOffset = -0.3f;

        // Mỗi pile cần BoxCollider2D để nhận drop
        // Size collider nên lớn để dễ drop vào cột rỗng

        protected override void ArrangeCards()
        {
            for (var i = 0; i < cards.Count; i++)
            {
                cards[i].transform.localPosition = new Vector3(0, i * cardYOffset, i * -0.01f);
                cards[i].SetSortingOrder(i);
            }
        }

        public override bool CanAccept(CardView incoming)
        {
            if (incoming == null || TopCard.CardData is CoverCard)
            {
                return false;
            }

            if (IsEmpty)
            {
                return true;
            }

            return TopCard.CardData.Type == incoming.CardData.Type;
        }

        public List<CardView> GetCardsFrom(int index)
        {
            return cards.GetRange(index, cards.Count - index);
        }

        public int IndexOf(CardView card)
        {
            return cards.IndexOf(card);
        }
    }
}
