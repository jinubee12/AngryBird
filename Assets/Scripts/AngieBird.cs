using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AngieBird : MonoBehaviour
{
    [SerializeField] private AudioClip _hitClip;
    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;

    private bool _hasBeenLaunched; 
    private bool _shouldFaceVelocityDirection = true; //바닥 굴러가기 위한 트리거
    
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        
    }

    private void Start()
    {
        _rb.isKinematic = true;
        _circleCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_hasBeenLaunched && _shouldFaceVelocityDirection) 
        {
            transform.right = _rb.velocity; //날아가는 방향 바라보기
        }
    }
    public void LaunchBird(Vector2 direction, float force)
    {
        _rb.isKinematic = false;
        _circleCollider.enabled = true;
        
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
        
        _hasBeenLaunched = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _shouldFaceVelocityDirection = false; //부딪히면 데굴데굴
        SoundManager.instance.PlayClip(_hitClip, _audioSource);
        Destroy(this);
    }
}
