using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void Toggle()
    {
        animator.SetTrigger("Toggle");
    }
}
