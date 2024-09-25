using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Rigidbody bossRB;
    public GameObject player;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SmashStart()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(SmashCoroutine());
    }

    private IEnumerator SmashCoroutine()
    {
        // raise player
        bossRB.AddForce(Vector3.up * smashUpForce, ForceMode.Impulse);
        smashing = true;

        // coroutine wait while raising
        yield return new WaitForSeconds(smashUpDuration); 

        // impulse force down/y for player
        bossRB.AddForce(Vector3.down * smashDownForce, ForceMode.Impulse);
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
