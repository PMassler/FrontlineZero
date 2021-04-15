using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSource[] particleSources;

    //Singleton
    private static ParticleManager _instance;
    public static ParticleManager Instance { get { return _instance; } }

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

    // play particle systems and increment
    public void PlayParticleSystem(int index, Vector3 pos,Vector3 upVector)
    {
        if (index >= particleSources.Length)
        {
            VRAudioDebugger.Instance.DebugFalse();
            Debug.LogError("Hier Particle BUG!!!!!");
            return;
        }

        ParticleSystem cPS = particleSources[index].childPS[particleSources[index].currentPSCounter];
        cPS.transform.position = pos;
        cPS.transform.up = upVector;
        cPS.Play();
        particleSources[index].currentPSCounter++;
        if (particleSources[index].currentPSCounter >= particleSources[index].count)
        {
            particleSources[index].currentPSCounter = 0;
        }
    }

    // create particle systems
    private void Start()
    {
        for (int i = 0; i < particleSources.Length; i++)
        {
            ParticleSource cPSource = particleSources[i];
            cPSource.childPS = new ParticleSystem[cPSource.count];

            Transform particleSourceHolder = new GameObject(i + "_Holder").transform;
            particleSourceHolder.parent = transform;

            for (int a = 0; a < cPSource.count; a++)
            {
                cPSource.childPS[a] = Instantiate(particleSources[i].originalPS, particleSourceHolder);
            }
        }
    }

    [System.Serializable]
    public class ParticleSource
    {
        public ParticleSystem originalPS;
        [HideInInspector]
        public ParticleSystem[] childPS;

        public int count;
        [HideInInspector]
        public int currentPSCounter = 0;
    }
}
