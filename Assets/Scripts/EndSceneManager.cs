using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    [Header("Sonuç Görseli")]
    [SerializeField] private Image resultImage;

    [SerializeField] private Sprite winsSprite;
    [SerializeField] private Sprite drawSprite;


    [Header("Kazanan Karakter")]
    [SerializeField] private Image winnerImage;

    [SerializeField] private Sprite hamurWinnerSprite;
    [SerializeField] private Sprite marshmallowWinnerSprite;


    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "1.Level";
    [SerializeField] private string mainMenuSceneName = "StartScene";


    private void Start()
    {
        ShowResult();
    }


    private void ShowResult()
    {
        switch (GameResult.Result)
        {
            // HAMUR KAZANDI
            case MatchResult.HamurWins:

                resultImage.gameObject.SetActive(true);
                resultImage.sprite = winsSprite;

                winnerImage.gameObject.SetActive(true);
                winnerImage.sprite = hamurWinnerSprite;

                break;


            // MARSHMALLOW KAZANDI
            case MatchResult.MarshmallowWins:

                resultImage.gameObject.SetActive(true);
                resultImage.sprite = winsSprite;

                winnerImage.gameObject.SetActive(true);
                winnerImage.sprite = marshmallowWinnerSprite;

                break;


            // BERABERE
            case MatchResult.Draw:

                resultImage.gameObject.SetActive(true);
                resultImage.sprite = drawSprite;

                // Beraberlikte kazanan yok
                winnerImage.gameObject.SetActive(false);

                break;


            default:

                Debug.LogWarning("Oyun sonucu bulunamadı!");

                resultImage.gameObject.SetActive(false);
                winnerImage.gameObject.SetActive(false);

                break;
        }
    }


    // PLAY AGAIN
    public void PlayAgain()
    {
        GameResult.ResetResult();

        SceneManager.LoadScene(gameSceneName);
    }


    // MAIN MENU
    public void MainMenu()
    {
        GameResult.ResetResult();

        SceneManager.LoadScene("StartScene");
    }
}