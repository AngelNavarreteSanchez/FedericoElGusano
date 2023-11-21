using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChatacterMovement : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    GameManager _gameManager;
    private BoxCollider2D _collider;
    private SpriteRenderer _sr;
    public float jumpForce = 10f;
    public float velocidad;
    private GameObject _platform;

    [SerializeField]private Transform _groundCheck;
    public LayerMask _groundLayer;

    void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        //_platform = GameObject.Find("Col*");
        _collider = gameObject.GetComponent<BoxCollider2D>();

    }
    // Update is called once per frame
    void Update()
    {
        MovimientoHorizontal();
        Salto();

    }
    void MovimientoHorizontal()
    {


        //_rb.velocity = new Vector2(velocidad, _rb.velocity.y);

    }
    void Salto()
    {
        //Salto
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
         
                _rb.velocity = new Vector3(0, 10, 0);

            //_rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            DesactivaCollider();
            Invoke(nameof(ActivateCollider), 0.5f); // Esto activar� el collider despu�s de 0.5 segundos
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DesactivaCollider();
            Invoke(nameof(ActivateCollider), 0.6f);
        }
    }
    void ActivateCollider()
    {
        _collider.enabled = true;
    }
    void DesactivaCollider()
    {
        _collider.enabled = false;
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }
}
