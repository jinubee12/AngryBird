using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f;

    [SerializeField] private GameObject _restartScreen;

    private int _usedNumberOfShots;
    
    private List<Baddie> _baddies = new List<Baddie>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        
        Baddie[] baddies = FindObjectsOfType<Baddie>(); // Baddie 전부를 찾아 복사
        for (var i = 0; i < baddies.Length; i++)
        {
            _baddies.Add(baddies[i]); //리스트에 추가
        }
    }
    public void UseShot()
    {
        _usedNumberOfShots++;
        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        if (_usedNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForLastShot()
    {
        if (_usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);

        if (_baddies.Count == 0) // baddies all dead
        {
            WinGame();
        }
        else
        {
            RestartGame();
        }
        
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        CheckForAllDeadBaddies();
    }

    private void CheckForAllDeadBaddies()
    {
        if (_baddies.Count == 0)
        {
            WinGame();
        }
    }
    
    #region Win/Lose

    private void WinGame()
    {
        _restartScreen.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    #endregion
}
