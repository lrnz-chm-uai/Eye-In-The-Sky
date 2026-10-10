using System;
using UnityEngine;

public class OrbBehavior : MonoBehaviour
{
    [SerializeField] GameObject player;
    private int hp;
    [SerializeField] int hpOnStart = 100;
    [SerializeField] private PlayerController playerController;

    // Se dispara cada vez que el orb recibe daño. UIManager lo usa para mostrar/actualizar la barra de vida.
    // Parámetros: hp actual (clampeado a >= 0) y hp máximo.
    //public static event Action<int, int> OnOrbDamaged;

    void Start()
    {
        hp = hpOnStart;
    }

    void Update()
    {
        // El player puede haber sido destruido (derrota) mientras el orb sigue vivo
        if (player == null) return;
        transform.LookAt(player.transform.position);
    }

    private void TakeDamage()
    {
        hp = Mathf.Max(hp - playerController.BulletDamage, 0);

        // Notificar antes del Destroy: el último evento llega con hp = 0
        //OnOrbDamaged?.Invoke(hp, hpOnStart);

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
            TakeDamage();
            if (collision.gameObject != null)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
