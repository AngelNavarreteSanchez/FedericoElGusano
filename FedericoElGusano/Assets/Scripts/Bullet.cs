using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Bullet : MonoBehaviour
{
    private Animator animatorController;
    private float bulletSpeed = 5f;
    private Rigidbody2D rb;
    private AudioManagerScript _audioManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();
        _audioManager = AudioManagerScript.instance;
    }
    private void Update()
    {
        DirectionBullet();
    }
    IEnumerator DestroyBulletAfeterTime()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
      
    }
    public void DirectionBullet()
    {
        Vector3 movimiento = Vector3.right * bulletSpeed * Time.deltaTime;
        transform.Translate(movimiento);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pared") || other.CompareTag("Obstacle"))
        {
            AnimateExplotion();
            gameObject.SetActive(false);    
        }
    }
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _audioManager.PlaySFX(_audioManager.killEnemy);
            Destroy(collision.gameObject);
            GameManager.enemysDefeated++;
        }
    }
    private void AnimateExplotion()
    {
        //Debug.Log("se ha activado la animacion");
        //animatorController.SetBool("isExploted", true);
        rb.velocity = Vector3.zero;
        StartCoroutine(DestroyBulletAfeterTime());
    }
}
