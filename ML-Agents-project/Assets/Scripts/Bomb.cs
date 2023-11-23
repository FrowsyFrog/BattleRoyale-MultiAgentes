using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [SerializeField] private float _explodingTime;
    [SerializeField] GameObject _explosion;
    BattleAgent _bombOwner;
    float _curExplodeTime;
    public BattleAgent BombOwner { get => _bombOwner; }
    public float CurExplodeTime { get => _curExplodeTime; }
    public bool IsDisabled { get => !gameObject.activeSelf; }

    public void PlaceBomb(BattleAgent owner, float maxExplodeTime)
    {
        transform.position = owner.transform.position;
        gameObject.SetActive(true);
        _bombOwner = owner;
        _curExplodeTime = maxExplodeTime;
        StartCoroutine(IWaitToExplode());
    }

    IEnumerator IWaitToExplode()
    {
        while (_curExplodeTime > 0)
        {
            _curExplodeTime -= Time.deltaTime;
            yield return null;
        }

        _curExplodeTime = 0;

        _explosion.SetActive(true);
        yield return new WaitForSeconds(_explodingTime);

        DisableBomb();

        _gameManager.BombExploded();
    }

    public void DisableBomb()
    {
        gameObject.SetActive(false);
        _explosion.SetActive(false);
    }
}
