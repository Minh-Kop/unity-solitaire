using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Piles
{
    public abstract class Pile : MonoBehaviour
    {
        protected List<CardView> cards = new();

        public CardView TopCard => cards.Count > 0 ? cards[^1] : null;
        public bool IsEmpty => cards.Count == 0;
        public int Count => cards.Count;

        public virtual void AddCard(CardView card)
        {
            cards.Add(card);
            card.transform.SetParent(transform);
            ArrangeCards();
        }

        public virtual void RemoveCard(CardView card)
        {
            cards.Remove(card);
            ArrangeCards();
        }

        // Mỗi loại pile sắp xếp bài khác nhau
        protected abstract void ArrangeCards();

        // Kiểm tra có thể đặt bài vào không
        public abstract bool CanAccept(CardView card);
    }
}
