using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bidirectional portal built on a primitive cube.
/// Teleports the player to a randomly chosen other portal.
/// Enemies pass through unaffected (only the "Player" tag is handled).
/// </summary>
[DisallowMultipleComponent]
public class Portal : MonoBehaviour
{
    // Cuando OnEnable se llama, se carga una lista con los portales activos
    private static readonly List<Portal> instances = new List<Portal>();

    [Header("Teleport Settings")]
    [Tooltip("Seconds the player must wait before teleporting again (prevents ping-pong between portals).")]
    [SerializeField] private float cooldownSeconds = 0.5f;

    [Tooltip("Local offset from the destination portal where the player appears. E.g. (0, 0, 1) spawns in front of the portal.")]
    [SerializeField] private Vector3 exitOffset = new Vector3(0f, 1f, 2f);

    [Tooltip("Height above the portal's pivot where the player appears. Use it to match the player's spawn height.")]
    [SerializeField] private float exitHeight = 1f;

    [Tooltip("If false, this portal won't teleport anyone (can be toggled at runtime).")]
    [SerializeField] private bool isActive = true;
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    public float CooldownSeconds
    {
        get => cooldownSeconds;
        set => cooldownSeconds = Mathf.Max(0f, value);
    }

    void OnEnable()
    {
        if (!instances.Contains(this))
        {
            instances.Add(this);
        }
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    void OnTriggerEnter(Collider other)
    {
        // Si el portal no está activo o el collider es un enemigo, no hacer nada.
        if (!isActive || !other.CompareTag("Player")) return;

        // Obtengo el componenete PlayerTeleportState del jugador. Si no existe, lo creo.
        PlayerTeleportState state = other.GetComponent<PlayerTeleportState>();
        if (state == null)
        {
            state = other.gameObject.AddComponent<PlayerTeleportState>();
        }

        // Aprovecho el método público CanTeleport de PlayerTeleportState para verificar si el jugador puede teletransportarse según el cooldown
        if (!state.CanTeleport(cooldownSeconds))
        {
            if (showDebugLogs)
            {
                Debug.Log($"{nameof(Portal)} '{name}': player on cooldown ({state.RemainingCooldown(cooldownSeconds):F2}s left).");
            }
            return;
        }

        Portal destination = PickRandomOtherPortal();
        if (destination == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"{nameof(Portal)} '{name}': no other active portal available as destination.");
            }
            return;
        }

        destination.TeleportPlayer(state);

        if (showDebugLogs)
        {
            Debug.Log($"{nameof(Portal)} '{name}': teleported player to '{destination.name}'.");
        }
    }

    private Portal PickRandomOtherPortal()
    {
        // Uso una lista con los demás portales
        List<Portal> candidates = new List<Portal>();
        for (int i = 0; i < instances.Count; i++)
        {
            Portal p = instances[i];
            if (p != null && p != this && p.isActive)
            {
                candidates.Add(p);
            }
        }

        if (candidates.Count == 0) return null;
        // Devuelvo uno al azar
        return candidates[Random.Range(0, candidates.Count)];
    }

    private void TeleportPlayer(PlayerTeleportState state)
    {
        // Punto de salida en world-space: posición del portal + offset local.
        Vector3 destinationPosition = transform.TransformPoint(exitOffset);
        // exitHeight corrige la altura final (cuanto más positivo, más arriba del pivot del portal).
        destinationPosition.y = transform.position.y + exitHeight;

        // Rotación de salida: solo el yaw del portal, para que el jugador salga derecho
        // aunque el cubo/portal esté rotado en X/Z.
        Vector3 portalForward = transform.forward;
        portalForward.y = 0f;
        Quaternion destinationRotation = portalForward.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(portalForward.normalized, Vector3.up)
            : Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        Transform playerTransform = state.transform;
        Rigidbody rb = state.Rigidbody;
        CharacterController cc = state.CharacterController;

        if (rb != null)
        {
            rb.position = destinationPosition;
            rb.rotation = destinationRotation;
            playerTransform.position = destinationPosition;
            playerTransform.rotation = destinationRotation;
        }
        else if (cc != null)
        {
            cc.enabled = false;
            playerTransform.SetPositionAndRotation(destinationPosition, destinationRotation);
            cc.enabled = true;
        }
        else
        {
            playerTransform.SetPositionAndRotation(destinationPosition, destinationRotation);
        }

        // Marca el tiempo del teleport y limpia la velocidad del Rigidbody (caída, empujones).
        state.SetTeleportedNow();
    }

    void OnDrawGizmosSelected()
    {
        // Dibujo el punto de salida para que el offset sea fácil de ajustar en el editor
        Gizmos.color = Color.cyan;
        Vector3 exit = transform.TransformPoint(exitOffset);
        exit.y = transform.position.y + exitHeight;
        Gizmos.DrawWireSphere(exit, 0.25f);
        Gizmos.DrawLine(transform.position, exit);

        // Draw lines to the other portals to visualize possible destinations.
        //Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        //for (int i = 0; i < instances.Count; i++)
        //{
        //    if (instances[i] != null && instances[i] != this)
        //    {
        //        Gizmos.DrawLine(transform.position, instances[i].transform.position);
        //    }
        //}
    }
}
