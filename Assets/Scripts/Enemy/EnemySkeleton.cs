using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    [Header("Movement")] 
    public float speed = 1f;

    [Header("Stats")] 
    public float health = 50f;
    public float damage = 10f;

    private GameObject _player;
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
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
        transform.position =
            Vector2.MoveTowards(transform.position, _player.transform.position, speed * Time.deltaTime);
        _animator.SetBool(Animator.StringToHash("Walk"), true);
    }
    
    public void TakeDamage(float damageVal) => health -= damageVal;
    
    IEnumerator Dead()
    {
        _isDead = true;
        _boxCollider2D.enabled = false;
        _animator.SetBool(Animator.StringToHash("Dead"), true);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }
    }
    void LocalScaleRotate()
    {
        if (_player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    

}
