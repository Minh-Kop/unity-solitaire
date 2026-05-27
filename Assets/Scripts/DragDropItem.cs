using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Transform _mousePointer;

    [SerializeField]
    private Transform _slot;

    private bool _isPlacedInSlot;

    private Vector2 _originalPosition;

    private void Awake()
    {
        _originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isPlacedInSlot)
        {
            transform.position = _mousePointer.position;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isPlacedInSlot)
        {
            if (Vector2.Distance(transform.position, _slot.position) < 2f)
            {
                _isPlacedInSlot = true;
                transform.parent = _slot;
                transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.position = _originalPosition;
            }
        }
    }
}
