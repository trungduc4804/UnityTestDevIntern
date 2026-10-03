using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Sút bóng bay thẳng về phía mục tiêu với một lực nhất định
    /// </summary>
    public void KickTowards(Vector3 targetPosition, float kickForce = 15f)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        // Tính hướng từ quả bóng đến khung thành
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Đặt vận tốc bóng bay thẳng theo hướng đó
        rb.velocity = direction * kickForce;
    }
}
