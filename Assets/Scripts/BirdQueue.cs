using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdQueue : MonoBehaviour
{
    public GameObject _birdPrefab;
    public Transform[] birdSpawns;
    private GameObject[] _spawnedBirds;
    
    private int _lastIndex;

    public void BirdSpawn(int _numberOfBirds)
    {
        _spawnedBirds = new GameObject[_numberOfBirds];
        for (var i = 0; i < _numberOfBirds - 1; i++)
        {
            _spawnedBirds[i] = Instantiate(_birdPrefab, birdSpawns[i].position, Quaternion.identity);
        }
        
        _lastIndex = _numberOfBirds - 2;
    }

    public void BirdDestroy()
    {
        if (_lastIndex >= 0)
        {
            Destroy(_spawnedBirds[_lastIndex]);
            _lastIndex--;
        }
    }
}
