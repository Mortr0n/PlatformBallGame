using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private GameObject enemies;
    private Rigidbody bulletRB;
    private Transform target;

    public float speed = 200;
    private float timeToDestroy = 2.5f;

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
        bulletRB = GetComponent<Rigidbody>();
        StartCoroutine(DestroyBulletRoutine(timeToDestroy));
    }

    void Update()
    {

        if (target != null)
        {
            Vector3 moveDirection = (target.position - transform.position); 
            bulletRB.AddForce(moveDirection.normalized * speed * Time.deltaTime);
        }
        if (transform.position.x >= 40 || transform.position.x <= -40 || transform.position.z >= 40 || transform.position.z <= -40)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyBulletRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
