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
        // El player puede haber sido destruido (derrota) mientras el orb sigue vivo
        if (player == null) return;
        transform.LookAt(player.transform.position);
    }

    private void TakeDamage()
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
            TakeDamage();
            if (collision.gameObject != null)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
