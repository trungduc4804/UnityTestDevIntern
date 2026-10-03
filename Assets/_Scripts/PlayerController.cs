using UnityEngine;

/// <summary>
/// Facade / Coordinator kết nối các thành phần độc lập của nhân vật (SOLID Design)
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(BallDetector))]
[RequireComponent(typeof(BallKicker))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private BallDetector ballDetector;
    private BallKicker kicker;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>() ?? gameObject.AddComponent<PlayerMovement>();
        ballDetector = GetComponent<BallDetector>() ?? gameObject.AddComponent<BallDetector>();
        kicker = GetComponent<BallKicker>() ?? gameObject.AddComponent<BallKicker>();
    }

    /// <summary>
    /// Hàm gọi khi bấm nút Kick trên UI (hoặc gán sự kiện OnClick)
    /// </summary>
    public void Kick()
    {
        if (ballDetector != null && kicker != null)
        {
            kicker.KickBall(ballDetector.CurrentNearbyBall);
        }
    }

    /// <summary>
    /// Hàm gọi khi bấm nút Auto Kick trên UI (hoặc gán sự kiện OnClick)
    /// </summary>
    public void AutoKick()
    {
        if (kicker != null)
        {
            kicker.AutoKickFurthestBall(transform.position);
        }
    }
}
