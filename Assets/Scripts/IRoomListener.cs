public interface IRoomListener {
    void OnEnterRoom(RoomPresence room);
    void OnExitRoom(RoomPresence room);
    void OnPlayerEnterRoom();
    void OnPlayerExitRoom();
}
