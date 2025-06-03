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

    // Список врагов в комнате
    private List<GameObject> enemies = new List<GameObject>();
    private bool doorsLocked = false;

    public Vector2Int RoomIndex { get; set; }

    private void Awake()
    {
        // Находим всех врагов в комнате (предполагается, что враги имеют тег "Enemy")
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Enemy"))
            {
                enemies.Add(child.gameObject);
            }
        }
    }

    private void Update()
    {
        if (doorsLocked)
        {
            CheckEnemies();
        }
    }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up && topDoor != null)
            topDoor.SetActive(true);
        if (direction == Vector2Int.down && bottomDoor != null)
            bottomDoor.SetActive(true);
        if (direction == Vector2Int.right && rightDoor != null)
            rightDoor.SetActive(true);
        if (direction == Vector2Int.left && leftDoor != null)
            leftDoor.SetActive(true);
    }

    // Метод для настройки выхода для двери
    public void SetExitPosition(Vector2Int direction, Vector2 position)
    {
        exitPositions[direction] = position;
    }

    // Закрыть все двери, если есть враги
    public void LockDoors()
    {
        // Блокируем двери только если есть враги
        if (enemies.Count > 0)
        {
            doorsLocked = true;
            if (topDoor != null) topDoor.SetActive(false);
            if (bottomDoor != null) bottomDoor.SetActive(false);
            if (rightDoor != null) rightDoor.SetActive(false);
            if (leftDoor != null) leftDoor.SetActive(false);
        }
    }

    // Открыть все двери, соответствующие подключённым комнатам
    public void UnlockDoors()
    {
        doorsLocked = false;
        foreach (var pair in connectedRooms)
        {
            OpenDoor(pair.Key);
        }
    }

    // Проверка, остались ли живые враги
    private void CheckEnemies()
    {
        // Удаляем уничтоженные враги из списка
        enemies.RemoveAll(enemy => enemy == null);

        // Если врагов не осталось, открываем двери
        if (enemies.Count == 0)
        {
            UnlockDoors();
        }
    }

    // Проверка, является ли комната начальной (опционально)
    public bool IsInitialRoom()
    {
        return RoomIndex == new Vector2Int(5, 5); // Начальная комната в центре сетки (gridSizeX/2, gridSizeY/2)
    }
}