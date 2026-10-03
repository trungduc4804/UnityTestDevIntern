using System.Collections;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Quản lý Camera và các chuyển đổi góc nhìn (Single Responsibility)
/// </summary>
public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CameraManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("[CameraManager]");
                    instance = go.AddComponent<CameraManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerTransform;

    private Coroutine trackBallRoutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitReferences();
    }

    private void InitReferences()
    {
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        if (playerTransform == null)
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null) playerTransform = pc.transform;
        }
    }

    private void Start()
    {
        FollowPlayer();
    }

    /// <summary>
    /// Chuyển camera về bám theo nhân vật
    /// </summary>
    public void FollowPlayer()
    {
        if (virtualCamera != null && playerTransform != null)
        {
            virtualCamera.Follow = playerTransform;
        }
    }

    /// <summary>
    /// Bám theo bóng khi sút, sau khi bóng vào khung thành chờ delay rồi quay lại nhân vật
    /// </summary>
    public void TrackBall(Ball ball, Goal goal, float waitAfterGoal = 2f)
    {
        if (trackBallRoutine != null)
        {
            StopCoroutine(trackBallRoutine);
        }
        trackBallRoutine = StartCoroutine(FollowBallRoutine(ball, goal, waitAfterGoal));
    }

    private IEnumerator FollowBallRoutine(Ball ball, Goal goal, float waitAfterGoal)
    {
        if (virtualCamera != null && ball != null)
        {
            virtualCamera.Follow = ball.transform;
        }

        Rigidbody ballRb = ball != null ? ball.GetComponent<Rigidbody>() : null;
        float elapsed = 0f;
        float maxDuration = 4f;

        // Chờ bóng bay vào bên trong khung thành hoặc dừng lại
        while (ball != null && elapsed < maxDuration)
        {
            elapsed += Time.deltaTime;

            if (elapsed > 0.4f && goal != null)
            {
                float distToGoal = Vector3.Distance(ball.transform.position, goal.TargetPosition);
                bool hasStopped = ballRb != null && ballRb.velocity.magnitude < 0.3f;

                if (distToGoal <= 1.0f || hasStopped)
                {
                    break;
                }
            }

            yield return null;
        }

        // Kích hoạt pháo hoa tại khung thành
        if (goal != null)
        {
            goal.PlayConfetti();
        }

        // Đợi 2 giây ăn mừng bàn thắng
        yield return new WaitForSeconds(waitAfterGoal);

        // Quay lại nhân vật
        FollowPlayer();
    }
}
