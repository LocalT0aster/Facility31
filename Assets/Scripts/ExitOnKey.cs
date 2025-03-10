using UnityEngine;

public class ExitOnKey : MonoBehaviour
{
    public KeyCode key = KeyCode.Escape;
    void Update() {
        if (Input.GetKeyDown(key)) Application.Quit(0);
    }
}
