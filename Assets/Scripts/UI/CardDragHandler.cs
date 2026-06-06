using Core;
using Interfaces;
using Piles;
using Solitaire;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    [RequireComponent(typeof(CardView), typeof(BoxCollider2D))]
    public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private readonly Collider2D[] _overlapResults = new Collider2D[20];

        private readonly int MaxSortingOrder = 100;

        private BoxCollider2D _boxCollider2D;
        protected CardView _cardView;
        private ContactFilter2D _contactFilter;

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
            _contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = Const.Layers.MaskGlower,
                // useTriggers = true,
            };
        }

        public void OnBeginDrag(PointerEventData eventData)
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

            HandleBeginDrag();

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

                if (_currentGlower != null && _currentGlower.GetPile() is not WastePile)
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

        protected virtual void HandleBeginDrag()
        {
            _originalPile.RemoveCard(_cardView);
            _originalPile.ArrangeCards();
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
            var hitCount = _boxCollider2D.Overlap(_contactFilter, _overlapResults);
            Collider2D bestCollider2D = null;
            var maxOverlap = 0f;

            for (var i = 0; i < hitCount; i++)
            {
                var hit = _overlapResults[i];

                if (
                    hit == null
                    || hit == _boxCollider2D
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
