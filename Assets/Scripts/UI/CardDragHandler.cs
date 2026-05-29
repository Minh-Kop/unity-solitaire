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
        [SerializeField]
        private float _cardYOffset = -0.3f;

        private readonly int MaxSortingOrder = 100;

        private BoxCollider2D _boxCollider2D;
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
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_cardView.CardData.IsFaceUp)
            {
                print("Cannot drag unflipped card");
                return;
            }

            _originalPile = GetComponentInParent<Pile>();
            print("Original pile: " + _originalPile);

            if (_originalPile is FoundationPile)
            {
                return;
            }

            // Lấy nhóm bài nếu đang ở Tableau
            _dragGroup = new List<CardView>();
            if (_originalPile is TableauPile tableau)
            {
                _dragGroup = tableau.GetCardsFromLastToFirstFaceDown();
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

            _originalPile.ArrangeCards();

            // Tính offset để bài không nhảy về tâm con trỏ
            var mouseWorldPos = GetMouseWorldPos();
            _dragOffset = transform.position - mouseWorldPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragGroup == null || _dragGroup.Count == 0)
            {
                return;
            }

            var targetPos = GetMouseWorldPos() + _dragOffset;
            targetPos.z = 0;

            _dragGroup[^1].transform.position = targetPos;
            _dragGroup[^1].SetSortingOrder(MaxSortingOrder);

            // Kéo cả nhóm theo, offset dọc + z
            for (var i = 1; i < _dragGroup.Count; i++)
            {
                var index = _dragGroup.Count - 1 - i;
                _dragGroup[index].transform.position = new Vector2(
                    targetPos.x,
                    targetPos.y - i * _cardYOffset // offset dọc giữa các lá
                );

                _dragGroup[index].SetSortingOrder(MaxSortingOrder - i);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragGroup == null)
            {
                return;
            }

            var targetPile = FindTargetPile();
            print("Drop on: " + targetPile);

            if (targetPile != null && targetPile.CanAccept(_dragGroup[^1]))
            {
                // Drop thành công
                foreach (var c in _dragGroup)
                {
                    targetPile.AddCard(c);
                }

                targetPile.ArrangeCards();

                // Lật lá trên cùng của pile cũ
                if (_originalPile is TableauPile tp && tp.TopCard != null)
                {
                    tp.TopCard.FlipFaceUp();
                }

                GameManager.MoveCount--;
            }
            else
            {
                // Trả về chỗ cũ
                foreach (var c in _dragGroup)
                {
                    _originalPile.AddCard(c);
                }

                _originalPile.ArrangeCards();
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

        private Pile FindTargetPile_()
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

        private Pile FindTargetPile()
        {
            // 1. Lấy vị trí tâm chuẩn trong không gian thế giới (đã tính cả Offset của Collider)
            Vector2 center = _boxCollider2D.bounds.center;

            // 2. Lấy kích thước chuẩn (đã nhân với hệ số Scale của Transform)
            // Chúng ta lấy từ size của collider nhân với lossyScale của GameObject
            var size = new Vector2(
                _boxCollider2D.size.x * transform.lossyScale.x,
                _boxCollider2D.size.y * transform.lossyScale.y
            );

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
                    return pile;
                }
            }

            return null;
        }
    }
}
