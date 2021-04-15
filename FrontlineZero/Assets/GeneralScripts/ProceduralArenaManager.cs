using UnityEngine;

public class ProceduralArenaManager : MonoBehaviour
{
    public int debrisCount;
    public int craterCount;
    public int objCountPerRubble;
    public float objectSpawnHeightDistance;   

    public CraterObject[] craterObjects;
    public DebrisObject[] debrisObjects;
    public GameObject[] objPrefabs;

    // Singleton
    private static ProceduralArenaManager _instance;
    public static ProceduralArenaManager Instance { get { return _instance; } }

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



    [ContextMenu("Generate")]
    public void ProceduralGeneration()
    {
        GenerateCraters();
        GenerateDebris();
    }


    void GenerateCraters()
    {
        for (int i = 0; i < craterCount; i++)
        {
            int index = Random.Range(0, craterObjects.Length);

            Vector3 debrisPos = NavManager.Instance.GetRandomPosInBounds(5f);
            debrisPos.y = 40f + Random.Range(-craterObjects[index].heightRandomness / 2f, craterObjects[index].heightRandomness / 2f);
            craterObjects[index].exObj.Explode(debrisPos, Quaternion.identity);

            PlaceDebrisObjects(craterObjects[index].objectCount, debrisPos + Vector3.up * craterObjects[index].objectSpawnHeight, craterObjects[index].objectSpawnRadius);
        }
    }


    void GenerateDebris()
    {
        for (int i = 0; i < debrisCount; i++)
        {
            int index = Random.Range(0, debrisObjects.Length);

            Vector3 debrisPos = NavManager.Instance.GetRandomPosInBounds(5f);
            debrisPos.y = 40f + Random.Range(-debrisObjects[index].heightRandomness / 2f, debrisObjects[index].heightRandomness / 2f);
            debrisObjects[index].exObj.Explode(debrisPos, Quaternion.identity);

            PlaceDebrisObjects(debrisObjects[index].objectCount, debrisPos + Vector3.up * debrisObjects[index].objectSpawnHeight, debrisObjects[index].objectSpawnRadius);
        }
    }

    void PlaceDebrisObjects(int count, Vector3 pos, float radius)
    {
        int counter = 0;
        for (int i = 0; i < count; i++)
        {
            float centerDistance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(radius, 2)));
            Vector3 spawnPos = pos + Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * new Vector3(centerDistance, 0f, 0f) + Vector3.up * counter * objectSpawnHeightDistance; 

            Instantiate(objPrefabs[Random.Range(0, objPrefabs.Length)], spawnPos, Quaternion.identity);

            counter++;
        }
    }



    [System.Serializable]
    public class CraterObject
    {
        public float heightRandomness;
        public int objectCount;
        public float objectSpawnHeight;
        public float objectSpawnRadius;
        public ExplodingObject exObj;
    }

    [System.Serializable]
    public class DebrisObject
    {
        public float heightRandomness;
        public int objectCount;
        public float objectSpawnHeight;
        public float objectSpawnRadius;
        public ExplodingObject exObj;
    }
}
