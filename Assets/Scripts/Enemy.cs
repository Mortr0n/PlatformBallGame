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
            Vector3 moveDirection = (player.transform.position - transform.position);
            enemyRB.AddForce(moveDirection.normalized * speed * Time.deltaTime);
             
        }
        if (transform.position.y <= -10)
        {
            enemyScore = 1; // set up here to change score based on enemy type maybe!
            Destroy(gameObject);
        }
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
