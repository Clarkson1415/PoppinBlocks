using UnityEngine;

namespace Assets.Scripts
{
    public class Dust : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void PlayDust()
        {
            this.animator.SetTrigger("Dust");
        }
    }
}
