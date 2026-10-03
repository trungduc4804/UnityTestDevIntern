using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    public Rigidbody Rb
    {
        get
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            return rb;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
