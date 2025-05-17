using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject prefabBullet;
    public int damage;
    public float bulletForce;
    public float fireRate;
    private float _timeFireRate;
    private GameObject _posSpawnBullet;
    private GameObject _cursor;

    void Awake()
    {
        _posSpawnBullet = transform.GetChild(0).gameObject;
        _cursor = GameObject.FindWithTag("Cursor");
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    void Fire()
    {
        if(_timeFireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                GameObject bullet = Instantiate(prefabBullet, _posSpawnBullet.transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = 
                    (_cursor.transform.position - transform.position).normalized * bulletForce;
                bullet.GetComponent<Bullet>().SetDamage(damage);
                _timeFireRate = fireRate;
            }
        }
        else
        {
            _timeFireRate-=Time.deltaTime;
        }
    }
}
