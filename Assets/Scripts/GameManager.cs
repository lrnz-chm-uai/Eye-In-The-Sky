using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
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

    void OnEnable() // OnEnable es llamado cuando el objeto se activa en la escena
    {
        // Suscripción al evento OnEnemyDestroyed del EnemyController
        EnemyController.OnEnemyDestroyed += HandleEnemyDestroyed; // con += llamo al método HandleEnemyDestroyed
        // Lo que termina disparando el evento es una corutina
    }

    void OnDisable()
    {
        EnemyController.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    void Start()
    {
        if (enemyPrefab == null) return;

        for (int i = 0; i < initialEnemyCount; i++)
        {
            SpawnEnemyAtRandomPosition();
        }
    }

    private void HandleEnemyDestroyed(EnemyController enemy)
    {
        // Empieza la corrutina para respawnear un enemigo después de un retraso
        StartCoroutine(RespawnAfterDelay(respawnDelay));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Sólo spawnear un enemigo si el Orbe no fue destruido
        if (orb != null)
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
                break;
            }
        }

        if (!found)
        {
            // fallback: en caso de que ninguno de los intentos haya sido válido, spawnear en el centro del área de spawn
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
    }

    // Para visualizar el área de spawn en la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawCube(spawnCenter, spawnSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        // Para visualizar la distancia mínima de spawn, relativa al jugador
        if (playerTransform != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Vector3 center = playerTransform.position;
            center.y = spawnY;
            Gizmos.DrawSphere(center, 0.1f);
            // Dibuja un círculo alrededor del jugador para indicar la distancia mínima de spawn 
            int steps = 24;
            for (int i = 0; i < steps; i++)
            {
                float a = (i / (float)steps) * Mathf.PI * 2f;
                Vector3 p = center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * minSpawnDistanceFromPlayer;
                Gizmos.DrawSphere(p, 0.08f);
            }
        }
    }
}
