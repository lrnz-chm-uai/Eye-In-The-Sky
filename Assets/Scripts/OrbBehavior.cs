//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class OrbBehavior : MonoBehaviour
{
    [SerializeField] GameObject player;
    private int hp;
    [SerializeField] int hpOnStart = 100;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        hp = hpOnStart;
    }

    void Update()
    {
        transform.LookAt(player.transform.position);
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
            Debug.Log("Bullet hit the orb");
            takeDamage();
            if (collision.gameObject != null)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
