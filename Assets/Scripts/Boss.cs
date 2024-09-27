using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Rigidbody bossRB;
    public GameObject player;
    public AudioSource bossAudio;
    public AudioSource crash;

    public GameObject enemy;
    public Enemy enemyController;
    public AudioClip bossCrash;

    public float centerSafeZone = 8f;
    public float platformRadius = 12f;
    private Vector3 platformCenter = Vector3.zero;
    public float speed = 800f;

    public float smashStart = 10f;
    private float smashUpForce = 50f;
    private float smashUpDuration = 1f;
    private float shockwaveRadius = 10f;
    private float shockwaveForce = 60f;
    private float smashDownForce = 50f;
    private bool smashing = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SmashStart());   
        bossRB = GetComponent<Rigidbody>();
        player = GameObject.Find("PlayerSphere");
        enemy = GameObject.Find("Enemy");
        bossAudio = GetComponent<AudioSource>();
        bossAudio.Play();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StopBackgroundMusic();  // Stop background music
        }

        if (bossAudio != null)
        {
            bossAudio.Play();  // Play boss music
        }
    }

    // Update is called once per frame 
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 moveDirection = (player.transform.position - transform.position).normalized;
            float distanceFromCenter = Vector3.Distance(transform.position, platformCenter);
            
            
            if (IsPlayerBetweenBossAndEdge())
            {
                Debug.Log("attacking player");
                bossRB.AddForce(moveDirection * speed * Time.deltaTime);
            }
            if (distanceFromCenter > centerSafeZone)
            {
                Debug.Log("saving self");
                bossRB.AddForce((Vector3.zero - transform.position) * speed * 1.8f * Time.deltaTime);
            }
            else // if (distanceFromCenter < centerSafeZone)
            {
                Debug.Log("chasing player");
                bossRB.AddForce(moveDirection * speed * Time.deltaTime);
            }
            
             
        }
        if (transform.position.y <= -10)
        {
            //GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            //Enemy enemyController = enemy.GetComponent<Enemy>();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(5);
                GameManager.Instance.PlayBackgroundMusic();
            }
            if (bossAudio != null)
            {
                bossAudio.Stop();  // Stop the boss music
            }
            Destroy(gameObject);
        }
    }

    //check if player is between boss and edge
    private bool IsPlayerBetweenBossAndEdge()
    {
        Vector3 toCenter = platformCenter - transform.position;
        Vector3 toPlayer = player.transform.position - transform.position;

        // Angle between boss to center and boss to player
        float angleToPlayer = Vector3.Angle(toCenter, toPlayer);

        // Only return true if player is roughly between the boss and the center
        return angleToPlayer < 90f; // You can adjust the threshold
    }

    private IEnumerator SmashStart()
    {
        yield return new WaitForSeconds(smashStart);
        StartCoroutine(SmashCoroutine());
    }

    private IEnumerator SmashCoroutine()
    {
        // raise player
        bossRB.velocity = Vector3.zero;
        bossRB.AddForce(Vector3.up * smashUpForce, ForceMode.Impulse);
        smashing = true;

        // coroutine wait while raising
        yield return new WaitForSeconds(smashUpDuration); 

        // impulse force down/y for player
        bossRB.AddForce(Vector3.down * smashDownForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        crash.PlayOneShot(bossCrash, 1f);
        if (smashing && collision.gameObject.CompareTag("Ground"))
        {
            // if that does not apply outward force then we need to add an outward force
            TriggerShockWave();
        }
    }

    private void TriggerShockWave()
    {
        // Get all nearby rigidbodies in shockwave radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockwaveRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            Rigidbody playerRB = hitCollider.GetComponent<Rigidbody>();

            if (playerRB != null && hitCollider.CompareTag("Player"))
            {
                // direction from the player to the enemy
                Vector3 direction = hitCollider.transform.position - transform.position;

                // push the enemy rigidbodies out
                playerRB.AddForce(direction.normalized * shockwaveForce, ForceMode.Impulse);
            }
        }
        smashing = false;
    }
}
