using TMPro;
using UnityEngine;

namespace UI
{
    public class TextCardView : CardView
    {
        [SerializeField]
        private TextMeshPro _mainText;

        [SerializeField]
        private TextMeshPro _rightText;

        [SerializeField]
        private TextMeshPro _upperText;

        protected override void Awake()
        {
            base.Awake();
            _mainText.gameObject.SetActive(true);
            _upperText.gameObject.SetActive(false);
            _rightText.gameObject.SetActive(false);
        }

        protected override void CustomSetup()
        {
            _mainText.text = CardData.TextContent;
            _upperText.text = CardData.TextContent;
            _rightText.text = CardData.TextContent;
        }

        protected override void HandleFlipped()
        {
            _mainText.gameObject.SetActive(CardData.IsFaceUp);
            _upperText.gameObject.SetActive(false);
            _rightText.gameObject.SetActive(false);
        }

        protected override void HandleStackedHorizontally()
        {
            _mainText.gameObject.SetActive(false);
            _upperText.gameObject.SetActive(false);
            _rightText.gameObject.SetActive(true);
        }

        protected override void HandleStackedVertically()
        {
            _mainText.gameObject.SetActive(false);
            _upperText.gameObject.SetActive(true);
            _rightText.gameObject.SetActive(false);
        }
    }
}
