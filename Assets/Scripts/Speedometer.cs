using UnityEngine;
using TMPro;
using System;

public class Speedometer : MonoBehaviour {
    public TextMeshProUGUI speedText;
    private Rigidbody rb;
    private const string textFormat = "Speed: {0:F1} m/s";

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        float speed = rb.velocity.magnitude;
        speedText.text = String.Format(textFormat, speed);
    }
}
