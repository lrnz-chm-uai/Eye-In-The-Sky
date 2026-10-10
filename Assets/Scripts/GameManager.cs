using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public enum GameState { WaitingToStart, Playing, Won, Lost }

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Vector3 spawnCenter;
    [SerializeField] Vector3 spawnSize;
    [SerializeField] int initialEnemyCount = 5;
    [SerializeField] float respawnDelay = .5f;
    [SerializeField] GameObject orb;
    [SerializeField] Transform playerTransform;
    [SerializeField] float minSpawnDistanceFromPlayer = 10f;
    [SerializeField] int maxSpawnAttempts = 20;

    private float spawnY = 2.5f;

    public GameState CurrentState { get; private set; } = GameState.WaitingToStart;

    // Se dispara cuando el juego termina (Won o Lost). UIManager lo usa para re-mostrar el botón.
    public event Action OnGameEnded;

    private int aliveEnemies = 0;
    private PlayerController playerController;
    private CameraController cameraController;

    void OnEnable() // OnEnable es llamado cuando el objeto se activa en la escena
    {
        // Suscripción al evento OnEnemyDestroyed del EnemyController
        EnemyController.OnEnemyDestroyed += HandleEnemyDestroyed; // con += llamo al método HandleEnemyDestroyed
        // Lo que termina disparando el evento es una corutina

        PlayerController.OnPlayerDestroyed += HandlePlayerDestroyed;
    }

    void OnDisable()
    {
        EnemyController.OnEnemyDestroyed -= HandleEnemyDestroyed;
        PlayerController.OnPlayerDestroyed += HandlePlayerDestroyed;
    }

    void Awake()
    {
        Debug.Log("GameManager count: " + FindObjectsOfType<GameManager>().Length);
    }

    void Start()
    {
        // El juego NO arranca solo: queda en WaitingToStart hasta que UIManager llame a StartGame()
        if (playerTransform != null)
        {
            playerController = playerTransform.GetComponent<PlayerController>();
            cameraController = playerTransform.GetComponentInChildren<CameraController>();
        }
    }

    void Update()
    {
        // Victoria: orb destruido y no quedan enemigos vivos.
        // Se evalúa acá porque el orb puede morir después del último enemigo.
        if (CurrentState == GameState.Playing && orb == null && aliveEnemies == 0)
        {
            EndGame(GameState.Won);
        }
    }

    // Llamado por UIManager al clickear StartButton en WaitingToStart
    public void StartGame()
    {
        if (CurrentState != GameState.WaitingToStart) return;
        if (enemyPrefab == null) return;

        CurrentState = GameState.Playing;
        for (int i = 0; i < initialEnemyCount; i++)
        {
            SpawnEnemyAtRandomPosition();
        }

        if (playerController != null)
        {
            playerController.SetInputEnabled(true);
        }

        if (cameraController != null)
        {
            cameraController.SetInputEnabled(true);
        }

    }

    private void EndGame(GameState result)
    {
        CurrentState = result;
        Debug.Log("Game ended: " + result);
        // Bloquear la cámara hasta que se inicie una nueva partida
        if (cameraController != null)
        {
            cameraController.SetInputEnabled(false);
        }
        OnGameEnded?.Invoke();
    }

    private void HandleEnemyDestroyed(EnemyController enemy)
    {
        if (enemy == null) return;
        aliveEnemies--;

        // Sólo respawnear si el juego sigue en curso
        if (CurrentState != GameState.Playing) return;

        Debug.Log("Enemy destroyed, starting respawn coroutine.");
        StartCoroutine(RespawnAfterDelay(respawnDelay));
    }

    private void HandlePlayerDestroyed()
    {
        // Derrota: colisión con Enemy y Player destruido
        if (CurrentState != GameState.Playing) return;
        EndGame(GameState.Lost);
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Chequeo extra por si el juego terminó mientras corría el delay
        if (CurrentState == GameState.Playing && orb != null)
        {
            SpawnEnemyAtRandomPosition();
        }
    }
    private void SpawnEnemyAtRandomPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        bool found = false;
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 randomOffset = new Vector3(
                (Random.value - 0.5f) * spawnSize.x,
                0f,
                (Random.value - 0.5f) * spawnSize.z
            );

            Vector3 candidate = spawnCenter + randomOffset;
            candidate.y = spawnY;

            Vector2 playerXZ = new Vector2(playerTransform.position.x, playerTransform.position.z);
            Vector2 candXZ = new Vector2(candidate.x, candidate.z);
            float dist = Vector2.Distance(playerXZ, candXZ);
            if (dist >= minSpawnDistanceFromPlayer)
            {
                spawnPos = candidate;
                found = true;
                Debug.Log($"Spawned enemy at {spawnPos} after {attempt + 1} attempts.");
                break;
            }
        }

        if (!found)
        {
            // fallback: en caso de que ninguno de los intentos haya sido válido, spawnear en el centro del área de spawn
            Debug.LogWarning("Could not find a valid spawn position after max attempts. Spawning at fallback position.");
            Vector3 fallback = spawnCenter;
            fallback.y = spawnY;
            if (playerTransform != null)
            {
                Vector3 dir = (fallback - playerTransform.position);
                dir.y = 0f;
                if (dir.sqrMagnitude < 0.001f) dir = Vector3.forward;
                dir.Normalize();
                fallback = playerTransform.position + dir * minSpawnDistanceFromPlayer;
                fallback.y = spawnY;
            }
            spawnPos = fallback;
        }

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        aliveEnemies++;
        Debug.Log($"Enemy instatiation at {spawnPos}");
    }

    // Para visualizar el área de spawn en la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawCube(spawnCenter, spawnSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        // TODO: Para visualizar la distancia mínima de spawn, relativa al jugador
        //if (playerTransform != null)
        //{
        //    Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        //    Vector3 center = playerTransform.position;
        //    center.y = spawnY;
        //    Gizmos.DrawSphere(center, 0.1f);
        //    // Dibuja un círculo alrededor del jugador para indicar la distancia mínima de spawn 
        //    int steps = 24;
        //    for (int i = 0; i < steps; i++)
        //    {
        //        float a = (i / (float)steps) * Mathf.PI * 2f;
        //        Vector3 p = center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * minSpawnDistanceFromPlayer;
        //        Gizmos.DrawSphere(p, 0.08f);
        //    }
        //}
    }
}
