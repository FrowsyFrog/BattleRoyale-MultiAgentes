using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public enum BombState
{
    Waiting,
    Exploding,
    Resting
}

public class Bomb : MonoBehaviour
{
    [SerializeField] private bool _checkerBomb = false;
    [SerializeField] private float _maxExplodeTime;
    [SerializeField] private float _explodingTime;
    [SerializeField] private float _timeDisabled;
    [SerializeField] GameObject _explosion;
    [SerializeField] BombManager _manager;
    BombState _state;
    MeshRenderer _meshRenderer;
    float _curExplodeTime;
    public float CurExplodeTime { get => _curExplodeTime; }
    public int State { get => (int)_state; }
    public Vector3 Position { get => transform.localPosition; }

    void Start()
    {
        _meshRenderer =GetComponent<MeshRenderer>();
        StartCoroutine(IWaitToExplode());
    }

    IEnumerator IWaitToExplode()
    {
        while (true)
        {
            _curExplodeTime = _maxExplodeTime;
            _state = BombState.Waiting;
            _meshRenderer.enabled = true;

            while (_curExplodeTime > 0)
            {
                _curExplodeTime -= Time.deltaTime;
                yield return null;
            }

            _state = BombState.Exploding;
            _curExplodeTime = 0;

            _explosion.SetActive(true);

            yield return new WaitForSeconds(_explodingTime);

            _explosion.SetActive(false);

            _state = BombState.Resting;
            _meshRenderer.enabled = false;

            if(_checkerBomb) _manager.RoundEnded();

            transform.localPosition = _manager.GetRandomSpawnPos();
            yield return new WaitForSeconds(_timeDisabled);

        }

    }

    public void PlayerDied()
    {
        StopAllCoroutines();

        _explosion.SetActive(false);

        _state = BombState.Resting;
        _meshRenderer.enabled = false;
        transform.localPosition = _manager.GetRandomSpawnPos();

        StartCoroutine(IWaitToExplode());

    }
}
