using UnityEngine;

public class OpenLink : MonoBehaviour
{
    [SerializeField] string link;

    public void OpenALink()
    {
        Application.OpenURL(link);
    }
}
