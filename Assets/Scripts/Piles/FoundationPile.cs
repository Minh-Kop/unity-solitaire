using System.Collections;
using Core;
using UI;
using UnityEngine;

namespace Piles
{
    public class FoundationPile : Pile
    {
        private int _count;
        private int _maxCount;
        private string _type;

        public override void ArrangeCards()
        {
            if (TopCard.CardData is CoverCard coverCard)
            {
                _count = coverCard.Count;
                _maxCount = coverCard.MaxCount;
                _type = coverCard.Type;

                if (cards.Count > 1)
                {
                    // Destroy(TopCard.gameObject);
                    // cards.RemoveAt(cards.Count - 1);
                    (cards[^2], cards[^1]) = (cards[^1], cards[^2]);
                }
            }

            // Foundation chỉ hiện lá trên cùng
            foreach (var c in cards)
            {
                c.gameObject.SetActive(false);
                c.transform.localPosition = Vector3.zero;
            }

            if (TopCard != null)
            {
                TopCard.gameObject.SetActive(true);
                TopCard.EnableCollider(true);
                TopCard.SetSortingOrder(0);
            }

            _count = cards.Count - 1;

            if (_count == _maxCount)
            {
                StartCoroutine(ClearPile());
            }

            print($"{_count}/{_maxCount}, type: {_type}");
        }

        private IEnumerator ClearPile()
        {
            yield return new WaitForSeconds(1f);

            foreach (var card in cards)
            {
                Destroy(card.gameObject);
            }

            cards.Clear();
        }

        public override bool CanAccept(CardView incoming)
        {
            print($"{_count}/{_maxCount}, type: {_type}");
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
