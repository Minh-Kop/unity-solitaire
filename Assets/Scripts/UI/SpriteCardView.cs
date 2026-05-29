using UnityEngine;

namespace UI
{
    public class SpriteCardView : CardView
    {
        [SerializeField]
        private SpriteRenderer _mainIconSpriteRenderer;

        [SerializeField]
        private SpriteRenderer _upperIconSpriteRenderer;

        [SerializeField]
        private SpriteRenderer _rightIconSpriteRenderer;

        protected override void Awake()
        {
            base.Awake();
            _mainIconSpriteRenderer.gameObject.SetActive(true);
            _upperIconSpriteRenderer.gameObject.SetActive(false);
            _rightIconSpriteRenderer.gameObject.SetActive(false);
        }

        protected override void CustomSetup()
        {
            _mainIconSpriteRenderer.sprite = CardData.SpriteContent;
            _upperIconSpriteRenderer.sprite = CardData.SpriteContent;
            _rightIconSpriteRenderer.sprite = CardData.SpriteContent;
        }

        protected override void HandleFlipped()
        {
            _mainIconSpriteRenderer.gameObject.SetActive(CardData.IsFaceUp);
            _upperIconSpriteRenderer.gameObject.SetActive(false);
            _rightIconSpriteRenderer.gameObject.SetActive(false);
        }

        protected override void HandleStackedHorizontally()
        {
            _mainIconSpriteRenderer.gameObject.SetActive(false);
            _upperIconSpriteRenderer.gameObject.SetActive(false);
            _rightIconSpriteRenderer.gameObject.SetActive(true);
        }

        protected override void HandleStackedVertically()
        {
            _mainIconSpriteRenderer.gameObject.SetActive(false);
            _upperIconSpriteRenderer.gameObject.SetActive(true);
            _rightIconSpriteRenderer.gameObject.SetActive(false);
        }
    }
}
