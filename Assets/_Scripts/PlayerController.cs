using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [Header("Kick & Ball Detection")]
    [SerializeField] private float kickDistance = 2.5f;
    [SerializeField] private GameObject kickButton;
    [SerializeField] private LayerMask ballLayer = ~0;
    [Tooltip("Lực sút bóng")]
    [SerializeField] private float kickForce = 15f;

    private Rigidbody rb;
    private Animator animator;
    private Transform nearbyBallTarget;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Đảm bảo ban đầu nút Kick ẩn đi và tự động gán sự kiện click
        if (kickButton != null)
        {
            kickButton.SetActive(false);

            Button btn = kickButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(Kick);
                btn.onClick.AddListener(Kick);
            }
        }
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
            animator.SetBool("isRun",true);
        }else{
            animator.SetBool("isRun",false);
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
            // Nhận diện qua component Ball, Tag "Ball", hoặc tên GameObject chứa "ball"
            bool isBall = hit.GetComponent<Ball>() != null;

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

    [SerializeField] private Transform[] goals;

    public void Kick()
    {
        if (nearbyBallTarget == null) return;

        Ball ball = nearbyBallTarget.GetComponent<Ball>();
        if (ball == null) return;

        // Tìm khung thành gần quả bóng nhất
        Transform nearestGoal = GetNearestGoal(ball.transform.position);
        if (nearestGoal != null)
        {
            // Sút bóng bay về khung thành đó
            ball.KickTowards(nearestGoal.position, kickForce);
        }
    }

    private Transform GetNearestGoal(Vector3 ballPos)
    {
        // Nếu chưa kéo 2 khung thành vào Inspector, tự tìm qua component Goal
        if (goals == null || goals.Length == 0)
        {
            Goal[] foundGoals = FindObjectsOfType<Goal>();
            if (foundGoals != null && foundGoals.Length > 0)
            {
                goals = new Transform[foundGoals.Length];
                for (int i = 0; i < foundGoals.Length; i++)
                {
                    goals[i] = foundGoals[i].transform;
                }
            }
        }

        if (goals == null || goals.Length == 0) return null;

        // So sánh khoảng cách để lấy khung thành gần nhất
        Transform nearest = goals[0];
        float minDistance = Vector3.Distance(ballPos, nearest.position);

        for (int i = 1; i < goals.Length; i++)
        {
            if (goals[i] == null) continue;
            float dist = Vector3.Distance(ballPos, goals[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = goals[i];
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
