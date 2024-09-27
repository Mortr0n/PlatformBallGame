using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody enemyRB;
    private GameObject player;
    private int enemyScore = 0;
    public GameObject playerObject;
    public AudioSource enemyAudio;
    public AudioClip enemyDie;

    // Start is called before the first frame update
    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        player = GameObject.Find("PlayerSphere");
        
    }

    // Update is called once per frame
    void Update()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null) {
            Vector3 moveDirection = (player.transform.position - transform.position).normalized;
            enemyRB.AddForce(moveDirection * speed * Time.deltaTime);
             
        }
        if (transform.position.y <= -10)
        {
            enemyAudio.PlayOneShot(enemyDie, 1f);
            enemyScore = GameManager.Instance.GetScore(); // set up here to change score based on enemy type maybe!
            Destroy(gameObject);
        }
    }

    public void AddScore(int score)
    {
        GameManager.Instance.AddScore(score);
        enemyScore = GameManager.Instance.GetScore();
        //enemyScore += score;
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            PlayerController playerObject = player.GetComponent<PlayerController>();
            playerObject.AddScore(enemyScore);
        }
    }
}
