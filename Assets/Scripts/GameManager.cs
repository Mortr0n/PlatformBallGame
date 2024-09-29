using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance
    
    public Text scoreText;
    public Text spaceText;
    public Button restartButton;
    public Text livesText;

    private int playerLives = 3;
    private int score = 0;

    public AudioSource backgroundMusic;
    public AudioSource gameOverMusic;
    public AudioClip playerDeath;
    public AudioSource enemyAudio;
    public AudioClip enemyDeath;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent GameObject from being destroyed
        }
        else
        {
            Destroy(gameObject); // single instance only
        }

    }

    void Start()
    {
        enemyAudio = GetComponent<AudioSource>();
        // set text items
        //scoreText.text = "Score: " + score.ToString();
        //spaceText.text = "";
        //livesText.text = "Lives: " + playerLives.ToString();

        // using funcs to check text obj's and set values 
        UpdateScoreText();
        UpdateSpaceText("");
        UpdateLivesText();
        
        // hide restart button at start of game
        restartButton.gameObject.SetActive(false);
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find the UI elements in the newly loaded scene
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        livesText = GameObject.Find("LivesText").GetComponent<Text>();
        spaceText = GameObject.Find("SpaceText").GetComponent<Text>();

        // reset vals for new game
        score = 0;
        restartButton.gameObject.SetActive(false);
        playerLives = 3;
        spaceText.text = "";


        // Re-initialize UI values after reloading the scene
        UpdateScoreText();
        UpdateLivesText();
        UpdateSpaceText("");
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + playerLives.ToString();
        }
    }

    public void UpdateSpaceText(string message)
    {
        if (spaceText != null)
        {
            spaceText.text = message;
        }
    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }    

    public void PlayerLostLife()
    {
        playerLives--;
        //livesText.text = "Lives: " + playerLives.ToString();
        UpdateLivesText();
        UpdateSpaceText("");

        if (playerLives <= 0)
        {
            livesText.text = "Game Over";
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        restartButton.gameObject.SetActive(true);  
    }

    // Method to restart the game
    public void RestartGame()
    {
        // Reload the current scene  
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
    }

    public void AddScore(int points)
    {
        score += points;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
        Debug.Log("Total Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();  // Stop background music
        }
    }

    public void PlayBackgroundMusic()
    {
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();  // Resume background music
        }
    }

    public void PlayGameOverMusic()
    {
        
            Debug.Log("player died");
            gameOverMusic.PlayOneShot(playerDeath);
        
    }
    public void PlayEnemyDeath()
    {

        enemyAudio.PlayOneShot(enemyDeath);
    }

}
