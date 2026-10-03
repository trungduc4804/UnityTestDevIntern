using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý thông tin và hiệu ứng của Khung thành (Single Responsibility)
/// </summary>
public class Goal : MonoBehaviour
{
    // Danh sách đăng ký tĩnh tất cả khung thành trong scene (Registry Pattern)
    public static readonly List<Goal> AllGoals = new List<Goal>();

    [Header("Goal Settings")]
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private ParticleSystem confettiEffect;

    public Vector3 TargetPosition => transform.position + targetOffset;

    private void OnEnable()
    {
        if (!AllGoals.Contains(this))
        {
            AllGoals.Add(this);
        }
    }

    private void OnDisable()
    {
        AllGoals.Remove(this);
    }

    /// <summary>
    /// Tìm khung thành gần một vị trí nhất
    /// </summary>
    public static Goal GetNearest(Vector3 position)
    {
        if (AllGoals == null || AllGoals.Count == 0)
        {
            // Fallback nếu scene chưa kịp load OnEnable
            Goal[] found = FindObjectsOfType<Goal>();
            AllGoals.AddRange(found);
        }

        if (AllGoals.Count == 0) return null;

        Goal nearest = AllGoals[0];
        float minDistance = Vector3.Distance(position, nearest.TargetPosition);

        for (int i = 1; i < AllGoals.Count; i++)
        {
            if (AllGoals[i] == null) continue;
            float dist = Vector3.Distance(position, AllGoals[i].TargetPosition);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = AllGoals[i];
            }
        }

        return nearest;
    }

    /// <summary>
    /// Kích hoạt hiệu ứng pháo hoa Confetti tại vị trí khung thành
    /// </summary>
    public void PlayConfetti()
    {
        if (confettiEffect != null && !confettiEffect.isPlaying)
        {
            confettiEffect.Play(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Khi bóng lăn vào trigger của lưới khung thành
        if (other.GetComponent<Ball>() != null)
        {
            PlayConfetti();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(TargetPosition, 0.6f);
    }
}
