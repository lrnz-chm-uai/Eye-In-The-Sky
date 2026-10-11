using System;
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
    [SerializeField] float colldown = 2;
    private float lastBulletTime = 0f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool inputEnabled = false;

    // Evento que se dispara cuando el player es destruido (condición de derrota)
    public static event Action OnPlayerDestroyed;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // El bloqueo del cursor lo maneja UIManager
        //Cursor.lockState = CursorLockMode.Locked; 
    }

    // Habilita/deshabilita el input (movimiento y disparo). Lo llama GameManager.StartGame().
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (!enabled)
        {
            moveInput = Vector2.zero;
        }
    }

    void Update()
    {
        if (!inputEnabled) return;

        // Para movimiento usando el transform
        // Esto puede significa problemas cuando uso un rb no kinematic
        // Mejor manejas en FixedUpdate para las físicas
        //float zMovement = Input.GetAxis("Vertical") * movementSpeed; // direccion por rapidez
        //float xMovement = Input.GetAxis("Horizontal") * movementSpeed;
        //zMovement *= Time.deltaTime;
        //xMovement *= Time.deltaTime;
        //transform.Translate(xMovement, 0, zMovement);

        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

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

    private void FixedUpdate()
    {
        if (rb == null) return;
        if (!inputEnabled) return;

        Vector3 move = (transform.forward * moveInput.y + transform.right * moveInput.x)
                       * movementSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collision with enemy");
            // Aviso a suscriptores (GameManager) antes de destruir
            OnPlayerDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
