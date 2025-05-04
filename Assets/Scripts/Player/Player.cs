using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float dashSpeed;
    public float dashDistance;
    public float dashDuration;
    public float dashCooldown;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private GameObject _cursor;
    public float health;
    private InputManager _inputActions;
    private bool _isDashing = false;
    private bool _canDash = true;
    private Vector2 _dashDirection;
    private Vector2 _dashStartPosition;
    private float _dashProgress = 0f;

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
        Debug.Log(" ");
    }

    // Update is called once per frame
    void Update()
    {
        Health();
        Move();
        Dash();
        Animation();
        LocalScaleRotate();
    }

    private void Dash()
    {
        if (_inputActions.Player.Dash.triggered)
        {
            Debug.Log("Dash triggered!");
            if (_canDash && !_isDashing)
            {
                Debug.Log("Starting dash coroutine");
                StartCoroutine(PerformDash());
            }
            else
            {
                Debug.Log($"Can't dash: _canDash={_canDash}, _isDashing={_isDashing}");
            }
        }
    }

    private IEnumerator PerformDash()
    {
        Debug.Log("Dash started");
        _canDash = false;
        _isDashing = true;
        
        // Store the current movement direction
        _dashDirection = _inputActions.Player.Movement.ReadValue<Vector2>();
        Debug.Log($"Dash direction: {_dashDirection}");
        
        if (_dashDirection == Vector2.zero)
        {
            // If not moving, dash in the direction the player is facing
            _dashDirection = _cursor.transform.position.x > transform.position.x ? Vector2.right : Vector2.left;
            Debug.Log($"No movement input, dashing in facing direction: {_dashDirection}");
        }

        _dashDirection = _dashDirection.normalized;
        _dashStartPosition = _rigidbody2D.position;
        _dashProgress = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            _dashProgress = elapsedTime / dashDuration;
            // Используем квадратичную кривую для более плавного ускорения и замедления
            float smoothProgress = _dashProgress * _dashProgress * (3f - 2f * _dashProgress);
            
            // Вычисляем текущую позицию
            Vector2 targetPosition = _dashStartPosition + (_dashDirection * dashDistance * smoothProgress);
            _rigidbody2D.MovePosition(targetPosition);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Гарантируем, что мы достигли конечной позиции
        Vector2 finalPosition = _dashStartPosition + (_dashDirection * dashDistance);
        _rigidbody2D.MovePosition(finalPosition);
        
        _isDashing = false;
        Debug.Log("Dash ended");
        
        // Start cooldown
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
        Debug.Log("Dash cooldown ended");
    }
    
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

        // Устанавливаем анимацию рывка
        _animator.SetBool(Animator.StringToHash("Dash"), _isDashing);

        // Если есть горизонтальное движение, поворачиваем спрайт
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            transform.localScale = new Vector3(moveInput.x > 0 ? 1 : -1, 1, 1);
        }
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
