using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public VFX[] virtualEffects;
    ParticleManager pManager;


    // Singleton
    private static VFXManager _instance;
    public static VFXManager Instance { get { return _instance; } }

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

    private void Start()
    {
        pManager = ParticleManager.Instance;
    }

    // combine multiple particle systems from ParticleManager to form VFX
    public void PlayVFX(int index, Vector3 pos, Vector3 upVector)
    {
        if(index >= virtualEffects.Length)
        {
            VRAudioDebugger.Instance.DebugFalse();
            Debug.LogError("Hier VFX BUG!!!!!");
            return;
        }
        VFX cVFX = virtualEffects[index];

        for (int i = 0; i < cVFX.particles.Length; i++)
        {
            pManager.PlayParticleSystem(cVFX.particles[i], pos, upVector);
        }
    }

    [System.Serializable]
    public class VFX
    {
        public string VFXName;
        public int[] particles;
    }
}
