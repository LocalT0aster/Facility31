using UnityEngine;

public class CursorLock : MonoBehaviour {
    [SerializeField] bool lockCursor = true;
    [SerializeField] bool hideCursor = true;

    void OnValidate() {
        UpdateLock(keep: true);
    }

    void Awake() {
        UpdateLock(keep: true);
    }

    public void UpdateLock(bool Lock = true, bool hide = true, bool keep = false) {
        if (!keep) {   
            lockCursor = Lock;
            hideCursor = hide;
        }
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hideCursor;
    }
}
