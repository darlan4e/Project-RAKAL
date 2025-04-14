using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private GameObject _cursor;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _cursor = GameObject.FindWithTag("Cursor");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Animation();
        LocalScaleRotate();
    }

    void Move()
    {
        float hor = Input.GetAxis("Debug Horizontal");
        float ver = Input.GetAxis("Debug Vertical");

        _rigidbody2D.velocity = new Vector2(hor, ver).normalized * speed;
    }

    void Animation()
    {
        if (_rigidbody2D.velocity == Vector2.zero) 
        {
            _animator.SetBool(Animator.StringToHash("Walk"),false);
        }
        else 
        {
            _animator.SetBool(Animator.StringToHash("Walk"),true);
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
