using Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public abstract class CardView : MonoBehaviour
    {
        public enum CardState
        {
            Flipped,
            StackedHorizontally,
            StackedVertically,
        }

        [SerializeField]
        private Sprite _faceDownSprite;

        [SerializeField]
        private Sprite _faceUpSprite;

        [SerializeField]
        private Sprite _blankFaceUpSprite;

        private SpriteRenderer _cardSpriteRenderer;

        protected BoxCollider2D _collider2D;

        protected GameObject _glowBorders;

        private SortingGroup _sortingGroup;

        public Card CardData { get; private set; }

        public Vector2 ColliderSize => _collider2D.size;

        protected virtual void Awake()
        {
            _cardSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<BoxCollider2D>();
            _sortingGroup = GetComponent<SortingGroup>();
            GetGlowBorders();
        }

        protected virtual void GetGlowBorders()
        {
            _glowBorders = transform.Find("Glow Border").gameObject;
        }

        public virtual void SetGlowBorders(bool enable)
        {
            _glowBorders.SetActive(enable);
        }

        public void Setup(Card card)
        {
            CardData = card;

            CustomSetup();

            Refresh(CardState.Flipped);
            EnableCollider(false);
        }

        protected abstract void CustomSetup();

        public void Refresh(CardState cardState)
        {
            switch (cardState)
            {
                case CardState.Flipped:
                    _cardSpriteRenderer.sprite = CardData.IsFaceUp
                        ? _faceUpSprite
                        : _faceDownSprite;
                    HandleFlipped();
                    break;
                case CardState.StackedHorizontally:
                    _cardSpriteRenderer.sprite = _blankFaceUpSprite;
                    HandleStackedHorizontally();
                    break;
                case CardState.StackedVertically:
                default:
                    _cardSpriteRenderer.sprite = _blankFaceUpSprite;
                    HandleStackedVertically();
                    break;
            }
        }

        protected abstract void HandleFlipped();
        protected abstract void HandleStackedHorizontally();
        protected abstract void HandleStackedVertically();

        public void FlipFaceUp()
        {
            CardData.IsFaceUp = true;
            Refresh(CardState.Flipped);
        }

        public void EnableCollider(bool enable)
        {
            _collider2D.enabled = enable;
        }

        public virtual void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
        }

        public int GetSortingOrder()
        {
            return _sortingGroup.sortingOrder;
        }
    }
}
