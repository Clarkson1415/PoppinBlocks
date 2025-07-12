using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class WinComponentController : MonoBehaviour
    {
        [SerializeField] private WinParticleTrigger winParticleTrigger;

        private Animator animator;

        public bool IsFinishedAnimating => this.animator.GetCurrentAnimatorStateInfo(0).IsName("StayIn");

        private AudioSource audiosSource;

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
            audiosSource = this.GetComponent<AudioSource>();
        }

        public void TurnOn()
        {
            this.animator.SetTrigger("On");
            this.audiosSource.Play();
            winParticleTrigger.PlayParticles();
        }
    }
}
