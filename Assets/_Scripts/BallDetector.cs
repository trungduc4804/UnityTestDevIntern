using System;
using UnityEngine;

/// <summary>
/// Quản lý việc phát hiện quả bóng ở gần người chơi và bật/tắt UI (Single Responsibility)
/// </summary>
public class BallDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectDistance = 2.5f;
    [SerializeField] private LayerMask ballLayer = ~0;
    [SerializeField] private GameObject kickButton;

    public Ball CurrentNearbyBall { get; private set; }

    public event Action<Ball> OnBallFound;
    public event Action OnBallLost;

    private void Start()
    {
        if (kickButton == null)
        {
            GameObject btn = GameObject.Find("KickButton");
            if (btn == null) btn = GameObject.Find("Kick");
            if (btn != null) kickButton = btn;
        }
    }

    private void Update()
    {
        DetectNearbyBall();
    }

    private void DetectNearbyBall()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectDistance, ballLayer);
        Ball nearestBall = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            Ball ball = hit.GetComponent<Ball>();
            if (ball != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestBall = ball;
                }
            }
        }

        // Cập nhật trạng thái bóng hiện tại
        if (nearestBall != CurrentNearbyBall)
        {
            CurrentNearbyBall = nearestBall;
            if (CurrentNearbyBall != null)
            {
                OnBallFound?.Invoke(CurrentNearbyBall);
            }
            else
            {
                OnBallLost?.Invoke();
            }
        }

        // Cập nhật hiển thị nút Kick UI
        if (kickButton != null)
        {
            bool shouldShow = CurrentNearbyBall != null;
            if (kickButton.activeSelf != shouldShow)
            {
                kickButton.SetActive(shouldShow);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectDistance);
    }
}
