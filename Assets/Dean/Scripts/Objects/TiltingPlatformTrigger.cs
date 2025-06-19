using UnityEngine;

public class TiltingPlatformTrigger : MonoBehaviour
{
    [SerializeField] private Animator tiltingPlatform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tiltingPlatform.SetTrigger("TiltTrigger");
        }
    }
}
