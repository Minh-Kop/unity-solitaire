using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Piles
{
    public abstract class Pile : MonoBehaviour
    {
        private BoxCollider2D _collider2D;

        protected List<CardView> cards = new();

        public CardView TopCard => cards.Count > 0 ? cards[^1] : null;
        public bool IsEmpty => cards.Count == 0;
        public int Count => cards.Count;

        protected virtual void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();
        }

        public virtual void AddCard(CardView card)
        {
            if (cards.Count == 0)
            {
                _collider2D.enabled = false;
            }

            cards.Add(card);
            card.transform.SetParent(transform);
            // ArrangeCards();
        }

        public virtual void RemoveCard(CardView card)
        {
            cards.Remove(card);
            // ArrangeCards();

            if (cards.Count == 0)
            {
                _collider2D.enabled = true;
            }
        }

        // Mỗi loại pile sắp xếp bài khác nhau
        public abstract void ArrangeCards();

        // Kiểm tra có thể đặt bài vào không
        public abstract bool CanAccept(CardView card);
    }
}
