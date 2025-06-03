using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    [Header("Stats")] 
    public float health = 50f;
    public float speed = 1f;
    
    [Header("Attack Settings")]
    public float damage = 10f;
    public float attackCooldown = 2f;
    public float attackDuration = 0.8f;
    private float _lastAttackTime;
    private bool _isAttacking;
    private bool isTakingDamage;
    private float _takingDamageDuration = 0.5f;
    
    [Header("Damage Effect")]
    [SerializeField] private Color _damageColor = new Color(255, 0, 0, 0.5f);
    [SerializeField] private float _flashDuration = 0.2f;
    
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private bool _isFlashing = false;
    
    private BoxCollider2D _attackCollider;
    private CapsuleCollider2D _collision;
    private CircleCollider2D _detectionCollider;
    
    private GameObject _player; 
    private Animator _animator;
        

    private bool _isPlayerInTrigger;
    private bool _isPlayerDetected;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color; // Сохраняем исходный цвет
        _player = GameObject.FindWithTag("Player");
        _animator = GetComponent<Animator>();
        _attackCollider = GetComponent<BoxCollider2D>();
        _collision = GetComponent<CapsuleCollider2D>();
        _detectionCollider = GetComponent<CircleCollider2D>();
    }
    
    private bool _isDead;
    
    void Update()
    {
        if (!_isDead)
        {
            if (health <= 0)
                    StartCoroutine(Dead());
            Move();
            LocalScaleRotate();
        }
    }

    void Move()
    {
        if (_isPlayerDetected)
        {
            transform.position =
                        Vector2.MoveTowards(transform.position, _player.transform.position, speed * Time.deltaTime);
                    _animator.SetBool(Animator.StringToHash("Walk"), true);
        }
    }

    public void TakeDamage(float damageVal)
    {
       health -= damageVal; 
       
       if (!_isFlashing)
           StartCoroutine(FlashDamage());
       if (SoundManager.Instance != null)
           SoundManager.Instance.PlayEnemyHurt();
       isTakingDamage = true;
        _animator.SetBool(Animator.StringToHash("TakeDamage"), isTakingDamage);
        Invoke(nameof(ResetTakingDamage), _takingDamageDuration);
    }

    private void ResetTakingDamage()
    {
        isTakingDamage = false;
        _animator.SetBool(Animator.StringToHash("TakeDamage"), isTakingDamage);
    }
    private IEnumerator FlashDamage()
    {
        _isFlashing = true;
        
        // Меняем цвет на красный
        _spriteRenderer.color = _damageColor;
        
        // Ждем _flashDuration секунд
        yield return new WaitForSeconds(_flashDuration);
        
        // Возвращаем исходный цвет
        _spriteRenderer.color = _originalColor;
        
        _isFlashing = false;
    }

    
    
    IEnumerator Dead()
    {
        _isDead = true;
        _collision.enabled = false;
        _attackCollider.enabled = false;
        _detectionCollider.enabled = false; 
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Сбрасываем скорость
            rb.isKinematic = true;      // Отключаем физику
            rb.simulated = false;       // Полностью выключаем симуляцию
        }
        _animator.SetBool(Animator.StringToHash("Dead"), true);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.IsTouching(_attackCollider))
            {
                _isPlayerInTrigger = true;
                TryAttack(other.GetComponent<Player>());
            }
            else if (other.IsTouching(_detectionCollider))
            {
                _isPlayerDetected = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_isPlayerInTrigger && other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Player player))
            {
                TryAttack(player);
            }
        }
    }

    private void TryAttack(Player player)
    {
        // Проверяем, прошло ли достаточно времени с последней атаки
        if (Time.time - _lastAttackTime >= attackCooldown)
        {
            _lastAttackTime = Time.time;
            _isAttacking = true;
            _animator.SetBool("Attack", true);
        
            // Наносим урон (можно добавить задержку, если нужно)
            player.TakeDamage(damage);
        
            // Отключаем атаку через время (например, 0.5 секунды)
            Invoke(nameof(ResetAttack), 0.4f);
        }
    }

    private void ResetAttack()
    {
        _isAttacking = false;
        _animator.SetBool("Attack", false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
            _isPlayerDetected = true;
        }
    }

    private float skeletonSize = 1.5f;
    void LocalScaleRotate()
    {
        if (_player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(skeletonSize, skeletonSize, skeletonSize);
        }
        else
        {
            transform.localScale = new Vector3(-skeletonSize, skeletonSize, skeletonSize);
        }
    }
    

}
