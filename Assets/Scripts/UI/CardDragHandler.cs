using System.Collections.Generic;
using Core;
using Interfaces;
using Piles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    [RequireComponent(typeof(CardView), typeof(BoxCollider2D))]
    public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private readonly int MaxSortingOrder = 100;

        private BoxCollider2D _boxCollider2D;
        protected CardView _cardView;
        private IGlower _currentGlower;

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
            _cardView.SetSortingOrder(MaxSortingOrder);
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

            var currentGlower = FindTargetPile();
            if (currentGlower != _currentGlower)
            {
                if (_currentGlower != null)
                {
                    _currentGlower.SetGlowBorders(false);
                }

                _currentGlower = currentGlower;

                if (_currentGlower != null)
                {
                    _currentGlower.SetGlowBorders(true);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                return;
            }

            _cardView.SetGlowBorders(false);

            Pile targetPile = null;
            if (_currentGlower != null)
            {
                targetPile = _currentGlower.GetPile();
                _currentGlower.SetGlowBorders(false);
            }

            print("Drop on: " + targetPile);

            HandleDrop(targetPile);
        }

        protected virtual void HandleDrop(Pile targetPile)
        {
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

        private IGlower FindTargetPile()
        {
            // 1. Lấy vị trí tâm chuẩn trong không gian thế giới (đã tính cả Offset của Collider)
            Vector2 center = _boxCollider2D.bounds.center;

            // Đơn giản hơn và chính xác hơn:
            var size = _boxCollider2D.bounds.size;

            // 3. Lấy góc quay hiện tại của GameObject (Tính theo trục Z trong 2D)
            var angle = transform.eulerAngles.z;

            // 4. Truyền tất cả vào hàm OverlapBox
            // var hits = Physics2D.OverlapBoxAll(center, size, angle);
            var hits = Physics2D.OverlapBoxAll(center, size, 0f);

            Collider2D bestCollider2D = null;
            var maxOverlap = 0f;

            // Duyệt kết quả
            foreach (var hit in hits)
            {
                // Tránh việc hộp tự quét trúng chính nó
                if (
                    hit == _boxCollider2D
                    || hit == null
                    || hit.gameObject == _originalPile.gameObject
                )
                {
                    continue;
                }

                var overlap = CalculateOverlapArea(_boxCollider2D.bounds, hit.bounds);
                if (overlap > maxOverlap)
                {
                    maxOverlap = overlap;
                    bestCollider2D = hit;
                }
            }

            if (bestCollider2D == null)
            {
                return null;
            }

            return bestCollider2D.GetComponent<IGlower>();
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

        private float CalculateOverlapArea(Bounds a, Bounds b)
        {
            var overlapX = Mathf.Min(a.max.x, b.max.x) - Mathf.Max(a.min.x, b.min.x);
            var overlapY = Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y);

            if (overlapX <= 0 || overlapY <= 0)
            {
                return 0f; // Không overlap
            }

            return overlapX * overlapY;
        }
    }
}
