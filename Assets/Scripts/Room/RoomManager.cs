using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    [Header("Room generation options")]
    [SerializeField] private GameObject initialRoomPrefab; // Префаб для начальной комнаты (например, Room0)
    [SerializeField] private List<GameObject> roomPrefabs; // Список обычных комнат (Room0, Room1)
    [SerializeField] private GameObject bossRoomPrefab; // Префаб для комнаты с боссом
    [SerializeField] private int maxRooms;
    [SerializeField] private int minRooms;

    [Header("Room size")]
    public int roomWidth;
    public int roomHeight;

    int gridSizeX = 10;
    int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;
    private int roomCount;
    private bool generationComplete = false;
    private Vector2Int bossRoomIndex; // Индекс комнаты с боссом

    public static RoomManager Instance { get; private set; } // Синглтон
    public Room CurrentRoom { get; private set; }

    [Header("Other settings")]
    [SerializeField] private GameObject player;
    // Параметры для правильного позиционирования игрока
    [SerializeField] private float doorOffset = 1.0f; // Расстояние от двери до позиции игрока
    [SerializeField] private float playerRadius = 0.5f; // Примерный размер игрока для избежания застревания

    private bool isTransitioning = false;

    public System.Action OnRoomChanged; // Событие при смене комнаты

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (initialRoomPrefab == null)
        {
            Debug.LogError("Initial Room Prefab не задан в инспекторе!");
            return;
        }
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if (roomCount < minRooms)
        {
            Debug.Log($"Room count less than {minRooms} ({roomCount}). Trying again.");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation Complete, {roomCount} rooms created");
            generationComplete = true;
            PlaceBossRoom();
        }
    }

    public void TransitionToRoom(Room targetRoom, Vector2 exitPosition)
    {
        // Предотвращаем множественные переходы
        if (isTransitioning || RoomTransitionFade.Instance == null) return;

        isTransitioning = true;

        // Запускаем переход с эффектом затемнения
        RoomTransitionFade.Instance.FadeTransition(() => {
            // Этот код выполнится в момент полного затемнения
            PerformRoomTransition(targetRoom, exitPosition);
        });

        // Сбрасываем флаг перехода после завершения эффекта
        StartCoroutine(ResetTransitionFlag());
    }

    private IEnumerator ResetTransitionFlag()
    {
        // Ждем, пока затемнение полностью завершится
        yield return new WaitWhile(() => RoomTransitionFade.Instance.IsTransitioning);
        isTransitioning = false;
    }

    private void PerformRoomTransition(Room targetRoom, Vector2 exitPosition)
    {
        // Обновляем статусы комнат
        if (CurrentRoom != null)
        {
            CurrentRoom.SetStatus(RoomStatus.Explored);
        }

        CurrentRoom = targetRoom;
        CurrentRoom.SetStatus(RoomStatus.Current);

        // Уведомляем миникарту
        OnRoomChanged?.Invoke();

        // Перемещаем игрока
        player.transform.position = exitPosition;
        // Центрируем камеру на новой комнате
        Camera.main.transform.position = new Vector3(
            targetRoom.transform.position.x,
            targetRoom.transform.position.y,
            Camera.main.transform.position.z);

        // Блокируем двери в новой комнате, если там есть враги
        targetRoom.LockDoors();
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;
        // Используем указанный префаб начальной комнаты
        var initialRoom = Instantiate(initialRoomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room - {roomCount} (Initial)";
        var roomScript = initialRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
        roomScript.SetStatus(RoomStatus.Current); // Начальная — текущая
        roomObjects.Add(initialRoom);

        // Инициализируем текущую комнату
        CurrentRoom = roomScript;
        player.transform.position = GetPositionFromGridIndex(roomIndex); // Ставим игрока в начальную комнату
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms)
            return false;

        // Возвращён 50% шанс на генерацию комнаты
        if (Random.value < 0.5f && roomIndex != new Vector2Int(gridSizeX / 2, gridSizeY / 2))
            return false;

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        // Проверка, занята ли ячейка
        if (roomGrid[x, y] != 0) return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        // Случайно выбираем префаб из списка
        GameObject selectedPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
        var newRoom = Instantiate(selectedPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        // Сохраняем индекс последней комнаты для размещения босса
        bossRoomIndex = roomIndex;

        OpenDoors(newRoom, x, y);

        return true;
    }

    private void PlaceBossRoom()
    {
        // Находим последнюю сгенерированную комнату для замены на комнату с боссом
        GameObject lastRoom = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == bossRoomIndex);
        if (lastRoom != null)
        {
            // Удаляем последнюю комнату
            roomObjects.Remove(lastRoom);
            Destroy(lastRoom);

            // Создаём комнату с боссом на том же месте
            var bossRoom = Instantiate(bossRoomPrefab, GetPositionFromGridIndex(bossRoomIndex), Quaternion.identity);
            bossRoom.name = $"Room-Boss";
            bossRoom.GetComponent<Room>().RoomIndex = bossRoomIndex;
            roomObjects.Add(bossRoom);

            // Открываем двери для комнаты с боссом
            OpenDoors(bossRoom, bossRoomIndex.x, bossRoomIndex.y);

            Debug.Log($"Комната с боссом размещена на индексе сетки {bossRoomIndex}");
        }
        else
        {
            Debug.LogWarning("Не удалось найти комнату для замены на комнату с боссом.");
        }
    }

    private void RegenerateRooms()
    {
        // Очищаем все дверные связи у всех комнат
        foreach (var room in roomObjects)
        {
            var roomScript = room.GetComponent<Room>();
            roomScript.ClearConnections();
            Destroy(room);
        }

        roomObjects.Clear();

        // Сбрасываем сетку и очередь
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        // Перезапускаем генерацию с начальной комнатой
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // Подключение левой двери
        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
            newRoomScript.connectedRooms[Vector2Int.left] = leftRoomScript;
            leftRoomScript.connectedRooms[Vector2Int.right] = newRoomScript;

            Vector3 leftRoomCenter = leftRoomScript.transform.position;
            Vector3 newRoomCenter = newRoomScript.transform.position;

            Vector3 exitPosToLeft = leftRoomCenter + new Vector3(roomWidth/2 - doorOffset - playerRadius, 0, 0);
            newRoomScript.SetExitPosition(Vector2Int.left, exitPosToLeft);

            Vector3 exitPosToRight = newRoomCenter + new Vector3(-roomWidth/2 + doorOffset + playerRadius, 0, 0);
            leftRoomScript.SetExitPosition(Vector2Int.right, exitPosToRight);
        }

        // Подключение правой двери
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
            newRoomScript.connectedRooms[Vector2Int.right] = rightRoomScript;
            rightRoomScript.connectedRooms[Vector2Int.left] = newRoomScript;

            Vector3 rightRoomCenter = rightRoomScript.transform.position;
            Vector3 newRoomCenter = newRoomScript.transform.position;

            Vector3 exitPosToRight = rightRoomCenter + new Vector3(-roomWidth/2 + doorOffset + playerRadius, 0, 0);
            newRoomScript.SetExitPosition(Vector2Int.right, exitPosToRight);

            Vector3 exitPosToLeft = newRoomCenter + new Vector3(roomWidth/2 - doorOffset - playerRadius, 0, 0);
            rightRoomScript.SetExitPosition(Vector2Int.left, exitPosToLeft);
        }

        // Подключение нижней двери
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
            newRoomScript.connectedRooms[Vector2Int.down] = bottomRoomScript;
            bottomRoomScript.connectedRooms[Vector2Int.up] = newRoomScript;

            Vector3 bottomRoomCenter = bottomRoomScript.transform.position;
            Vector3 newRoomCenter = newRoomScript.transform.position;

            Vector3 exitPosToBottom = bottomRoomCenter + new Vector3(0, roomHeight/2 - doorOffset - playerRadius, 0);
            newRoomScript.SetExitPosition(Vector2Int.down, exitPosToBottom);

            Vector3 exitPosToTop = newRoomCenter + new Vector3(0, -roomHeight/2 + doorOffset + playerRadius, 0);
            bottomRoomScript.SetExitPosition(Vector2Int.up, exitPosToTop);
        }

        // Подключение верхней двери
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
            newRoomScript.connectedRooms[Vector2Int.up] = topRoomScript;
            topRoomScript.connectedRooms[Vector2Int.down] = newRoomScript;

            Vector3 topRoomCenter = topRoomScript.transform.position;
            Vector3 newRoomCenter = newRoomScript.transform.position;

            Vector3 exitPosToTop = topRoomCenter + new Vector3(0, -roomHeight/2 + doorOffset + playerRadius, 0);
            newRoomScript.SetExitPosition(Vector2Int.up, exitPosToTop);

            Vector3 exitPosToBottom = newRoomCenter + new Vector3(0, roomHeight/2 - doorOffset - playerRadius, 0);
            topRoomScript.SetExitPosition(Vector2Int.down, exitPosToBottom);
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++; // Левый сосед
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // Правый сосед
        if (y > 0 && roomGrid[x, y - 1] != 0) count++; // Нижний сосед
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // Верхний сосед

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2),
              roomHeight * (gridY - gridSizeY / 2));
    }
}