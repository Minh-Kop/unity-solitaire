using System.Collections;
using Core;
using TMPro;
using UI;
using UnityEngine;

namespace Piles
{
    public class FoundationPile : Pile
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private TextMeshPro _typeText;

        [SerializeField]
        private TextMeshPro _countText;

        private int _count;
        private int _maxCount;
        private string _type;

        protected override void Awake()
        {
            base.Awake();
            EnableTypeAndCount(false);
        }

        private void Update()
        {
            _typeText.text = _type;
            _countText.text = $"{_count}/{_maxCount}";
        }

        private void EnableTypeAndCount(bool enable = true)
        {
            _spriteRenderer.gameObject.SetActive(enable);
            _countText.gameObject.SetActive(enable);
        }

        public override void ArrangeCards()
        {
            if (TopCard.CardData is CoverCard coverCard)
            {
                _count = coverCard.Count;
                _maxCount = coverCard.MaxCount;
                _type = coverCard.Type;

                if (_cards.Count > 1)
                {
                    // Destroy(TopCard.gameObject);
                    // cards.RemoveAt(cards.Count - 1);
                    (_cards[^2], _cards[^1]) = (_cards[^1], _cards[^2]);
                    EnableTypeAndCount();
                }
            }
            else
            {
                EnableTypeAndCount();
            }

            // Foundation chỉ hiện lá trên cùng
            foreach (var c in _cards)
            {
                c.gameObject.SetActive(false);
                c.transform.localPosition = Vector3.zero;
            }

            if (TopCard != null)
            {
                TopCard.gameObject.SetActive(true);
                TopCard.EnableCollider(true);
                TopCard.SetSortingOrder(0);
                TopCard.Refresh(CardView.CardState.Flipped);
            }

            _count = _cards.Count - 1;

            if (_count == _maxCount)
            {
                StartCoroutine(ClearPile());
            }
        }

        private IEnumerator ClearPile()
        {
            yield return new WaitForSeconds(1f);

            foreach (var card in _cards)
            {
                Destroy(card.gameObject);
            }

            _cards.Clear();
            EnableTypeAndCount(false);
        }

        public override bool CanAccept(CardView incoming)
        {
            if (incoming == null)
            {
                return false;
            }

            if (IsEmpty && incoming.CardData is CoverCard)
            {
                return true;
            }

            return _type == incoming.CardData.Type;
        }
    }
}
