using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody enemyRB;
    public GameObject playerObject;
    public GameObject gameManager;

    public float speed = 3f;
    private GameObject player;
    private int enemyScore = 1;

    public AudioSource enemyAudio;
    public AudioClip enemyDie;

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        player = GameObject.Find("PlayerSphere");
        gameManager = GameObject.Find("GameManager");
    }

    void Update()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null) {
            Vector3 moveDirection = (player.transform.position - transform.position).normalized;
            enemyRB.AddForce(moveDirection * speed * Time.deltaTime);
        }
        if (transform.position.y <= -10)
        {  
            enemyScore = GameManager.Instance.GetScore();  
            GameManager.Instance.PlayEnemyDeath(); 
            Destroy(gameObject);
        }
    }

    IEnumerator DeathWait()
    {
        yield return new WaitForSeconds(1.1f);
    }

    private void OnDestroy()
    {
        GameManager.Instance.AddScore(1);   
    }
}
