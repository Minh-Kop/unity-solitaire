using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Piles
{
    public abstract class Pile : MonoBehaviour
    {
        protected List<CardView> _cards = new();
        private BoxCollider2D _collider2D;

        public CardView TopCard => _cards.Count > 0 ? _cards[^1] : null;
        public bool IsEmpty => _cards.Count == 0;
        public int Count => _cards.Count;

        protected virtual void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();
        }

        public virtual void AddCard(CardView card)
        {
            if (_cards.Count == 0)
            {
                _collider2D.enabled = false;
            }

            _cards.Add(card);
            card.transform.SetParent(transform);
            // ArrangeCards();
        }

        public virtual void RemoveCard(CardView card)
        {
            _cards.Remove(card);
            // ArrangeCards();

            if (_cards.Count == 0)
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
