using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoomPresence : MonoBehaviour {
    // true if Player is anywhere inside this room
    bool playerPresent = false;

    // all enemies currently inside this room
    readonly List<EnemyAI> enemies = new();

    private const string playerTag = "Player";

    void Awake() {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
        // Player enters
        if (other.CompareTag(playerTag)) {
            Debug.Log(string.Format("Player has entered the {0}", name));
            playerPresent = true;
            // notify every enemy already in the room
            foreach (var e in enemies)  
                e.OnPlayerEnterRoom();
        }
        // Enemy enters
        else if (other.TryGetComponent<EnemyAI>(out var enemy)) {
            Debug.Log(string.Format("{0} has entered the {1}", enemy.gameObject.name, name));
            enemies.Add(enemy);
            enemy.OnEnterRoom(this);
            // if player is already here, immediately notify this enemy
            if (playerPresent)
                enemy.OnPlayerEnterRoom();
        }
    }

    void OnTriggerExit(Collider other) {
        // Player exits
        if (other.CompareTag(playerTag)) {
            Debug.Log(string.Format("Player has left the {0}", name));
            playerPresent = false;
            // notify all enemies still in here
            foreach (var e in enemies)
                e.OnPlayerExitRoom();
        }
        // Enemy exits
        else if (other.TryGetComponent<EnemyAI>(out var enemy)) {
            Debug.Log(string.Format("{0} has left the {1}", enemy.gameObject.name, name));
            enemy.OnExitRoom(this);
            enemies.Remove(enemy);
            // if player is still here, tell this enemy that they just left sight
            if (playerPresent)
                enemy.OnPlayerExitRoom();
        }
    }

    // in case you need it
    public bool IsPlayerPresent => playerPresent;
}
