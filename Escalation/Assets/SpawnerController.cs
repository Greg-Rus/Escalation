using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public BouncingController Prefab;

    public Transform SpawnPoint;

    public Transform[] JumpPoints;

    public float StartJumpHeight;

    public float StartJumpDuration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        var bouncable = Instantiate(Prefab, SpawnPoint);
        bouncable.JumpTo(GetRandomJumpPoint(), StartJumpHeight, StartJumpDuration);
    }

    private Vector3 GetRandomJumpPoint()
    {
        var randomIndex = Random.Range(0, JumpPoints.Length);
        return JumpPoints[randomIndex].position;
    }
}
