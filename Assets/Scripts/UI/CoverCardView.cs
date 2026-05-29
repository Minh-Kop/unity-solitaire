using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CoverCardView : CardView
    {
        [SerializeField]
        private TextMeshPro _countText;

        [SerializeField]
        private TextMeshPro _mainText;

        [SerializeField]
        private TextMeshPro _rightText;

        private CoverCard _coverCard;

        protected override void Awake()
        {
            base.Awake();
            _mainText.gameObject.SetActive(true);
            _countText.gameObject.SetActive(true);
            _rightText.gameObject.SetActive(false);
        }

        private void Update()
        {
            _countText.text = $"{_coverCard.Count}/{_coverCard.MaxCount}";
        }

        protected override void CustomSetup()
        {
            _coverCard = (CoverCard)CardData;

            _mainText.text = _coverCard.Type;
            _rightText.text = _coverCard.Type;
            _countText.text = $"{_coverCard.Count}/{_coverCard.MaxCount}";
        }

        protected override void HandleFlipped()
        {
            _mainText.gameObject.SetActive(CardData.IsFaceUp);
            _countText.gameObject.SetActive(CardData.IsFaceUp);
            _rightText.gameObject.SetActive(false);
        }

        protected override void HandleStackedHorizontally()
        {
            _mainText.gameObject.SetActive(false);
            _countText.gameObject.SetActive(false);
            _rightText.gameObject.SetActive(true);
        }

        protected override void HandleStackedVertically() { }
    }
}
