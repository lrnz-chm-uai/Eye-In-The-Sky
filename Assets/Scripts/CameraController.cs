//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector2 mouseLook;
    Vector2 mouseSmoothing;
    [SerializeField] float sensitivityFactor = 5.0f;
    [SerializeField] float mouseSmoothingFactor = 2.0f;
    GameObject player;
    [SerializeField] GameObject orb;
    private bool inputEnabled = false; // Bloqueada hasta que GameManager inicie el juego

    void Start()
    {
        player = this.transform.parent.gameObject;

        transform.LookAt(orb.transform.position);
        // Calculo de valores de mouseLook que correspondan a la orientación del orbe
        Vector3 dir = (orb.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        Vector3 e = lookRot.eulerAngles;

        float yaw = e.y;
        if (yaw > 180f) yaw -= 360f;
        float pitch = e.x;
        if (pitch > 180f) pitch -= 360f;

        mouseLook.x = yaw;
        mouseLook.y = -pitch; // se usa -mouseLook.y al aplicar la rotación en Update
    }

    // Habilita/deshabilita el input de la cámara. Lo llama GameManager.StartGame().
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    void Update()
    {
        if (!inputEnabled) return;

        var mouseMove = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); // mouseMove 
        mouseMove = Vector2.Scale(mouseMove, new Vector2(sensitivityFactor * mouseSmoothingFactor, sensitivityFactor * mouseSmoothingFactor)); // Scale multiplica 2 vectores
        mouseSmoothing.x = Mathf.Lerp(mouseSmoothing.x, mouseMove.x, 1f / mouseSmoothingFactor);
        mouseSmoothing.y = Mathf.Lerp(mouseSmoothing.y, mouseMove.y, 1f / mouseSmoothingFactor);
        mouseLook += mouseSmoothing;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f); // Limita un valor a un rango específico
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);
    }
}
