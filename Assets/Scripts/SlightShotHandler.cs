using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlightShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] LineRenderer _leftLineRenderer;
    [SerializeField]private LineRenderer _rightLineRenderer;

    [Header("Transform References")]
    [SerializeField]private Transform _leftStartPosition;
    [SerializeField]private Transform _rightStartPosition;
    [SerializeField]private Transform _centerPosition;
    [SerializeField]private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;
    
    [Header("SlingShot Stats")]
    [SerializeField] private float _maxDragDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _shotDuration = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private BirdQueue _birdQueue;
    [SerializeField] private CameraManager _cameraManager;
    
    
    
    [Header("Bird")]
    [SerializeField] private AngieBird _angieBirdPrefab;
    [SerializeField] private float _angieBirdPositionOffset = 0.2f;

    [Header("Sounds")] 
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip[] _elasticReleasedClips;
        
    private AngieBird Angie;
    
    private bool _isDragging;
    private bool _birdOnSlingShot;
    
    private AudioSource _audioSource;
    
    private Vector2 _slingShotLinesPosition;
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;
        
        SpawnAngieBird();
        
        
    }

    private void Start()
    {
        _birdQueue.BirdSpawn(GameManager.instance.MaxNumberOfShots);
    }
    private void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingShotArea())
        {
            _isDragging = true;

            if (_birdOnSlingShot)
            {
                SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
                _cameraManager.SwitchToFollowCam(Angie.transform);
            }
        }

        if (InputManager.IsLeftMousePressed && _isDragging && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }
        
        if(InputManager.WasLeftMouseButtonReleased && _isDragging)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                _isDragging = false;
                
                Angie.LaunchBird(_direction, _shotForce);
                
                SoundManager.instance.PlayRandomClip(_elasticReleasedClips, _audioSource);
                
                GameManager.instance.UseShot();
                
                _birdOnSlingShot = false;
                
                //SetLines(_centerPosition.position); DoTween으로 교체
                AnimateSlingShot();

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngieBirdAfterTime());
                }
            }
        }
    }
    
    #region SlingShot Methods

    private void DrawSlingShot()
    {
        
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);
        
        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDragDistance);
        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }
        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);
        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }
    
    #endregion
    
    #region Angie Bird Methods

    private void SpawnAngieBird()
    {
        SetLines(_idlePosition.position);
        
        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angieBirdPositionOffset;
        
        Angie = Instantiate(_angieBirdPrefab, spawnPosition, Quaternion.identity);
        Angie.transform.right = dir;
        
        _birdOnSlingShot = true;
    }

    private void PositionAndRotateAngieBird()
    {
        Angie.transform.position = _slingShotLinesPosition + _directionNormalized * _angieBirdPositionOffset;
        Angie.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(_shotDuration);
        
        SpawnAngieBird();
        _birdQueue.BirdDestroy();
        
        _cameraManager.SwitchToIdleCam();
    }
    
    #endregion
    
    #region Animate SlingShot

    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);
        
        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);
            
            yield return null;
        }
    }
    
    #endregion
}
