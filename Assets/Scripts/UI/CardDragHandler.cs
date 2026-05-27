using System.Collections.Generic;
using Piles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class CardDragHandler
        : MonoBehaviour,
            IBeginDragHandler,
            IDragHandler,
            IEndDragHandler,
            IPointerClickHandler
    {
        // Z position khi đang kéo (gần camera nhất)
        private const float DRAG_Z = -10f;
        private const float CARD_Z_STEP = 0.01f; // Offset Z giữa các lá trong nhóm

        private CardView _cardView;

        private List<CardView> _dragGroup; // Nhóm bài đang kéo
        private Vector3 _dragOffset;
        private Camera _mainCamera;
        private InputAction _moveAction;
        private Pile _originalPile;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _moveAction = InputSystem.actions.FindAction("Player/Mouse Move");
            _cardView = GetComponent<CardView>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_cardView.CardData.IsFaceUp)
            {
                print("Cannot drag unflipped card");
                return;
            }

            _originalPile = GetComponentInParent<Pile>();

            // Lấy nhóm bài nếu đang ở Tableau
            _dragGroup = new List<CardView>();
            if (_originalPile is TableauPile tableau)
            {
                var index = tableau.IndexOf(_cardView);
                _dragGroup = tableau.GetCardsFrom(index);
            }
            else
            {
                _dragGroup.Add(_cardView);
            }

            // Chuyển nhóm bài lên DragLayer
            foreach (var c in _dragGroup)
            {
                _originalPile.RemoveCard(c);
            }

            // Tính offset để bài không nhảy về tâm con trỏ
            var mouseWorldPos = GetMouseWorldPos();
            _dragOffset = transform.position - mouseWorldPos;
            print("Mouse pos: " + mouseWorldPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragGroup == null || _dragGroup.Count == 0)
            {
                return;
            }

            var targetPos = GetMouseWorldPos() + _dragOffset;
            targetPos.z = 0;
            // targetPos.z = DRAG_Z;

            _dragGroup[0].transform.position = targetPos;

            // Kéo cả nhóm theo, offset dọc + z
            for (var i = 1; i < _dragGroup.Count; i++)
            {
                _dragGroup[i].transform.position = new Vector3(
                    targetPos.x,
                    targetPos.y - i * 0.3f, // offset dọc giữa các lá
                    DRAG_Z + i * CARD_Z_STEP
                );

                _dragGroup[i].SetSortingOrder(100 + i);
            }

            _dragGroup[0].SetSortingOrder(100);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragGroup == null)
            {
                return;
            }

            var targetPile = FindTargetPile();

            if (targetPile != null && targetPile.CanAccept(_dragGroup[0]))
            {
                // Drop thành công
                foreach (var c in _dragGroup)
                {
                    targetPile.AddCard(c);
                }

                // Lật lá trên cùng của pile cũ
                if (_originalPile is TableauPile tp && tp.TopCard != null)
                {
                    tp.TopCard.FlipFaceUp();
                }
            }
            else
            {
                // Trả về chỗ cũ
                foreach (var c in _dragGroup)
                {
                    _originalPile.AddCard(c);
                }
            }

            _dragGroup = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Double click → tự động gửi lên Foundation nếu được
            if (eventData.clickCount == 2)
            {
                // GameManager.Instance.TryAutoMove(_cardView);
            }
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePosition = _moveAction.ReadValue<Vector2>();
            return _mainCamera.ScreenToWorldPoint(mousePosition);
        }

        private Pile FindTargetPile()
        {
            // Raycast 2D để tìm pile đang hover
            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(_moveAction.ReadValue<Vector2>());
            var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (var hit in hits)
            {
                var pile = hit.collider.GetComponent<Pile>();
                if (pile != null && pile != _originalPile)
                {
                    return pile;
                }
            }

            return null;
        }
    }
}
