using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    private Camera _camera;
    private InputAction _moveAction;

    private void Awake()
    {
        _camera = Camera.main;
        _moveAction = InputSystem.actions.FindAction("Player/Mouse Move");
    }

    private void Update()
    {
        var worldPosition = _camera.ScreenToWorldPoint(_moveAction.ReadValue<Vector2>());
        worldPosition.z = 0;
        transform.position = worldPosition;
    }
}
