using Core;
using UnityEngine;

namespace UI
{
    public class CardView : MonoBehaviour
    {
        [SerializeField]
        private Sprite _faceDownSprite;

        [SerializeField]
        private Sprite _faceUpSprite;

        private SpriteRenderer _spriteRenderer;

        public Card CardData { get; private set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Setup(Card card)
        {
            CardData = card;
            Refresh();
        }

        public void Refresh()
        {
            _spriteRenderer.sprite = CardData.IsFaceUp ? _faceUpSprite : _faceDownSprite;
        }

        public void FlipFaceUp()
        {
            CardData.IsFaceUp = true;
            Refresh();
        }

        // Z nhỏ hơn = hiển thị trước (gần camera hơn)
        public void SetSortingOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }
    }
}
