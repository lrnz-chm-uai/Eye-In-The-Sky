//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 6f;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject bullet;
    [SerializeField] int bulletDamage = 25;
    public int BulletDamage => bulletDamage;
    [SerializeField] int bulletSpeed = 15;
    [SerializeField] int bulletLifetime = 5;
    [SerializeField] int colldown = 2;
    private float lastBulletTime = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        float zMovement = Input.GetAxis("Vertical") * movementSpeed; // direccion por rapidez
        float xMovement = Input.GetAxis("Horizontal") * movementSpeed;
        zMovement *= Time.deltaTime;
        xMovement *= Time.deltaTime;
        transform.Translate(xMovement, 0, zMovement);
        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastBulletTime > colldown || lastBulletTime == 0f)
            {
                Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                GameObject _bullet;
                _bullet = Instantiate(bullet, ray.origin, transform.rotation);
                Debug.Log("Bullet instantiated at: " + ray.origin);
                Rigidbody rb = _bullet.GetComponent<Rigidbody>();
                rb.AddForce(mainCamera.transform.forward * bulletSpeed, ForceMode.Impulse);
                if (_bullet != null)
                {
                    Destroy(_bullet, bulletLifetime);
                }
                lastBulletTime = Time.time;
            }
            else
            {
                Debug.Log("Gun is on cooldown");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collision with enemy");
            Destroy(gameObject);
        }
    }
}
