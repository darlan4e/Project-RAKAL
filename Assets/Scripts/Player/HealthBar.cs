using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Player _player;
    public Image health;
    
    void Awake()
    {
        _player = FindObjectOfType<Player>();
    }


    void Update()
    {
        health.fillAmount = _player.health / _player.MaxHealth;
    }
}
