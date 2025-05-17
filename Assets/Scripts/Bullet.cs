using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
  private int _damage;
  public void SetDamage(int damage) => _damage = damage;
  
  private IEnumerator Start()
  {
    yield return new WaitForSeconds(5f);
    Destroy(gameObject);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Enemy")
    {
      // other.GetComponent<Enemy>().TakeDamage(_damage);
      Destroy(gameObject);
    }
      
      
  }
}
