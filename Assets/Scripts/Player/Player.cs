using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float dashSpeed;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private GameObject _cursor;
    public float health;
    private InputManager _inputActions;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _inputActions = new InputManager();
        _animator = GetComponent<Animator>();
        _cursor = GameObject.FindWithTag("Cursor");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("жопа");
    }

    // Update is called once per frame
    void Update()
    {
        Health();
        Move();
        //Dash();
        Animation();
        LocalScaleRotate();
    }

    //void Dash(){}
    
    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
    
    void Move()
    {
        //Получаем ввод из кастомного действия Movement
        Vector2 moveInput = _inputActions.Player.Movement.ReadValue<Vector2>();
        
        // Применяем движение
        _rigidbody2D.velocity = moveInput.normalized * speed;
    }
    
    void Health()
    {

    }

    private void Animation()
    {
        // Получаем текущий ввод (даже если он нулевой)
        Vector2 moveInput = _inputActions.Player.Movement.ReadValue<Vector2>();
    
        // Проверяем, есть ли ввод (более точная проверка чем сравнение velocity)
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
    
        _animator.SetBool(Animator.StringToHash("Walk"), isMoving);
    }

    void LocalScaleRotate()
    {
        if(_cursor.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
