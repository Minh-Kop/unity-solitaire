using Core;
using UI;
using UnityEngine;

namespace Piles
{
    public class TableauPile : Pile
    {
        private int _firstFaceUpIndex;
        private ListCardView _listCardView;

        public int FirstFaceUpIndex
        {
            get => _firstFaceUpIndex;
            set => _firstFaceUpIndex = value >= 0 ? value : 0;
        }

        protected override void Awake()
        {
            base.Awake();
            _listCardView = transform.GetChild(0).GetComponent<ListCardView>();
        }

        public override void AddCard(CardView card)
        {
            if (card.CardData.IsFaceUp)
            {
                _listCardView.AddCard(card);
            }
            else
            {
                _cards.Add(card);
                card.transform.SetParent(transform);
            }
        }

        // Mỗi pile cần BoxCollider2D để nhận drop
        // Size collider nên lớn để dễ drop vào cột rỗng
        public override void ArrangeCards()
        {
            for (var i = 0; i < _firstFaceUpIndex; i++)
            {
                _cards[i].transform.localPosition = new Vector2(
                    0,
                    -i * GameManager.Instance.cardYOffset
                );
                _cards[i].SetSortingOrder(i);
            }

            _listCardView.transform.localPosition = new Vector2(
                0,
                -_firstFaceUpIndex * GameManager.Instance.cardYOffset
            );
            _listCardView.SetSortingOrder(_firstFaceUpIndex);
            _listCardView.RefreshList(_firstFaceUpIndex);
        }

        public override bool CanAccept(CardView incoming)
        {
            if (_listCardView.IsEmpty)
            {
                return true;
            }

            if (incoming == null || _listCardView.TopCard.CardData is CoverCard)
            {
                return false;
            }

            return _listCardView.TopCard.CardData.Type == incoming.CardData.Type;
        }

        public void FlipTopCard()
        {
            if (TopCard != null)
            {
                TopCard.FlipFaceUp();
                FirstFaceUpIndex -= 1;
                _listCardView.AddCard(TopCard);
                _cards.Remove(TopCard);
                ArrangeCards();
            }
        }
    }
}
