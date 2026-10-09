using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    [SerializeField] float speed = 5f;
    private int hp;
    [SerializeField] int hpOnStart = 25;
    [SerializeField] private PlayerController playerController;

    // Evento que se dispara cuando el enemigo es destruido
    public static event Action<EnemyController> OnEnemyDestroyed;

    void Start()
    {
        hp = hpOnStart;
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Ensure we have the PlayerController reference at runtime
            playerController = player.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        transform.LookAt(player.transform.position);
        transform.Translate(speed * Vector3.forward * Time.deltaTime);
    }

    private void takeDamage()
    {
        //int damage = playerController != null ? playerController.BulletDamage : 25;
        int damage = playerController.BulletDamage;
        hp -= damage;
        if (hp <= 0)
        {
            // Aviso a posibles suscriptores al evento OnEnemyDestroyed
            OnEnemyDestroyed?.Invoke(this);
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
