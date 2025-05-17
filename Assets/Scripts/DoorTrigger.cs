using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Vector2Int direction; // Направление двери (up, down, left, right)
    private Room currentRoom;

    private void Awake()
    {
        currentRoom = GetComponentInParent<Room>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Room targetRoom = currentRoom.connectedRooms[direction];
            Vector2 exitPosition = currentRoom.exitPositions[direction];
            RoomManager.Instance.TransitionToRoom(targetRoom, exitPosition);
        }
    }
}