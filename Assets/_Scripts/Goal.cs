using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.2f, 0f);

    public Vector3 TargetPosition => transform.position + targetOffset;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(TargetPosition, 0.6f);
    }
}
