using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class ChatacterMovement : MonoBehaviour
{
    public static event Action OnHit = delegate { };
    public static event Action Charge = delegate { };

    private Animator _animator;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private SpriteRenderer _sr;
    private GameObject _platform;
    private AudioManagerScript _audioManager;
    public InventoryManager Inventory;

    [Header("Jump")]
    public float jumpForce;

    [Header("Bullet")]
    [SerializeField] public Transform bulletDirection;
    public Bullet _bullet;

    [Header("GroundCheck")]
    [SerializeField]private Transform _groundCheck;
    public LayerMask _groundLayer;
    private UIManager _uiManager;
    private bool _isGroundedDown;

    [SerializeField]private float invincibilityDurationSeconds;
    public bool _isInvicible;
    public static bool gameOver;
    void Start()
    {
        _isInvicible= false;
        _uiManager = UIManager.instance;
        _animator = gameObject.GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = gameObject.GetComponent<BoxCollider2D>();
        _audioManager = AudioManagerScript.instance;
        Inventory.actualArmor = Inventory.inventory.armor;
        Inventory.actualCharge = Inventory.inventory.charge;
        gameOver = false;

        Debug.Log(PlayerPrefs.GetInt("monedas"));
    }

    // Update is called once per frame
    void Update()
    {
        Salto();
        PlayerShoot();
    }
    void Salto()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            
           
            _rb.velocity = new Vector3(0, jumpForce, 0);
            _animator.SetBool("isJumping", true);
            ActivateTrigger();
            Invoke(nameof(DesactivateTrigger), 0.6f);
            _audioManager.PlaySFX(_audioManager.jump);
            if (_isGroundedDown) StartCoroutine(DesactivateJumpFromUnderground());
            else StartCoroutine(DesactivateJump());
            _rb.gravityScale = 1;

        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) && IsGrounded() && _isGroundedDown==false)
        {
            _rb.gravityScale = 1;
            _animator.SetBool("isJumping", true);
            ActivateTrigger();
            _audioManager.PlaySFX(_audioManager.jump);
            Invoke(nameof(DesactivateTrigger), 0.6f);
            StartCoroutine(DesactivateJump());
        }
    }
    public void ResetGravity()
    {
        _rb.gravityScale = 1;
    }
    IEnumerator DesactivateJumpFromUnderground()
    {
        yield return new WaitForSeconds(0.8f);
        _animator.SetBool("isJumping", false);
    }
    IEnumerator DesactivateJump()
    {
        yield return new WaitForSeconds(0.6f);
        _animator.SetBool("isJumping", false);
        _rb.gravityScale = 4;
    }
    void ActivateTrigger()
    {
        _collider.isTrigger = true;
    }
    void DesactivateTrigger()
    {
        _collider.isTrigger = false;
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }
    private void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(Inventory.actualCharge > 0)
            {
                Charge.Invoke();
                Bullet bullet = BulletPool.Instance.GetBullet();
                Inventory.actualCharge--;
                _audioManager.PlaySFX(_audioManager.attack);
                if (bullet != null)
                {
                    _animator.SetBool("isAttacking", true);
                    StartCoroutine(DesactivateAttack());
                    bullet.transform.position = transform.position;
                    bullet.transform.rotation = transform.rotation;

                    bullet.gameObject.SetActive(true);

                    bullet.DirectionBullet();
                }
                else
                {
                    Debug.Log("El objeto de la pool no tiene el componente Bullet.");
                }
            }
            
        }
    }

    IEnumerator DesactivateAttack()
    {
        yield return new WaitForSeconds(1);
        _animator.SetBool("isAttacking", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Plataforma2")) _isGroundedDown = true;
        else if (collision.gameObject.CompareTag("Plataforma")) _isGroundedDown = false;
        if (collision.gameObject.CompareTag("GameOver"))
        {
            StartCoroutine(WaitForDeath());
            _uiManager.SetGameOver();
            UpdateArchivements();
            Inventory.SaveCoins();
            Inventory.actualArmor = Inventory.inventory.armor;
            gameOver = true;
        }
        if ((collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyMole")) && !_isInvicible)
        {
            Debug.Log("HAOSJHASDJASOF1");
            if (Inventory.actualArmor.Equals(0))
            {
                StartCoroutine(WaitForDeath());
                _uiManager.SetGameOver();
                UpdateArchivements();
                Inventory.SaveCoins();
                Inventory.actualArmor = Inventory.inventory.armor;
                gameOver = true;

            }
            else
            {
                Inventory.actualArmor--;
                StartCoroutine(BecomeTemporarilyInvincible(_isInvicible));
                Destroy(collision.gameObject);
            }

        }
        if (collision.gameObject.CompareTag("Coin"))
        {
            _audioManager.PlaySFX(_audioManager.collectCoin);
            Inventory.inventory.coins++;
            HudManager.instance.UpdateCoins(Inventory.inventory.coins);
            Destroy(collision.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Plataforma2")) _isGroundedDown = true;
        else if (collision.gameObject.CompareTag("Plataforma")) _isGroundedDown = false;
        if (collision.gameObject.CompareTag("GameOver"))
        {
            StartCoroutine(WaitForDeath());
            _uiManager.SetGameOver();
            UpdateArchivements();
            Inventory.SaveCoins();
            Inventory.actualArmor = Inventory.inventory.armor;
            gameOver = true;
        }
        if ((collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyMole")) && !_isInvicible)
        {
            if (Inventory.actualArmor.Equals(0))
            {
                StartCoroutine(WaitForDeath());
                _uiManager.SetGameOver();
                UpdateArchivements();
                Inventory.SaveCoins();
                Inventory.actualArmor = Inventory.inventory.armor;
                //_audioManager.StopMusic();
                gameOver = true;

            }
            else
            {
                Inventory.actualArmor--;
                StartCoroutine(BecomeTemporarilyInvincible(_isInvicible));
                Destroy(collision.gameObject);
            }
            OnHit.Invoke();
        }
    }

    
    IEnumerator WaitForDeath()
    {
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 0;
        _audioManager.StartGameOverTheme();
    }

    public IEnumerator BecomeTemporarilyInvincible(bool _isInvincible)
    {
        Debug.Log("Player turned invincible!");
        _isInvincible = true;
        this.gameObject.layer = LayerMask.NameToLayer("Invincible");
            Debug.Log(gameObject.layer);

        yield return new WaitForSeconds(invincibilityDurationSeconds);

        _isInvincible = false;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        Debug.Log(gameObject.layer);
        Debug.Log("Player is no longer invincible!");
    }
    public void UpdateArchivements()
    {
        PlayerPrefs.SetInt("avanzar", PlayerPrefs.GetInt("avanzar") + (HudManager.scoreCounter/1000));
        PlayerPrefs.SetInt("matar", PlayerPrefs.GetInt("matar") + GameManager.enemysDefeated);
        PlayerPrefs.SetInt("monedas", PlayerPrefs.GetInt("monedas") + Inventory.inventory.coins);
        PlayerPrefs.SetInt("muerte", PlayerPrefs.GetInt("muerte") + 1);

    }
}
