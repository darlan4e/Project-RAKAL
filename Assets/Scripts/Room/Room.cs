using System.Collections.Generic;
using UnityEngine;

public enum RoomStatus
{
    Unexplored,   // Не исследована (серая)
    Explored,     // Исследована (белая)
    Current       // Текущая (жёлтая)
}

public class Room : MonoBehaviour
{
    [SerializeField] public GameObject topClosedDoor;
    [SerializeField] public GameObject bottomClosedDoor;
    [SerializeField] public GameObject rightClosedDoor;
    [SerializeField] public GameObject leftClosedDoor;
    [SerializeField] public GameObject topOpenDoor;
    [SerializeField] public GameObject bottomOpenDoor;
    [SerializeField] public GameObject rightOpenDoor;
    [SerializeField] public GameObject leftOpenDoor;

    // Связанные комнаты для каждой двери
    public Dictionary<Vector2Int, Room> connectedRooms = new Dictionary<Vector2Int, Room>();

    // Позиции выхода для каждой двери (где игрок появится в новой комнате)
    public Dictionary<Vector2Int, Vector2> exitPositions = new Dictionary<Vector2Int, Vector2>();

    // Список врагов в комнате
    private List<GameObject> enemies = new List<GameObject>();
    private bool doorsLocked = false;

    public Vector2Int RoomIndex { get; set; }

    // ✅ Статус комнаты для миникарты
    public RoomStatus Status = RoomStatus.Unexplored;

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

        // Изначально показываем открытые двери для подключённых комнат
        UpdateDoorVisibility();
    }

    private void Update()
    {
        if (doorsLocked)
        {
            CheckEnemies();
        }
    }

    // ✅ Метод для установки статуса комнаты (для миникарты)
    public void SetStatus(RoomStatus status)
    {
        Status = status;
    }

    // ✅ Метод для очистки связей (нужен при регенерации)
    public void ClearConnections()
    {
        connectedRooms.Clear();
        exitPositions.Clear();
    }

    public void OpenDoor(Vector2Int direction)
    {
        // Показываем открытую дверь и скрываем закрытую
        if (direction == Vector2Int.up)
        {
            if (topOpenDoor != null) topOpenDoor.SetActive(true);
            if (topClosedDoor != null) topClosedDoor.SetActive(false);
        }
        if (direction == Vector2Int.down)
        {
            if (bottomOpenDoor != null) bottomOpenDoor.SetActive(true);
            if (bottomClosedDoor != null) bottomClosedDoor.SetActive(false);
        }
        if (direction == Vector2Int.right)
        {
            if (rightOpenDoor != null) rightOpenDoor.SetActive(true);
            if (rightClosedDoor != null) rightClosedDoor.SetActive(false);
        }
        if (direction == Vector2Int.left)
        {
            if (leftOpenDoor != null) leftOpenDoor.SetActive(true);
            if (leftClosedDoor != null) leftClosedDoor.SetActive(false);
        }
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
            UpdateDoorVisibility();
        }
    }

    // Открыть все двери, соответствующие подключённым комнатам
    public void UnlockDoors()
    {
        doorsLocked = false;
        UpdateDoorVisibility();
    }

    // Обновление видимости дверей в зависимости от состояния
    private void UpdateDoorVisibility()
    {
        // Сбрасываем все двери
        if (topOpenDoor != null) topOpenDoor.SetActive(false);
        if (bottomOpenDoor != null) bottomOpenDoor.SetActive(false);
        if (rightOpenDoor != null) rightOpenDoor.SetActive(false);
        if (leftOpenDoor != null) leftOpenDoor.SetActive(false);
        if (topClosedDoor != null) topClosedDoor.SetActive(false);
        if (bottomClosedDoor != null) bottomClosedDoor.SetActive(false);
        if (rightClosedDoor != null) rightClosedDoor.SetActive(false);
        if (leftClosedDoor != null) leftClosedDoor.SetActive(false);

        // Показываем нужные двери
        foreach (var pair in connectedRooms)
        {
            if (doorsLocked)
            {
                // Показываем закрытые двери
                if (pair.Key == Vector2Int.up && topClosedDoor != null) topClosedDoor.SetActive(true);
                if (pair.Key == Vector2Int.down && bottomClosedDoor != null) bottomClosedDoor.SetActive(true);
                if (pair.Key == Vector2Int.right && rightClosedDoor != null) rightClosedDoor.SetActive(true);
                if (pair.Key == Vector2Int.left && leftClosedDoor != null) leftClosedDoor.SetActive(true);
            }
            else
            {
                // Показываем открытые двери
                OpenDoor(pair.Key);
            }
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

    // Проверка, является ли комната начальной
    public bool IsInitialRoom()
    {
        return RoomIndex == new Vector2Int(5, 5); // Начальная комната в центре сетки (gridSizeX/2, gridSizeY/2)
    }
}