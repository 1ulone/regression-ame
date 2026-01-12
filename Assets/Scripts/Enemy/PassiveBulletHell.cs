using UnityEngine;
using System.Collections;

public class PassiveBulletHell : MonoBehaviour
{
    private EnemyBoss boss;
    private Coroutine currentCoroutine; 

    private void Start()
    {
        boss = GetComponent<EnemyBoss>();
    }

    private void Update()
    {
        if (boss.isSecondPhase)
        {
            if (currentCoroutine == null)
                currentCoroutine = StartCoroutine(BulletHell());
        }
    }

    private IEnumerator BulletHell()
    {
        float bulletPerRound = 18f;
        string attackPrefab = "enemyBullet";
        float angleStep = 20;
        float startAngle = 0;

        for (int i = 0; i < bulletPerRound; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector3 currentDirection = Quaternion.Euler(0, 0, currentAngle) * Vector2.down;

            DamageComponent b = Pool.instances.CreateObject(attackPrefab, transform.position + (currentDirection*2), currentDirection).GetComponent<DamageComponent>();
            // b.GetComponent<DestroyOnExitView>().enabled = false;

            b.gameObject.GetComponent<Rigidbody2D>().linearVelocity = currentDirection * 1 * 3; 
            b.damage = 1;
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1f);
        currentCoroutine = null;
    }
}
