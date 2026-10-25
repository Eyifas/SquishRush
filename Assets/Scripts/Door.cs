using TMPro;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private HamurMovement hamur;
    [SerializeField] private MarshmallowMovement marshmallow;
    [SerializeField] private float kazanmaSuresi = 2f;

    [Header("Countdown UI")]
    [SerializeField] private TMP_Text geriSayimYazisi;

    [Header("Scene")]
    [SerializeField] private string endSceneName = "EndScene";


    private bool hamurIceride;
    private bool marshmallowIceride;
    private bool oyunBitti;

    private float hamurSure;
    private float marshmallowSure;

    private RectTransform countdownRect;
    private Canvas countdownCanvas;
    private Camera mainCamera;


    private void Start()
    {
        Time.timeScale = 1f;

        mainCamera = Camera.main;

        if (geriSayimYazisi != null)
        {
            countdownRect = geriSayimYazisi.rectTransform;
            countdownCanvas =
                geriSayimYazisi.GetComponentInParent<Canvas>();

            geriSayimYazisi.text = "";
            geriSayimYazisi.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Door: Geri Sayim Yazisi bağlanmamış!"
            );
        }
    }


    private void Update()
    {
        if (oyunBitti)
        {
            return;
        }


        // HAMUR
        if (hamurIceride && !hamur.IsStunned)
        {
            hamurSure += Time.deltaTime;
        }
        else
        {
            hamurSure = 0f;
        }


        // MARSHMALLOW
        if (marshmallowIceride && !marshmallow.IsStunned)
        {
            marshmallowSure += Time.deltaTime;
        }
        else
        {
            marshmallowSure = 0f;
        }


        UpdateCountdown();


        // BERABERE
        if (
            hamurSure >= kazanmaSuresi &&
            marshmallowSure >= kazanmaSuresi
        )
        {
            Bitir(MatchResult.Draw);
        }

        // HAMUR KAZANDI
        else if (hamurSure >= kazanmaSuresi)
        {
            Bitir(MatchResult.HamurWins);
        }

        // MARSHMALLOW KAZANDI
        else if (marshmallowSure >= kazanmaSuresi)
        {
            Bitir(MatchResult.MarshmallowWins);
        }
    }


    private void LateUpdate()
    {
        if (
            geriSayimYazisi == null ||
            countdownRect == null ||
            countdownCanvas == null ||
            !geriSayimYazisi.gameObject.activeSelf
        )
        {
            return;
        }


        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }


        Collider2D doorCollider =
            GetComponent<Collider2D>();


        Vector3 worldPosition;

        if (doorCollider != null)
        {
            worldPosition = new Vector3(
                doorCollider.bounds.center.x,
                doorCollider.bounds.max.y + 0.5f,
                transform.position.z
            );
        }
        else
        {
            worldPosition =
                transform.position + Vector3.up;
        }


        Vector2 screenPosition =
            mainCamera.WorldToScreenPoint(worldPosition);


        RectTransform canvasRect =
            countdownCanvas.transform as RectTransform;


        Camera uiCamera =
            countdownCanvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : countdownCanvas.worldCamera;


        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                uiCamera,
                out Vector2 localPoint
            )
        )
        {
            countdownRect.anchoredPosition =
                localPoint;
        }
    }


    private void UpdateCountdown()
    {
        if (geriSayimYazisi == null)
        {
            return;
        }


        bool visible =
            !oyunBitti &&
            (hamurIceride || marshmallowIceride);


        geriSayimYazisi.gameObject.SetActive(
            visible
        );


        if (!visible)
        {
            geriSayimYazisi.text = "";
            return;
        }


        string hamurKalan =
            Mathf.Max(
                0f,
                kazanmaSuresi - hamurSure
            )
            .ToString(
                "0.0",
                CultureInfo.InvariantCulture
            );


        string marshmallowKalan =
            Mathf.Max(
                0f,
                kazanmaSuresi - marshmallowSure
            )
            .ToString(
                "0.0",
                CultureInfo.InvariantCulture
            );


        // İKİSİ AYNI ANDA KAPIDA
        if (hamurIceride && marshmallowIceride)
        {
            geriSayimYazisi.text =
                "Hamur: " + hamurKalan +
                "\nMarshmallow: " + marshmallowKalan;
        }

        // SADECE HAMUR
        else if (hamurIceride)
        {
            geriSayimYazisi.text =
                hamurKalan;
        }

        // SADECE MARSHMALLOW
        else if (marshmallowIceride)
        {
            geriSayimYazisi.text =
                marshmallowKalan;
        }
    }


    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (
            other.GetComponentInParent<HamurMovement>()
            == hamur
        )
        {
            hamurIceride = true;
        }


        if (
            other.GetComponentInParent<MarshmallowMovement>()
            == marshmallow
        )
        {
            marshmallowIceride = true;
        }


        UpdateCountdown();
    }


    private void OnTriggerExit2D(
        Collider2D other
    )
    {
        if (
            other.GetComponentInParent<HamurMovement>()
            == hamur
        )
        {
            hamurIceride = false;
            hamurSure = 0f;
        }


        if (
            other.GetComponentInParent<MarshmallowMovement>()
            == marshmallow
        )
        {
            marshmallowIceride = false;
            marshmallowSure = 0f;
        }


        UpdateCountdown();
    }


    private void Bitir(
        MatchResult sonuc
    )
    {
        if (oyunBitti)
        {
            return;
        }


        oyunBitti = true;

        UpdateCountdown();


        // SONUCU KAYDET
        if (sonuc == MatchResult.HamurWins)
        {
            GameResult.HamurWon();
        }
        else if (
            sonuc == MatchResult.MarshmallowWins
        )
        {
            GameResult.MarshmallowWon();
        }
        else if (
            sonuc == MatchResult.Draw
        )
        {
            GameResult.Draw();
        }


        PlayerInput hamurInput =
            hamur.GetComponent<PlayerInput>();

        PlayerInput marshmallowInput =
            marshmallow.GetComponent<PlayerInput>();


        if (hamurInput != null)
        {
            hamurInput.DeactivateInput();
        }


        if (marshmallowInput != null)
        {
            marshmallowInput.DeactivateInput();
        }


        Time.timeScale = 1f;

        SceneManager.LoadScene(
            endSceneName
        );
    }
}