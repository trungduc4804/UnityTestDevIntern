using UnityEngine;

/// <summary>
/// Quản lý logic sút bóng vào khung thành (Single Responsibility)
/// </summary>
public class BallKicker : MonoBehaviour
{
    [Header("Kick Settings")]
    [SerializeField] private float kickForce = 15f;

    /// <summary>
    /// Sút một quả bóng cụ thể vào khung thành gần nó nhất
    /// </summary>
    public void KickBall(Ball ball)
    {
        if (ball == null) return;

        Goal targetGoal = Goal.GetNearest(ball.transform.position);
        if (targetGoal == null) return;

        // 1. Sút bóng bay về khung thành
        ball.KickTowards(targetGoal.TargetPosition, kickForce);

        // 2. Báo cho CameraManager bám theo quả bóng
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.TrackBall(ball, targetGoal);
        }
    }

    /// <summary>
    /// Tìm quả bóng xa vị trí origin nhất và tự động sút vào khung thành
    /// </summary>
    public void AutoKickFurthestBall(Vector3 origin)
    {
        Ball[] allBalls = FindObjectsOfType<Ball>();
        if (allBalls == null || allBalls.Length == 0) return;

        Ball furthestBall = allBalls[0];
        float maxDistance = Vector3.Distance(origin, furthestBall.transform.position);

        for (int i = 1; i < allBalls.Length; i++)
        {
            if (allBalls[i] == null) continue;
            float dist = Vector3.Distance(origin, allBalls[i].transform.position);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                furthestBall = allBalls[i];
            }
        }

        if (furthestBall != null)
        {
            KickBall(furthestBall);
        }
    }
}
