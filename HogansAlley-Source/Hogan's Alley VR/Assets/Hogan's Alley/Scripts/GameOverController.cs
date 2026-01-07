using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;

    void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        
        if(scoreDisplay != null)
        {
            scoreDisplay.text = "Final Score: " + finalScore;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}