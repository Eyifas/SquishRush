using UnityEngine;
using System.Collections;

public class CloudDisappear : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D cloudCollider;

    private bool playerTouching = false;
    private bool cycleRunning = false;

    private float touchTimer = 0f;
    private const float disappearTime = 1.5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cloudCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (playerTouching && !cycleRunning)
        {
            touchTimer += Time.deltaTime;

            // Temas ettiği sürece 2 saniyede saydamlaş
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(1f, 0f, touchTimer / disappearTime);
            spriteRenderer.color = color;

            if (touchTimer >= disappearTime)
            {
                StartCoroutine(DisappearAndReturn());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTouching = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !cycleRunning)
        {
            playerTouching = false;
            touchTimer = 0f;

            // 2 saniye dolmadan ayrıldıysa tekrar tamamen görünür
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

    private IEnumerator DisappearAndReturn()
    {
        cycleRunning = true;
        playerTouching = false;

        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        cloudCollider.enabled = false;

        // 1 saniye tamamen yok
        yield return new WaitForSeconds(1f);

        // 1 saniyede geri gelsin
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, timer);
            spriteRenderer.color = color;

            yield return null;
        }

        color.a = 1f;
        spriteRenderer.color = color;

        cloudCollider.enabled = true;

        touchTimer = 0f;
        cycleRunning = false;
    }
}