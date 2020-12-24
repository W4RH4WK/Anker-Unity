using UnityEngine;
using UnityEngine.Assertions;

public class PlatformTracker : MonoBehaviour
{
    public bool HasContact => Contacts > 0;

    public void DropThrough()
    {
        ColliderDisableTimeLeft = ColliderDisableTime;
    }

    int Contacts;

    Collider2D Collider;

    float ColliderDisableTime = 0.2f;
    float ColliderDisableTimeLeft;

    void Awake()
    {
        Collider = GetComponent<Collider2D>();
        Assert.IsNotNull(Collider);
    }

    void FixedUpdate()
    {
        ColliderDisableTimeLeft -= Time.fixedDeltaTime;

        Collider.enabled = ColliderDisableTimeLeft <= 0.0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Contacts++;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Contacts--;
    }
}
