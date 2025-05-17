using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public GameObject topDoor;
    [SerializeField] public GameObject bottomDoor;
    [SerializeField] public GameObject rightDoor;
    [SerializeField] public GameObject leftDoor;

    // Связанные комнаты для каждой двери
    public Dictionary<Vector2Int, Room> connectedRooms = new Dictionary<Vector2Int, Room>();

    // Позиции выхода для каждой двери (где игрок появится в новой комнате)
    public Dictionary<Vector2Int, Vector2> exitPositions = new Dictionary<Vector2Int, Vector2>();

    public Vector2Int RoomIndex { get; set; }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            topDoor.SetActive(true);
        if (direction == Vector2Int.down)
            bottomDoor.SetActive(true);
        if (direction == Vector2Int.right)
            rightDoor.SetActive(true);
        if (direction == Vector2Int.left)
            leftDoor.SetActive(true);
    }

    // Метод для настройки выхода для двери
    public void SetExitPosition(Vector2Int direction, Vector2 position)
    {
        exitPositions[direction] = position;
    }
}