using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldWeapon : MonoBehaviour
{
    private GameObject _cursor;
    private GameObject _player;

    public float distance;

    void Awake()
    {
        _cursor = GameObject.FindWithTag("Cursor");
        _player = GameObject.FindWithTag("Player");
    }
    void Update()
    {
        transform.position = _player.transform.position;
        Vector3 difference = _cursor.transform.position - _player.transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        transform.GetChild(0).transform.localPosition = new Vector3(distance, 0, 0);
    }
}
