using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombState
{
    Disabled,
    Waiting,
    Exploding,
}

public class Bomb : MonoBehaviour
{
    [SerializeField] BombManager _manager;
    [SerializeField] private float _explodingTime;
    [SerializeField] GameObject _explosion;
    BattleAgent _bombOwner;
    BombState _state = BombState.Disabled;
    float _curExplodeTime;
    public BattleAgent BombOwner() {
        return _bombOwner; 
    }
    public float CurExplodeTime { get => _curExplodeTime; }
    public int State { get => (int)_state; }
    public Vector2 Position { get => new Vector2(transform.position.x, transform.position.z); }

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
        _state = BombState.Waiting;

        while (_curExplodeTime > 0)
        {
            _curExplodeTime -= Time.deltaTime;
            yield return null;
        }

        _state = BombState.Exploding;
        _curExplodeTime = 0;

        _explosion.SetActive(true);

        yield return new WaitForSeconds(_explodingTime);

        DisableBomb();

        _manager.BombExploded();
    }

    public void DisableBomb()
    {
        gameObject.SetActive(false);
        _explosion.SetActive(false);
        _state = BombState.Disabled;
    }
}
