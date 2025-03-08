using UnityEngine;

public class CursorLock : MonoBehaviour {
    public bool LockCursor = true;
    public bool HideCursor = true;

    void OnValidate() {
        UpdateLock();
    }

    void Awake() {
        UpdateLock();
    }

    void UpdateLock() {
        Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = HideCursor;
    }
}
