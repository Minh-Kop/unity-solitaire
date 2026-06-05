using System.Collections.Generic;
using Core;
using Piles;
using UnityEngine;

namespace UI
{
    public class ListCardView : CardView
    {
        private readonly List<CardView> _cards = new();

        private Vector2 _originalBoxColliderSize;
        public bool IsDraggingAllowed => _cards.Count > 0;
        public CardView TopCard => _cards.Count > 0 ? _cards[^1] : null;
        public bool IsEmpty => _cards.Count == 0;

        protected override void Awake()
        {
            _pile = GetComponentInParent<TableauPile>();
            base.Awake();
            _originalBoxColliderSize = _collider2D.size;
        }

        public override void SetSortingOrder(int order)
        {
            base.SetSortingOrder(order);
            for (var i = 0; i < _cards.Count; i++)
            {
                _cards[i].SetSortingOrder(order + i);
            }
        }

        public override void SetGlowBorders(bool enable)
        {
            if (IsEmpty)
            {
                _pile.SetGlowBorders(enable);
            }
            else
            {
                foreach (var card in _cards)
                {
                    card.SetGlowBorders(enable);
                }
            }
        }

        protected override void CustomSetup() { }

        protected override void HandleFlipped() { }

        protected override void HandleStackedHorizontally() { }

        protected override void HandleStackedVertically() { }

        public void RefreshList(int firstFaceUpIndex)
        {
            for (var i = 0; i < _cards.Count; i++)
            {
                _cards[i].transform.localPosition = new Vector2(
                    0,
                    -i * GameManager.Instance.cardYOffset
                        - GameManager.Instance.extraYOffset * (i - 0)
                );
                _cards[i].SetSortingOrder(i + firstFaceUpIndex);
                if (i == _cards.Count - 1)
                {
                    _cards[i].Refresh(CardState.Flipped);

                    if (_cards[i]?.CardData is CoverCard coverCard)
                    {
                        coverCard.Count = _cards.Count - 1;
                    }
                }
                else
                {
                    _cards[i].Refresh(CardState.StackedVertically);
                }
            }

            if (_cards.Count == 0)
            {
                _collider2D.offset = Vector2.zero;
                _collider2D.size = _originalBoxColliderSize;
            }
            else
            {
                _collider2D.size = new Vector2(
                    _collider2D.size.x,
                    _cards[0].transform.position.y
                        - _cards[^1].transform.position.y
                        + _cards[0].ColliderSize.y
                );
                _collider2D.offset = new Vector2(
                    0,
                    -(_collider2D.size.y - _cards[0].ColliderSize.y) / 2
                );
            }
        }

        public void AddCard(CardView card)
        {
            card.EnableCollider(false);
            _cards.Add(card);
            card.transform.SetParent(transform);
        }

        public void TransferCards(Pile destinationPile)
        {
            foreach (var c in _cards)
            {
                destinationPile.AddCard(c);
            }

            destinationPile.ArrangeCards();
            _cards.Clear();
        }
    }
}
