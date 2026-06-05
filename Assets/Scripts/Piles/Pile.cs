using System.Collections.Generic;
using Interfaces;
using UI;
using UnityEngine;

namespace Piles
{
    public abstract class Pile : MonoBehaviour, IGlower
    {
        protected List<CardView> _cards = new();
        private BoxCollider2D _collider2D;
        private GameObject _glowBorders;

        public CardView TopCard => _cards.Count > 0 ? _cards[^1] : null;
        protected bool IsEmpty => _cards.Count == 0;

        protected virtual void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();

            var temp = transform.Find("Glow Border");
            if (temp)
            {
                _glowBorders = temp.gameObject;
            }
        }

        public void SetGlowBorders(bool enable)
        {
            if (_glowBorders)
            {
                _glowBorders.SetActive(enable);
            }
        }

        public Pile GetPile()
        {
            return this;
        }

        public virtual void AddCard(CardView card)
        {
            _cards.Add(card);
            card.transform.SetParent(transform);
            card.SetPile();
            // ArrangeCards();
        }

        public virtual void RemoveCard(CardView card)
        {
            _cards.Remove(card);
            // ArrangeCards();
        }

        // Mỗi loại pile sắp xếp bài khác nhau
        public abstract void ArrangeCards();

        // Kiểm tra có thể đặt bài vào không
        public abstract bool CanAccept(CardView card);
    }
}
