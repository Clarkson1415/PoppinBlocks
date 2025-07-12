using System.Collections.Generic;
using UnityEngine;

public class WinParticleTrigger : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particleSystems;


    public void PlayParticles()
    {
        foreach(var part in particleSystems)
        {
            part.Play();
        }
    }
}
