using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRB;
    private GameObject focalPoint;
    public GameObject bouncyPUIndicator;
    public GameObject bulletPowerUpIndicator;
    public GameObject bouncePowerUp;
    public GameObject bulletPowerUp;
    public GameObject smashPowerUp;
    public GameObject playerPrefab;
    public GameObject bullet;
    private GameObject player;
    public GameManager gameManager;

    public AudioSource playerAudio;
    public AudioSource oneShotAudio;

    public AudioClip enemyCrash;
    public AudioClip bossCrash;
    public AudioClip superSmash;
    public AudioClip collectPowerUp;
    public AudioClip playerDeathSound;


    private float speed = 450;
    //public bool hasPowerUp = false;

    public string powerUp;
    private Vector3 puOffset;
    private float pUpBounceForce =  25f;

    private float smashUpForce = 10f;
    private float smashUpDuration = 1f;
    private float shockwaveRadius = 10f;
    private float shockwaveForce = 30f;
    private float smashDownForce = 15f;
    private bool smashing = false;

    private float powerUpTime = 7f;
    private float powerUpSpawnRange = 9;
    private float pUpCountdownTime = 8;
    private int score = 0;
    private int lives = 3;
    private float spawnRange = 8;
    public bool hasFallen = false;
    
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
       
        //bouncyPUIndicator = GameObject.Find("BouncePowerUp");
        puOffset = new Vector3(0, -.4f, 0);
        StartCoroutine(SpawnPowerUpCountdownRoutine(powerUpTime));
    }
     
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");

        //Debug.Log("fIn: " + forwardInput + " speed " + speed + " fpt: " + focalPoint.transform.forward);
        playerRB.AddForce(forwardInput * speed * focalPoint.transform.forward * Time.deltaTime);
        bouncyPUIndicator.transform.position = transform.position + puOffset;
        bulletPowerUpIndicator.transform.position = transform.position + puOffset;

        // if player falls below -5y call kill func
        if (transform.position.y <-5)
        {

            oneShotAudio.PlayOneShot(playerDeathSound, 1.5f);
            killPlayer(hasFallen);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivatePowerUp();
        }

        // if we have the bullet power up and space is pressed fire off our bullets
        //if (Input.GetKeyDown(KeyCode.Space) && powerUp == "BulletPowerUp")
        //{          
        //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 

            //    // currently not limiting the bullets.  This is basically a clear the map power up potentially limit the rarity on this one
            //    StartCoroutine(bulletWait(enemies));
            //}

    }

    void ActivatePowerUp()
    {
        switch (powerUp)
        {
            case "BulletPowerUp":
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                // currently not limiting the bullets.  This is basically a clear the map power up potentially limit the rarity on this one
                StartCoroutine(bulletWait(enemies));
                break;
            case "SmashPowerUp":
                StartCoroutine(SmashCoroutine());
                break;
        }
    }

    private IEnumerator SmashCoroutine()
    {
        // raise player
        playerRB.AddForce(Vector3.up * smashUpForce, ForceMode.Impulse);
        smashing = true;

        // coroutine wait while raising
        yield return new WaitForSeconds(smashUpDuration);

        // impulse force down/y for player
        playerRB.AddForce(Vector3.down * smashDownForce, ForceMode .Impulse);
    }

    private void TriggerShockWave()
    {
        // Get all nearby rigidbodies in shockwave radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockwaveRadius);
        playerAudio.PlayOneShot(superSmash, 1f);
        foreach (Collider hitCollider in hitColliders)
        {
            Rigidbody enemyRigidbody = hitCollider.GetComponent<Rigidbody>();

            if (enemyRigidbody != null && hitCollider.CompareTag("Enemy"))
            {
                // direction from the player to the enemy
                Vector3 direction = hitCollider.transform.position - transform.position;

                // push the enemy rigidbodies out
                enemyRigidbody.AddForce(direction.normalized * shockwaveForce, ForceMode.Impulse);
            }
        }
        smashing = false;
    }

    IEnumerator bulletWait(GameObject[] enemies) 
    {
        for (int i = 0; i < enemies.Length * 4; i++)
        {
            yield return new WaitForSeconds(.3f);
            SpawnBullet(enemies);
        }
    }

    


    private Vector3 GenerateSpawnPos()
    {
        
        float spawnPosX = UnityEngine.Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }
    public void AddScore(int points)
    {
        score += points;
        Debug.Log(score);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BouncePowerUp") || other.CompareTag("SmashPowerUp") || (other.CompareTag("BulletPowerUp")))
        {
            playerAudio.PlayOneShot(collectPowerUp, 1f);
            powerUp = other.tag;
            bouncyPUIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
            StartCoroutine(PowerUpCountdownRoutine(powerUpTime));
        }
        
        //if (other.CompareTag("BulletPowerUp"))
        //{
        //    powerUp = other.tag;
        //    bulletPowerUpIndicator.gameObject.SetActive(true);
        //    Destroy(other.gameObject);
        //    StartCoroutine(PowerUpCountdownRoutine(powerUpTime));
        //}
    }

     IEnumerator PowerUpCountdownRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        bouncyPUIndicator.gameObject.SetActive(false);
        bulletPowerUpIndicator.gameObject.SetActive(false);
        powerUp = null;
        StartCoroutine(SpawnPowerUpCountdownRoutine(pUpCountdownTime));
    } 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerAudio.PlayOneShot(enemyCrash, 1f);
        }
        
        if (collision.gameObject.CompareTag("Enemy") && powerUp == "BouncePowerUp" )
        {
            //Debug.Log("Collided with " + collision.gameObject.name + " with powerup set to " + powerUp);

            Rigidbody enemyRB = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - playerRB.transform.position).normalized;
            
            enemyRB.AddForce(awayFromPlayer * pUpBounceForce, ForceMode.Impulse);            
        }
        if (smashing && collision.gameObject.CompareTag("Ground"))
        {
            // if that does not apply outward force then we need to add an outward force
            TriggerShockWave();
        }
    }

    IEnumerator SpawnPowerUpCountdownRoutine(float time)
    {
        yield return new WaitForSeconds (time);
        // now have a handler since there's more than one power up
        HandlePowerUpSpawn();
        
        StartCoroutine(SpawnPowerUpCountdownRoutine(pUpCountdownTime));
    }

    public void HandlePowerUpSpawn()
    {
        //adding all the power ups to the existing power ups list
        List<GameObject> existingPowerUps = new List<GameObject>();

        GameObject[] BCEPups = GameObject.FindGameObjectsWithTag("BouncePowerUp");
        existingPowerUps.AddRange(BCEPups);

        GameObject[] BulletPUps = GameObject.FindGameObjectsWithTag("BulletPowerUp");
        existingPowerUps.AddRange(BulletPUps);

        GameObject[] SmashPUps = GameObject.FindGameObjectsWithTag("SmashPowerUp");
        existingPowerUps.AddRange(SmashPUps);

        // destroying all but one power up if there's extras
        if (existingPowerUps.Count > 1)
        {
            for (int i = 1; i < existingPowerUps.Count; i++)
            {
                Destroy(existingPowerUps[i]);
            }
        }

        // if there's no power ups and we don't have one on the player then we can start the spawn process
        if (powerUp == null && existingPowerUps.Count == 0)
        {
            // pick random power up to spawn.  to change spawn percent you could add a bigger range and then make the case allow for extra num's for that power up.
            // so you could do Random.Range(0,5) and then case 0,1,2,3  and then a case 4 for a 80 percent 20 percent chance
            int rNum = UnityEngine.Random.Range(0, 5);
            switch (rNum)
            {
                case 0:
                    SpawnPowerUpAtRandLoc("Bounce");
                    break;
                case 1:
                    SpawnPowerUpAtRandLoc("Bullet");
                    break;
                case 2:
                case 3:
                case 4:
                    SpawnPowerUpAtRandLoc("Smash");
                    break;
            }
            
        }
    }



    void SpawnBullet(GameObject[] enemies)
    {
        // chose to send bullets at enemies at random.  It's not as random as I intended and actually just spreads the bullets evenly between them all
        GameObject randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Length)];
        if (randomEnemy != null) {
            Vector3 directionToEnemy = (randomEnemy.transform.position - playerRB.transform.position).normalized;

            GameObject bulletObject = Instantiate(bullet, playerRB.transform.position + directionToEnemy * 1.5f, Quaternion.identity);
            Debug.Log("Spawning bullet at: " + playerRB.transform.position + "Obj: " + bulletObject);

            bulletObject.GetComponent<BulletScript>().SetTarget(randomEnemy.transform);
        }
        
    }

    void SpawnPowerUpAtRandLoc(string type)
    {
        // simple switch for choosing which power up to spawn at random loc
        switch (type)
        {
            case "Bounce":
                Instantiate(bouncePowerUp, GenerateRandomSpawnPos(), bouncePowerUp.transform.rotation);
                break;
            case "Bullet":
                Instantiate(bulletPowerUp, GenerateRandomSpawnPos(), bulletPowerUp.transform.rotation);
                break;
            case "Smash":
                Instantiate(smashPowerUp, GenerateRandomSpawnPos(), smashPowerUp.transform.rotation);
                break;
        }
        
    }

    private Vector3 GenerateRandomSpawnPos()
    {
        //pick a random location
        float spawnPosX = UnityEngine.Random.Range(-powerUpSpawnRange, powerUpSpawnRange);
        float spawnPosZ = UnityEngine.Random.Range(-powerUpSpawnRange, powerUpSpawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    void killPlayer(bool fell)
    {
        //Debug.Log("Kill Player: " + fell);
        if (hasFallen) return;

        if (transform.position.y < -5 && !hasFallen)
        {
            hasFallen = true;
            lives--;
            //Debug.Log("Lives: " + lives + " hasFallen: " + hasFallen + " gO Name: " + gameObject.name + " : " + gameObject);
            if (lives >= 0)
            {
                GameObject newPlayer = Instantiate(playerPrefab, GenerateSpawnPos(), transform.rotation);
                PlayerController pController = newPlayer.GetComponent<PlayerController>();

                // setting the necessary settings on the new player object
                pController.hasFallen = false;
                pController.lives = lives;
                pController.powerUp = null;

                // making sure the power ups are not on when new player obj spawns
                bouncyPUIndicator.gameObject.SetActive(false);
                bulletPowerUpIndicator.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Game Over");
            }
            if (hasFallen)
            {
                //gameManager.PlayGameOverMusic();
                Destroy(this.gameObject);
                player = null;
            }
        }
    }

    // not in use anymore after allowing for doing both on one func
    //public void HandleBulletPowerUpSpawn()
    //{
    //    List<GameObject> existingPowerUps = new List<GameObject>();
    //    GameObject[] BouncePUps = GameObject.FindGameObjectsWithTag("BouncePowerUp");
    //    existingPowerUps.AddRange(BouncePUps);
    //    GameObject[] BulletPUps = GameObject.FindGameObjectsWithTag("BulletPowerUp");
    //    existingPowerUps.AddRange(BulletPUps);

    //    if (existingPowerUps.Count > 1)
    //    {
    //        for (int i = 1; i < existingPowerUps.Count; i++)
    //        {
    //            Destroy(existingPowerUps[i]);
    //        }
    //    }
    //    if (powerUp == null && existingPowerUps.Count == 0)
    //    {
    //        SpawnPowerUpAtRandLoc("Bullet");
    //    }
    //}

    // not in use after making spawn bullet do everything
    //public void FireBullet()
    //{
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    if (enemies.Length == 0)
    //    {
    //        Debug.Log("No enemies to target.");
    //        return;
    //    }

    //    GameObject randomEnemy = enemies[Random.Range(0, enemies.Length)];
    //    GameObject bulletObject = Instantiate(bullet, playerRB.transform.position, Quaternion.identity);
    //    if (bulletObject == null)
    //    {
    //        Debug.Log("Failed to instantiate bullet prefab.");
    //        return;
    //    }

    //    BulletScript bulletScript = bulletObject.GetComponent<BulletScript>();
    //    if (bulletScript == null)
    //    {
    //        Debug.LogError("BulletScript not found on bullet prefab.");
    //        Destroy(bulletObject);  // Clean up if the necessary component is missing
    //    }
    //    else
    //    {
    //        bulletScript.SetTarget(randomEnemy.transform);
    //        Debug.Log("Bullet spawned and target set: " + randomEnemy.name);
    //    }
    //}

}
