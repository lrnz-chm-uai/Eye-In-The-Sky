using UnityEngine;

/// <summary>
/// Tracks the player's teleport cooldown and caches physics components
/// so Portal can teleport safely with or without a Rigidbody.
/// </summary>
[DisallowMultipleComponent]
public class PlayerTeleportState : MonoBehaviour
{
    // Expongo la siguiente configuración en el editor
    [Header("Teleport Settings")]
    [Tooltip("If true, Rigidbody velocity is zeroed after a teleport to avoid carry-over motion.")]
    [SerializeField] private bool clearVelocityOnTeleport = true;

    private Rigidbody cachedRigidbody;
    private CharacterController cachedCharacterController;
    private float lastTeleportTime = float.NegativeInfinity;

    public Rigidbody Rigidbody => cachedRigidbody;
    public CharacterController CharacterController => cachedCharacterController;
    public float LastTeleportTime => lastTeleportTime;

    public bool ClearVelocityOnTeleport 
    {
        // La expongo así, con getter y setter para que otros scripts la modifiquen si es necesario
        get => clearVelocityOnTeleport;
        set => clearVelocityOnTeleport = value;
    }

    void Awake()
    {
        cachedRigidbody = GetComponent<Rigidbody>();
        cachedCharacterController = GetComponent<CharacterController>();

        // Si cachedRigidbody o cachedCharacterController son nulos, Portal.cs maneja el movimiento del jugador directamente usando el Transform
        if (cachedRigidbody == null && cachedCharacterController == null)
        {
            Debug.LogWarning($"{nameof(PlayerTeleportState)} on '{name}': no Rigidbody or CharacterController found. Teleport will move the Transform directly.");
        }
    }

    // Expongo un método que devuelvo true si el jugador puede teletransportarse
    public bool CanTeleport(float cooldownSeconds)
    {
        return Time.time - lastTeleportTime >= cooldownSeconds;
    }

    /// <summary>
    /// Seconds left before the player can teleport again (0 if ready).
    /// </summary>
    public float RemainingCooldown(float cooldownSeconds)
    {
        float remaining = cooldownSeconds - (Time.time - lastTeleportTime);
        return remaining > 0f ? remaining : 0f;
    }

    /// <summary>
    /// Marks the player as just teleported and clears physics velocity if configured.
    /// </summary>
    public void SetTeleportedNow()
    {
        lastTeleportTime = Time.time;

        if (clearVelocityOnTeleport && cachedRigidbody != null)
        {
            cachedRigidbody.velocity = Vector3.zero;
            cachedRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
