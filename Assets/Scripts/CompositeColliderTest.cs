using UnityEngine;

namespace DefaultNamespace
{
    public class CompositeColliderTest : MonoBehaviour
    {
        private void Start()
        {
            var composite = GetComponent<CompositeCollider2D>();

            Debug.Log("Số path (shape) riêng biệt: " + composite.pathCount);
            Debug.Log("Tổng số điểm: " + composite.pointCount);

            // Nếu pathCount = 1 → gộp thành công thành 1 shape
            // Nếu pathCount = 3 → vẫn còn 3 shape riêng biệt
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Collision detected with: " + other.gameObject.name);
        }
    }
}
