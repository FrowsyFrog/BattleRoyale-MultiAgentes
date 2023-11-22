using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] bool _restartWhenEnds = true;
    [SerializeField] float _minDistanceToBomb = 3.71f;
    [SerializeField] float _maxDistanceToBomb = 10.71f;

    [Header("UI")]
    [SerializeField] TMPro.TextMeshProUGUI _gameTimeTxt;
    [SerializeField] TMPro.TextMeshProUGUI _playersLeftTxt;

    [Header("Game Timers")]
    [SerializeField] float _maxTimeGame = 600;
    [SerializeField] private float _maxBombExplodeTime = 1.19f;


    [Header("Ammos spawner")]
    [SerializeField] Vector2 _timeToSpawnAmmo;
    [SerializeField] int _maxAmmosInGame;
    [SerializeField] Transform _minPos;
    [SerializeField] Transform _maxPos;

    [Header("Lists")]
    [SerializeField] List<BattleAgent> _agents;
    [SerializeField] List<Bomb> _bombs;
    [SerializeField] List<Transform> _ammos;

    float _timeToSpawn = 1;
    float _curTimeGame;
    int _maxAmmosSaved;
    bool _gameEnded = false;
    List<BattleAgent> _agentsAlive = new List<BattleAgent>();
    public float MinDistanceToBomb { get => _minDistanceToBomb; }
    public float MaxDistanceToBomb { get => _maxDistanceToBomb; }
    public List<BattleAgent> Agents { get => _agents; }
    public List<Bomb> Bombs { get=> _bombs; }
    public List<Transform> Ammos { get=> _ammos; }
    public float TimeLeft { get => _curTimeGame; }

    private void Awake()
    {
        _curTimeGame = _maxTimeGame;
        _maxAmmosSaved = _maxAmmosInGame;

        foreach(var agent in _agents)
        {
            _agentsAlive.Add(agent);
        }

        _gameTimeTxt.text = $"Tiempo restante: {(int)_curTimeGame}";
        _playersLeftTxt.text = $"Agentes vivos: {_agentsAlive.Count}";
    }

    void RestartGame()
    {
        Debug.Log("RESTART GAME");
        _gameEnded = false;
        _maxAmmosInGame = _maxAmmosSaved;
        _curTimeGame = _maxTimeGame;
        _agentsAlive.Clear();
        foreach (var agent in _agents)
        {
            _agentsAlive.Add(agent);
        }

        foreach (Bomb bomb in _bombs)
        {
            bomb.DisableBomb();
        }

        foreach (Transform ammo in _ammos)
        {
            ammo.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleTimer();
        HandleSpawnAmmo();
    }


    void HandleTimer()
    {
        if (_gameEnded) return;

        _curTimeGame -= Time.deltaTime;
        if (_curTimeGame <= 0)
        {
            foreach (BattleAgent agent in _agents)
            {
                agent.LoseGame();
            }
            _curTimeGame = 0;

            _gameEnded = true;

            if (_restartWhenEnds)
                RestartGame();
            else {
                enabled = false;
            }
        }

        _gameTimeTxt.text = $"Tiempo restante: {(int)_curTimeGame}";

    }

    void HandleSpawnAmmo()
    {

        if (_maxAmmosInGame <= 0) return;

        _timeToSpawn -= Time.deltaTime;
        if (_timeToSpawn <= 0)
        {
            _timeToSpawn = Random.Range(_timeToSpawnAmmo.x, _timeToSpawnAmmo.y);
            SpawnAmmo();
        }
    }

    void SpawnAmmo()
    {
        _maxAmmosInGame--;
        foreach (Transform ammo in _ammos)
        {
            if (!ammo.gameObject.activeSelf)
            {
                ammo.gameObject.SetActive(true);
                ammo.position = GetRandomSpawnPos();
                break;
            }
        }
    }

    public void PutBomb(BattleAgent owner)
    {
        foreach(Bomb bomb in _bombs)
        {
            if(bomb.State == 0) // PUT A DISABLED BOMB
            {
                bomb.PlaceBomb(owner, _maxBombExplodeTime);
                break;
            }
        }
    }

    public void BombExploded()
    {
        _maxAmmosInGame++;
    }

    public void PlayerDead(BattleAgent deadPlayer)
    {
        _maxAmmosInGame += deadPlayer.AvailableBombs;
        _agentsAlive.Remove(deadPlayer);
        _playersLeftTxt.text = $"Agentes vivos: {_agentsAlive.Count}";

        if (_agentsAlive.Count > 1) return;

        BattleAgent AliveAgent = null;
        if(_agentsAlive.Count == 1)
        {
            AliveAgent = _agentsAlive[0];
            AliveAgent.WinGame();
            Debug.Log("winner!!!");
        }

        foreach (BattleAgent agent in _agents)
        {
            if (agent == AliveAgent) continue;
            agent.LoseGame();
        }
        _gameEnded = true;
        if (_restartWhenEnds) RestartGame();
        else {
            enabled = false;
        }
    }

    public void GrabAmmo(Transform ammo)
    {
        ammo.gameObject.SetActive(false);
    }

    public Vector3 GetRandomSpawnPos()
    {
        return new Vector3(
            Random.Range(_minPos.position.x, _maxPos.position.x),
            _minPos.localPosition.y,
            Random.Range(_minPos.position.z, _maxPos.position.z)
            );
    }
}
