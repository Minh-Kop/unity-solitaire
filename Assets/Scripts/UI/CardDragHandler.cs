using System.Collections.Generic;
using Core;
using Piles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    [RequireComponent(typeof(CardView), typeof(BoxCollider2D))]
    public class CardDragHandler
        : MonoBehaviour,
            IBeginDragHandler,
            IDragHandler,
            IEndDragHandler,
            IPointerClickHandler
    {
        private readonly int MaxSortingOrder = 100;

        private BoxCollider2D _boxCollider2D;
        protected CardView _cardView;

        private List<CardView> _dragGroup; // Nhóm bài đang kéo
        private Vector3 _dragOffset;
        protected bool _isDragging;

        private Camera _mainCamera;
        private InputAction _moveAction;
        private Pile _originalPile;

        protected virtual void Awake()
        {
            _mainCamera = Camera.main;
            _moveAction = InputSystem.actions.FindAction("Player/Mouse Move");
            _cardView = GetComponent<CardView>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!CheckIfCanDrag())
            {
                return;
            }

            _originalPile = GetComponentInParent<Pile>();

            print("Original pile: " + _originalPile);

            if (_originalPile is FoundationPile)
            {
                _isDragging = false;
                return;
            }

            // Tính offset để bài không nhảy về tâm con trỏ
            var mouseWorldPos = GetMouseWorldPos();
            _dragOffset = transform.position - mouseWorldPos;

            _cardView.SetGlowBorders(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                return;
            }

            var targetPos = GetMouseWorldPos() + _dragOffset;
            targetPos.z = 0;

            _cardView.transform.position = targetPos;
            _cardView.SetSortingOrder(MaxSortingOrder);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                return;
            }

            // var targetPile = FindTargetPile();
            var targetPile = FindTargetPile(eventData);
            print("Drop on: " + targetPile);

            if (targetPile != null && targetPile.CanAccept(_cardView))
            {
                // Drop thành công
                targetPile.AddCard(_cardView);
                targetPile.ArrangeCards();

                GameManager.MoveCount--;
            }
            else
            {
                // Trả về chỗ cũ
                _originalPile.AddCard(_cardView);
                _originalPile.ArrangeCards();
            }

            _dragGroup = null;
            _cardView.SetGlowBorders(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Double click → tự động gửi lên Foundation nếu được
            if (eventData.clickCount == 2)
            {
                // GameManager.Instance.TryAutoMove(_cardView);
            }
        }

        protected virtual bool CheckIfCanDrag()
        {
            if (!_cardView.CardData.IsFaceUp)
            {
                print("Cannot drag unflipped card");
                _isDragging = false;
            }
            else
            {
                _isDragging = true;
            }

            return _isDragging;
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePosition = _moveAction.ReadValue<Vector2>();
            return _mainCamera.ScreenToWorldPoint(mousePosition);
        }

        protected Pile FindTargetPile()
        {
            // 1. Lấy vị trí tâm chuẩn trong không gian thế giới (đã tính cả Offset của Collider)
            Vector2 center = _boxCollider2D.bounds.center;

            // Đơn giản hơn và chính xác hơn:
            var size = _boxCollider2D.bounds.size;

            // 3. Lấy góc quay hiện tại của GameObject (Tính theo trục Z trong 2D)
            var angle = transform.eulerAngles.z;

            // 4. Truyền tất cả vào hàm OverlapBox
            var hits = Physics2D.OverlapBoxAll(center, size, angle);

            // Duyệt kết quả
            foreach (var hit in hits)
            {
                // Tránh việc hộp tự quét trúng chính nó
                if (hit == _boxCollider2D)
                {
                    continue;
                }

                var pile = hit.GetComponent<Pile>() ?? hit.GetComponentInParent<Pile>();
                if (pile != null && pile != _originalPile)
                {
                    print("Pile: " + pile);
                    return pile;
                }
            }

            print("Pile: NULL");
            return null;
        }

        protected Pile FindTargetPile(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                var pile = result.gameObject.GetComponentInParent<Pile>();
                if (pile != null && pile != _originalPile)
                {
                    return pile;
                }
            }

            return null;
        }
    }
}
