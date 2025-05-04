using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject _player;
    void Awake()
    {
        _player = GameObject.FindWithTag("Player");
    }


    void Update()
    {
        transform.position = new Vector3(
            _player.transform.position.x,
            _player.transform.position.y,
            -10);
    }
}
