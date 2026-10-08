using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector2 mouseLook;
    Vector2 mouseSmoothing;
    [SerializeField] float sensitivityFactor = 5.0f;
    [SerializeField] float mouseSmoothingFactor = 2.0f;
    GameObject player;
    void Start()
    {
        player = this.transform.parent.gameObject; // obtenemos el parent de la cámara, es decir, el player o Player
    }
    void Update()
    {
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); // md 
        md = Vector2.Scale(md, new Vector2(sensitivityFactor * mouseSmoothingFactor, sensitivityFactor * mouseSmoothingFactor)); // Scale multiplica 2 vectores
        mouseSmoothing.x = Mathf.Lerp(mouseSmoothing.x, md.x, 1f / mouseSmoothingFactor);
        mouseSmoothing.y = Mathf.Lerp(mouseSmoothing.y, md.y, 1f / mouseSmoothingFactor);
        mouseLook += mouseSmoothing;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f); // Limita un valor a un rango específico
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);
    }
}
