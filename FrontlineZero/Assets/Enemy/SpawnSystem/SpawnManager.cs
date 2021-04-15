using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int debugSpawn;
    public bool isRandomSpawing;
    public float spawnHeight;
    public float spawnBorderMargin;
    float scale;
    public float randomSpawnCoolDown;

    public float maxSpawnTime;
    public float minSpawnTime;

    public GameObject[] enemyPrefabs;
    public GameObject capsulePrefab;

    float lastSpawnTime;

    // Singleton
    private static SpawnManager _instance;
    public static SpawnManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if(isRandomSpawing && Time.time > lastSpawnTime + randomSpawnCoolDown)
        {
            SpawnRandomEnemyAtRandomPos();
            randomSpawnCoolDown = maxSpawnTime - (maxSpawnTime - minSpawnTime) * Mathf.Sqrt(RoundManager.Instance.GetDifficultyMultiplier());
            Debug.Log(randomSpawnCoolDown);
            lastSpawnTime = Time.time;
        }
    }

    public void Activate()
    {
        isRandomSpawing = true;
        lastSpawnTime = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        scale = MC_GPU.Instance.chunkCount.x * MC_GPU.Instance.chunkSize;
        randomSpawnCoolDown = maxSpawnTime;
    }

    [ContextMenu("SpawnRandom")]
    public void SpawnRandomEnemyAtRandomPos()
    {
        Vector3 randomPos = NavManager.Instance.GetRandomPosInBounds(spawnBorderMargin);
        randomPos.y = spawnHeight;

        if (debugSpawn < enemyPrefabs.Length)
        {
            SpawnEnemy(debugSpawn, randomPos);
        }
        else
        {
            //SpawnEnemy(Random.Range(0, enemyPrefabs.Length), randomPos);
            SpawnEnemy(GetEnemyIndex(), randomPos);
        }

    }

    int GetEnemyIndex()
    {
        List<int> weights = new List<int>();
        int currentMaximum = Mathf.RoundToInt(RoundManager.Instance.GetDifficultyMultiplier()*enemyPrefabs.Length);

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {

            int count = enemyPrefabs.Length- Mathf.Abs(i-currentMaximum);



            for (int a = 0; a < count; a++)
            {
               // Debug.Log(i);
                weights.Add(i);
            }
        }
        return weights[Random.Range(0, weights.Count)];
    }





    public void SpawnEnemy(int enemyIndex, Vector3 pos)
    {
        SpawnCapsule newCapsule = GameObject.Instantiate(capsulePrefab, pos, Quaternion.identity).GetComponent<SpawnCapsule>();
        newCapsule.enemyIndex = enemyIndex;
    }
}
