using System.Collections.Generic;
using UnityEngine;

public class SupplyManager : MonoBehaviour
{    
    public bool isRandomSpawing;
    public float spawnHeight;
    public float spawnBorderMargin;
    float scale;
    public float randomSpawnCoolDown;

    public GameObject[] powerUpPrefab;
    public GameObject supplyDropPrefab;

    float lastSpawnTime;

    public Parachute parachutePrefab;

    public Animator supplyShipAnimator;

    public float supplyDropSideSpeed;

    public int[] weightDistribution;
    int[] weights;

    int nextSpawnIndex;
    Vector3 nextSpawnPos;
    Vector3 nextSpawnDirection;

    // Singleton
    private static SupplyManager _instance;
    public static SupplyManager Instance { get { return _instance; } }

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
        if (isRandomSpawing && Time.time > lastSpawnTime + randomSpawnCoolDown)
        {
            SpawnPowerUpAtRandomPos();
            lastSpawnTime = Time.time;
        }
    }

    public void Activate()
    {
        isRandomSpawing = true;
        lastSpawnTime = Time.time;
    }


    void Start()
    {
        scale = MC_GPU.Instance.chunkCount.x * MC_GPU.Instance.chunkSize;
        // Set weights
        List<int> weightList = new List<int>();
        for (int i = 0; i < weightDistribution.Length; i++)
        {
            for (int a = 0; a < weightDistribution[i]; a++)
            {
                weightList.Add(i);
            }
        }
        weights = weightList.ToArray();
    }

    [ContextMenu("SpawnRandom")]
    public void SpawnPowerUpAtRandomPos()
    {
        nextSpawnPos = NavManager.Instance.GetRandomPosInBounds(spawnBorderMargin);
        nextSpawnPos.y = spawnHeight;        
        nextSpawnIndex = weights[Random.Range(0, weights.Length)];       
        nextSpawnDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));       
        SendSupplyShip();
    }

    // trigger supplyship animation
    public void SendSupplyShip()
    {
        supplyShipAnimator.transform.position = nextSpawnPos;
        supplyShipAnimator.SetTrigger("Drop");
        supplyShipAnimator.transform.forward = nextSpawnDirection;       
        Invoke("SpawnPowerUp", 2.5f);
    }

    // create new powerup
    public void SpawnPowerUp()
    {
        SupplyDrop newSupplyDrop = GameObject.Instantiate(supplyDropPrefab, nextSpawnPos, Quaternion.identity).GetComponent<SupplyDrop>();
        Parachute parachute = Instantiate(parachutePrefab, nextSpawnPos, Quaternion.identity);
        Rigidbody supplyDropRB = newSupplyDrop.GetComponent<Rigidbody>();
        parachute.target = supplyDropRB;
        supplyDropRB.velocity = nextSpawnDirection * supplyDropSideSpeed;
        newSupplyDrop.powerUpIndex = nextSpawnIndex;
    }
}
