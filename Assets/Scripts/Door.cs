using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private HamurMovement hamur;
    [SerializeField] private MarshmallowMovement marshmallow;
    [SerializeField] private float kazanmaSuresi = 2f;

    [Header("Scene")]
    [SerializeField] private string endSceneName = "EndScene";

    private bool hamurIceride;
    private bool marshmallowIceride;
    private bool oyunBitti;

    private float hamurSure;
    private float marshmallowSure;


    private void Start()
    {
        Time.timeScale = 1f;
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<HamurMovement>() == hamur)
        {
            hamurIceride = true;
        }

        if (other.GetComponentInParent<MarshmallowMovement>() == marshmallow)
        {
            marshmallowIceride = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<HamurMovement>() == hamur)
        {
            hamurIceride = false;
            hamurSure = 0f;
        }

        if (other.GetComponentInParent<MarshmallowMovement>() == marshmallow)
        {
            marshmallowIceride = false;
            marshmallowSure = 0f;
        }
    }


    private void Bitir(MatchResult sonuc)
    {
        if (oyunBitti)
        {
            return;
        }

        oyunBitti = true;


        // SONUCU KAYDET
        if (sonuc == MatchResult.HamurWins)
        {
            GameResult.HamurWon();
        }
        else if (sonuc == MatchResult.MarshmallowWins)
        {
            GameResult.MarshmallowWon();
        }
        else if (sonuc == MatchResult.Draw)
        {
            GameResult.Draw();
        }


        // Oyuncuları durdur
        PlayerInput hamurInput = hamur.GetComponent<PlayerInput>();
        PlayerInput marshmallowInput = marshmallow.GetComponent<PlayerInput>();

        if (hamurInput != null)
        {
            hamurInput.DeactivateInput();
        }

        if (marshmallowInput != null)
        {
            marshmallowInput.DeactivateInput();
        }


        // ÖNEMLİ:
        // Yeni sahneye geçmeden önce zamanı tekrar aç
        Time.timeScale = 1f;


        // END SCENE'E GEÇ
        SceneManager.LoadScene(endSceneName);
    }
}