using Core;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class CardView : MonoBehaviour
    {
        [SerializeField]
        private Sprite _faceDownSprite;

        [SerializeField]
        private Sprite _faceUpSprite;

        [SerializeField]
        private TextMeshPro _text;

        [SerializeField]
        private TextMeshPro _countText;

        private Collider2D _collider2D;

        private SortingGroup _sortingGroup;

        private SpriteRenderer _spriteRenderer;

        public Card CardData { get; private set; }

        private void Awake()
        {
            _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<Collider2D>();
            _collider2D.enabled = false;
            _sortingGroup = GetComponent<SortingGroup>();
        }

        public void Setup(Card card)
        {
            CardData = card;
            if (CardData is CoverCard coverCard)
            {
                _countText.text = $"{coverCard.Count}/{coverCard.MaxCount}";
                _text.text = coverCard.Type;
            }
            else
            {
                _text.text = CardData.StringContent;
            }

            Refresh();
        }

        public void Refresh()
        {
            _spriteRenderer.sprite = CardData.IsFaceUp ? _faceUpSprite : _faceDownSprite;

            _text.gameObject.SetActive(CardData.IsFaceUp);

            if (CardData is CoverCard)
            {
                _countText.gameObject.SetActive(CardData.IsFaceUp);
            }
        }

        public void FlipFaceUp()
        {
            CardData.IsFaceUp = true;
            Refresh();
        }

        public void EnableCollider(bool enable)
        {
            _collider2D.enabled = enable;
        }

        // Z nhỏ hơn = hiển thị trước (gần camera hơn)
        public void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
        }
    }
}
