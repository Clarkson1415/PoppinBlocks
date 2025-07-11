using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class WonLevelText : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            this.animator = this.GetComponent<Animator>();
        }

        public void TurnOn()
        {
            this.animator.SetTrigger("On");
        }
    }
}
