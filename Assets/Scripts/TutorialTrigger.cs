using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject tutorial;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorial.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorial.gameObject.SetActive(false);
        }
    }
}
