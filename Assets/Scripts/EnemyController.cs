using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float speed = 5f;
    private int hp;
    [SerializeField] int hpOnStart = 25;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        hp = hpOnStart;
    }

    void Update()
    {
        transform.LookAt(player.transform.position);
        transform.Translate(speed * Vector3.forward * Time.deltaTime);
    }

    private void takeDamage()
    {
        hp -= playerController.BulletDamage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Bullet hit one enemy");
            takeDamage();
            if (collision.gameObject != null)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
