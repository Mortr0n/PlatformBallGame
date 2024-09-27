using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance
    public Text scoreText;
    private int score = 0;
    public AudioSource backgroundMusic;
    //public AudioSource gameOverMusic;
    public AudioSource gameOverMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this GameObject from being destroyed
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance
        }

    }



    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Total Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
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
        if (!backgroundMusic.isPlaying)
        {
            gameOverMusic.Play();
        }
    }

    
}
