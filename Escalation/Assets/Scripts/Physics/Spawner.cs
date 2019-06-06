using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Bouncable;
    public float SpawnInterval;
    public float SpawnImpulse;
    private float _spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        _spawnTimer = SpawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpawnTimer();
    }

    private void UpdateSpawnTimer()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            _spawnTimer = SpawnInterval;
            OnSpawnTimerExpired();
        }
    }

    private void OnSpawnTimerExpired()
    {
        SpawnBody();
    }

    private void SpawnBody()
    {
        var go = Instantiate(Bouncable, transform.position, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * SpawnImpulse, ForceMode.Impulse);
    }
}
