using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float kickDistance = 2.5f;
    [SerializeField] private GameObject kickButton;
    [SerializeField] private LayerMask ballLayer = ~0;
    [SerializeField] private float kickForce = 15f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float cameraSmoothSpeed = 8f;
    [SerializeField] private Transform[] goals;

    private Rigidbody rb;
    private Animator animator;
    private Transform nearbyBallTarget;
    private Vector3 cameraOffset;
    private Transform cameraTarget;
    private Coroutine cameraRoutine;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        CheckNearbyBall();
    }

    private void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        if(moveDir != Vector3.zero)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private void CheckNearbyBall()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, kickDistance, ballLayer);
        Transform nearestBall = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            bool isBall = hit.GetComponent<Ball>();

            if (isBall)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestBall = hit.transform;
                }
            }
        }

        nearbyBallTarget = nearestBall;

        // Ẩn/hiện button Kick trên màn hình
        if (kickButton != null)
        {
            bool shouldShow = nearbyBallTarget != null;
            if (kickButton.activeSelf != shouldShow)
            {
                kickButton.SetActive(shouldShow);
            }
        }
    }

    public void Kick()
    {
        if (nearbyBallTarget == null) return;

        Ball ball = nearbyBallTarget.GetComponent<Ball>();
        if (ball == null)
        {
            ball = nearbyBallTarget.gameObject.AddComponent<Ball>();
        }

        // Tìm khung thành gần quả bóng nhất
        Transform nearestGoal = GetNearestGoal(ball.transform.position);
        if (nearestGoal != null)
        {
            // Sút bóng bay về khung thành đó
            ball.KickTowards(nearestGoal.position, kickForce);

            // Chuyển Camera bám theo quả bóng
            SwitchCameraToBall(ball, nearestGoal);
        }
    }

    public void AutoKick()
    {
        Ball[] allBalls = FindObjectsOfType<Ball>();
        if (allBalls == null || allBalls.Length == 0) return;

        // Tìm quả bóng xa nhân vật nhất
        Ball furthestBall = allBalls[0];
        float maxDistance = Vector3.Distance(transform.position, furthestBall.transform.position);

        for (int i = 1; i < allBalls.Length; i++)
        {
            if (allBalls[i] == null) continue;
            float dist = Vector3.Distance(transform.position, allBalls[i].transform.position);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                furthestBall = allBalls[i];
            }
        }

        if (furthestBall != null)
        {
            Transform nearestGoal = GetNearestGoal(furthestBall.transform.position);
            if (nearestGoal != null)
            {
                furthestBall.KickTowards(nearestGoal.position, kickForce);

                // Chuyển Camera bám theo quả bóng
                SwitchCameraToBall(furthestBall, nearestGoal);
            }
        }
    }

    private void SwitchCameraToBall(Ball ball, Transform goal)
    {
        if (cameraRoutine != null)
        {
            StopCoroutine(cameraRoutine);
        }
        cameraRoutine = StartCoroutine(FollowBallRoutine(ball, goal));
    }

    private IEnumerator FollowBallRoutine(Ball ball, Transform goal)
    {
        // 1. Chuyển mục tiêu theo dõi sang quả bóng
        if (virtualCamera != null)
        {
            virtualCamera.Follow = ball.transform;
        }
        cameraTarget = ball.transform;

        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        float elapsed = 0f;
        float maxDuration = 4f;

        // Chờ bóng bay tới gần khung thành hoặc dừng lại (tối đa 4s)
        while (ball != null && elapsed < maxDuration)
        {
            elapsed += Time.deltaTime;

            // Sau 0.4s để bóng bay được một đoạn nhất định
            if (elapsed > 0.4f)
            {
                float distToGoal = Vector3.Distance(ball.transform.position, goal.position);
                bool hasStopped = ballRb != null && ballRb.velocity.magnitude < 0.6f;

                if (distToGoal <= 3.5f || hasStopped)
                {
                    break;
                }
            }

            yield return null;
        }

        // 2. Bóng đã tới khung thành -> Đợi đúng 2 giây
        yield return new WaitForSeconds(2f);

        // 3. Chuyển Camera quay trở lại nhân vật
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform;
        }
        cameraTarget = transform;
    }

    private Transform GetNearestGoal(Vector3 ballPos)
    {
        List<Transform> validGoals = new List<Transform>();

        // 1. Kiểm tra mảng goals nếu đã kéo trong Inspector
        if (goals != null && goals.Length > 0)
        {
            foreach (var g in goals)
            {
                if (g != null) validGoals.Add(g);
            }
        }

        // 2. Tìm component Goal trong scene
        if (validGoals.Count == 0)
        {
            Goal[] foundGoals = FindObjectsOfType<Goal>();
            foreach (var g in foundGoals)
            {
                if (g != null) validGoals.Add(g.transform);
            }
        }


        Transform nearest = validGoals[0];
        float minDistance = Vector3.Distance(ballPos, nearest.position);

        for (int i = 1; i < validGoals.Count; i++)
        {
            float dist = Vector3.Distance(ballPos, validGoals[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = validGoals[i];
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn phát hiện quả bóng trong Scene view để trực quan căn chỉnh
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, kickDistance);
    }
}
