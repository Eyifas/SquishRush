using UnityEngine;

public class SuMermisi : MonoBehaviour
{
    private GameObject owner;

    // Mermiyi kimin attığını kaydeder
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Mermi kendi sahibine değerse hiçbir şey yapma
        if (owner != null && other.transform.root.gameObject == owner)
        {
            return;
        }

        // Marshmallow'a değdiyse stun
        MarshmallowMovement marshmallow =
            other.GetComponentInParent<MarshmallowMovement>();

        if (marshmallow != null)
        {
            marshmallow.Stun();
            Destroy(gameObject);
            return;
        }

        // Hamur'a değdiyse stun
        HamurMovement hamur =
            other.GetComponentInParent<HamurMovement>();

        if (hamur != null)
        {
            hamur.Stun();
            Destroy(gameObject);
            return;
        }
    }
}