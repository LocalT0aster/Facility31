using UnityEngine;

[RequireComponent(typeof(EnemyFollow))]
public class EnemyAI : MonoBehaviour, IRoomListener, IProximityListener {
    [Header("Vision / Wander")]
    [SerializeField] private float sightDistance = 100f;
    [SerializeField] private LayerMask sightMask = Physics.DefaultRaycastLayers;
    [SerializeField] private float wanderRayLength = 100f;
    [SerializeField] private float wanderAngleSpread = 25f;

    private enum State { Wander, Chase, Investigate, Return }
    private State state = State.Wander;

    private EnemyFollow mover;
    private Transform player;          // cached player transform
    private Transform pointMarker;     // invisible marker used for every “static” position

    private RoomPresence currentRoom;
    private Vector3 lastSeenPos;    // snapshot of where we lost the player
    private float defaultNotify;  // value to restore when not chasing

    private const string playerTag = "Player";

    void Awake() {
        mover = GetComponent<EnemyFollow>();
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        if (player == null) {
            player = new GameObject("EmptyPlayerMock").transform;
            player.SetParent(null);
            player.position = Vector3.zero;
        }

        // make a persistent marker object
        pointMarker = new GameObject($"{name}_TargetMarker").transform;
        pointMarker.SetParent(null); // To not move it with enemy.
        pointMarker.position = transform.position;

        defaultNotify = mover.NotifyProximity;
        PickNewWanderPoint(); // initial idle target
    }

    void FixedUpdate() {
        switch (state) {
            case State.Chase: DoChase(); break;
            case State.Investigate: DoInvestigate(); break;
            default: break;
        }
    }

    private void DoChase() {
        //if (!player) return;

        //mover.target = player; // follow the player’s transform

        // lost line-of-sight -> investigate last spot
        if (!CanSeePlayer()) {
            lastSeenPos = player.position;
            SwitchState(State.Investigate);
        }
    }

    private void DoInvestigate() {
        if (CanSeePlayer()) SwitchState(State.Chase); // player rediscovered
    }

    // LOS helper
    private bool CanSeePlayer() {
        Vector3 dir = player.position - transform.position;
        if (dir.sqrMagnitude > sightDistance * sightDistance) return false;

        return Physics.Raycast(transform.position, dir.normalized, out var hit, dir.magnitude, sightMask, QueryTriggerInteraction.Ignore)
                  && hit.collider.CompareTag(playerTag);
    }

    // FSM control 
    private void SwitchState(State next) {
        state = next;

        // enable proximity callbacks for every mode except Chase
        mover.NotifyProximity = (state == State.Chase) ? float.MinValue : defaultNotify;

        switch (state) {
            case State.Chase:
                Debug.Log(string.Format("{0} is chasing", name));
                mover.target = player;
                break;

            case State.Investigate:
                Debug.Log(string.Format("{0} is investigating", name));
                pointMarker.position = lastSeenPos;
                mover.target = pointMarker;
                break;

            case State.Return:
                Debug.Log(string.Format("{0} is returning to previous room", name));
                SetRoomCentreTarget();
                break;

            case State.Wander:
                Debug.Log(string.Format("{0} is wandering", name));
                PickNewWanderPoint();
                break;
        }
    }

    // wandering helpers 
    private void PickNewWanderPoint() {
        Vector3[] axes =
        {
            transform.forward, -transform.forward,
            transform.right,   -transform.right,
            transform.up,      -transform.up
        };

        float bestDist = -1f;
        Vector3 bestPos = transform.position;

        foreach (Vector3 axis in axes) {
            Quaternion randRot = Quaternion.Euler(
                Random.Range(-wanderAngleSpread, wanderAngleSpread),
                Random.Range(-wanderAngleSpread, wanderAngleSpread),
                Random.Range(-wanderAngleSpread, wanderAngleSpread));

            Vector3 dir = randRot * axis;

            if (Physics.Raycast(transform.position, dir, out var hit, wanderRayLength,
                                sightMask, QueryTriggerInteraction.Ignore)) {
                if (hit.distance > bestDist) { bestDist = hit.distance; bestPos = hit.point; }
            } else if (wanderRayLength > bestDist) {
                bestDist = wanderRayLength;
                bestPos = transform.position + dir.normalized * wanderRayLength;
            }
        }

        pointMarker.position = bestPos;
        mover.target = pointMarker;
    }

    private void SetRoomCentreTarget() {
        if (!currentRoom) { SwitchState(State.Wander); return; }

        Collider c = currentRoom.GetComponent<Collider>();
        pointMarker.position = c ? c.bounds.center : currentRoom.transform.position;
        mover.target = pointMarker;
    }

    // IRoomListener 
    public void OnEnterRoom(RoomPresence room) => currentRoom = room;
    public void OnExitRoom(RoomPresence room) { return; }

    public void OnPlayerEnterRoom() => SwitchState(State.Chase);
    public void OnPlayerExitRoom() {
        lastSeenPos = player.position;
        SwitchState(State.Investigate);
    }

    // IProximityListener
    public void OnProximity() {
        switch (state) {
            case State.Wander: PickNewWanderPoint(); break;
            case State.Investigate: SwitchState(State.Return); break;
            case State.Return: SwitchState(State.Wander); break;
            default: break;
        }
    }
}
