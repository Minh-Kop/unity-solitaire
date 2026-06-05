using Core;
using Piles;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(ListCardView))]
    public class ListCardDragHandler : CardDragHandler
    {
        private ListCardView _listCardView;

        private Vector3 _originalPosition;
        private int _originalSortingOrder;
        private TableauPile _tableauPile;

        protected override void Awake()
        {
            base.Awake();
            _listCardView = (ListCardView)_cardView;
            _tableauPile = GetComponentInParent<TableauPile>();
        }

        protected override bool CheckIfCanDrag()
        {
            if (!_listCardView.IsDraggingAllowed)
            {
                print("Cannot drag empty list card");
                _isDragging = false;
            }
            else
            {
                _isDragging = true;
            }

            return _isDragging;
        }

        protected override void HandleBeginDrag()
        {
            _originalSortingOrder = _listCardView.GetSortingOrder();
            _originalPosition = _listCardView.transform.position;
        }

        protected override void HandleDrop(Pile targetPile)
        {
            if (targetPile != null && targetPile.CanAccept(_listCardView.TopCard))
            {
                // Drop thành công
                _listCardView.TransferCards(targetPile);

                // Lật lá trên cùng của pile cũ
                _tableauPile.FlipTopCard();

                GameManager.MoveCount--;
            }
            else
            {
                _listCardView.transform.position = _originalPosition;
                _listCardView.SetSortingOrder(_originalSortingOrder);
            }
        }
    }
}
