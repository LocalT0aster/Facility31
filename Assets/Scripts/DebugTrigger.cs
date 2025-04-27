using UnityEngine;

public class DebugTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        Debug.Log(string.Format("{0}, is inside me.", other.gameObject.name));
    }
}
