using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HamurMovement : MonoBehaviour
{
    public bool IsStunned => stunTime > 0f;

    private Vector2 moveInput;
    private int suCount = 0;
    private float facingDirection = 1f;
    private float stunTime = 0f;

    [Header("Movement")]
    [SerializeField] private Rigidbody2D hamurRigibody;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Water")]
    [SerializeField] private TMP_Text HamurSu;
    [SerializeField] private GameObject suMermisiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 10f;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (moveInput.x > 0)
        {
            facingDirection = 1f;

            throwPoint.localPosition =
                new Vector3(1f, 0f, 0f);

            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            facingDirection = -1f;

            throwPoint.localPosition =
                new Vector3(-1f, 0f, 0f);

            spriteRenderer.flipX = true;
        }
    }


    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded && stunTime <= 0f)
        {
            hamurRigibody.linearVelocity =
                new Vector2(
                    hamurRigibody.linearVelocity.x,
                    jumpSpeed
                );
        }
    }


    public void OnThrow(InputValue value)
    {
        if (!value.isPressed || suCount <= 0 || stunTime > 0f)
        {
            return;
        }

        // Su mermisini oluştur
        GameObject suMermisi = Instantiate(
            suMermisiPrefab,
            throwPoint.position,
            Quaternion.identity
        );


        // Merminin sahibini Hamur olarak ayarla
        SuMermisi mermiScript =
            suMermisi.GetComponent<SuMermisi>();

        if (mermiScript != null)
        {
            mermiScript.SetOwner(gameObject);
        }


        // Rigidbody'yi bul
        Rigidbody2D suRb =
            suMermisi.GetComponent<Rigidbody2D>();

        if (suRb != null)
        {
            suRb.linearVelocity =
                Vector2.right * facingDirection * throwForce;
        }


        // Su azalt
        suCount--;

        HamurSu.text = suCount.ToString();


        // 3 saniye sonra mermiyi yok et
        Destroy(suMermisi, 3f);
    }


    public void Stun()
    {
        stunTime = 3f;
    }


    private void FixedUpdate()
    {
        // Yerde mi?
        isGrounded =
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            ) != null;


        // STUN
        if (stunTime > 0f)
        {
            stunTime -= Time.fixedDeltaTime;

            hamurRigibody.linearVelocity =
                new Vector2(
                    0f,
                    hamurRigibody.linearVelocity.y
                );

            UpdateAnimations();

            return;
        }


        // HAREKET
        hamurRigibody.linearVelocity =
            new Vector2(
                moveInput.x * moveSpeed,
                hamurRigibody.linearVelocity.y
            );


        UpdateAnimations();
    }


    private void UpdateAnimations()
    {
        bool isWalking =
            Mathf.Abs(hamurRigibody.linearVelocity.x) > 0.1f &&
            isGrounded &&
            stunTime <= 0f;

        animator.SetBool(
            "isWalking",
            isWalking
        );

        animator.SetBool(
            "isGrounded",
            isGrounded
        );
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ateş
        if (other.CompareTag("fire"))
        {
            hamurRigibody.position =
                respawnPoint.position;

            hamurRigibody.linearVelocity =
                Vector2.zero;

            stunTime = 0f;
            isGrounded = false;

            return;
        }


        // Su toplama
        if (other.CompareTag("Su"))
        {
            suCount++;

            HamurSu.text = suCount.ToString();

            Destroy(other.gameObject);
        }
    }
}