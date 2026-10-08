//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10.0f;
    [SerializeField] Camera mainCamera;

    //public GameObject bullet;
    //private int rayRange = 5;
    //private int bulletSpeed = 15;
    //private int bulletLifetime = 5;

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

        //if (Input.GetMouseButtonDown(0)) // mouse izq
        //{
        //    Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        //    // instancio prefab de bullet
        //    GameObject pro;
        //    pro = Instantiate(bullet, ray.origin, transform.rotation);
        //    Rigidbody rb = pro.GetComponent<Rigidbody>();
        //    rb.AddForce(mainCamera.transform.forward * bulletSpeed, ForceMode.Impulse);
        //    Destroy(pro, bulletLifetime);

        //    RaycastHit hit;
        //    if ((Physics.Raycast(ray, out hit) == true) && hit.distance < rayRange)
        //    {
        //        Debug.Log("El rayo tocó al objeto: " + hit.collider.name);
        //        //if (hit.collider.name.Substring(0, 3) == "Bot") // me conviene hacerlo por tag?
        //        if (hit.collider.gameObject.CompareTag("Bot"))
        //        {
        //            // obtengo o importo el script del bot
        //            GameObject objetoTocado = GameObject.Find(hit.transform.name);
        //            BotController scriptObjetoTocado = (BotController)objetoTocado.GetComponent(typeof(BotController));
        //            if (scriptObjetoTocado != null)
        //            {
        //                // invoco su funcion para recibir dano
        //                //scriptObjetoTocado.recibirDaño();
        //            }
        //        }
        //    }
        //}
    }

}
