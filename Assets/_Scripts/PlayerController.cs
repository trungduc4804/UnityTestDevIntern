using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [Header("Kick & Ball Detection")]
    [SerializeField] private float kickDistance = 2.5f;
    [SerializeField] private GameObject kickButton;
    [SerializeField] private LayerMask ballLayer = ~0;

    private Rigidbody rb;
    private Animator animator;
    private Transform nearbyBallTarget;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Đảm bảo ban đầu nút Kick ẩn đi
        if (kickButton != null)
        {
            kickButton.SetActive(false);
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

    private void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn phát hiện quả bóng trong Scene view để trực quan căn chỉnh
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, kickDistance);
    }
}
