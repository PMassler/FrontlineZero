using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public SFXSource[] SFXSources;

    // Singleton
    private static SFXManager _instance;
    public static SFXManager Instance { get { return _instance; } }

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

    // play audiosource and increment counter
    public void PlaySFX(int index, Vector3 pos)
    {
        if (index >= SFXSources.Length || index < 0)
        {            
            return;
        }

        AudioSource cAS = SFXSources[index].childAS[SFXSources[index].currentASCounter];
        cAS.transform.position = pos;
        cAS.Play();
        SFXSources[index].currentASCounter++;
        if(SFXSources[index].currentASCounter >= SFXSources[index].count)
        {
            SFXSources[index].currentASCounter = 0;
        }
    }

    // create audiosources
    void Start()
    {
        for (int i = 0; i < SFXSources.Length; i++)
        {
            SFXSource cSFXSource = SFXSources[i];
            cSFXSource.childAS = new AudioSource[cSFXSource.count];

            Transform audioSourceHolder = new GameObject(i + "_Holder").transform;
            audioSourceHolder.parent = transform;

            for (int a = 0; a < cSFXSource.count; a++)
            {
                cSFXSource.childAS[a] = Instantiate(SFXSources[i].originalAS, audioSourceHolder);
            }
        }
    }
   
    [System.Serializable]
    public class SFXSource
    {
        public AudioSource originalAS;
        [HideInInspector]
        public AudioSource[] childAS;

        public int count;
        [HideInInspector]
        public int currentASCounter = 0;
    }
}
