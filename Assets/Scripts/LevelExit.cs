using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public int level;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Level Clear!");
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            level = currentSceneIndex;
            SceneManager.LoadScene(level);
        }
    }
}
