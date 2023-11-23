using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleAgent : Agent
{
    [SerializeField] float _rotationSpeed;
    [SerializeField] bool _checkDebug = false;
    [SerializeField] int _availableBombs = 0;
    [SerializeField] float _moveSpeed = 1;
    [SerializeField] GameManager _gameManager;
    [SerializeField] private List<GameObject> _models;
    GameObject _agentModel;

    Rigidbody _rb;
    bool _isDead = false;
    bool _nearToBomb = false;
    public bool IsDead { get => !_agentModel.activeSelf; }
    public int AvailableBombs { get => _availableBombs; }
    public Vector2 Position { get => new Vector2(transform.position.x, transform.position.z); }

    public override void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_checkDebug) return;

        if (Input.GetKeyDown(KeyCode.E) && _availableBombs > 0) PutBomb();
    }

    public override void OnEpisodeBegin()
    {
        if(_agentModel) _agentModel.SetActive(false);
        _agentModel = _models[Random.Range(0, _models.Count)];
        _agentModel.SetActive(true);

        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = _gameManager.GetRandomSpawnPos();
        _availableBombs = 0;
        if (_checkDebug) _gameManager.SetAmmoText(_availableBombs);
        _nearToBomb = false;
        _isDead = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if(_isDead) return;

        sensor.AddObservation(Position);
        sensor.AddObservation(_availableBombs);
        sensor.AddObservation(_isDead);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (_isDead) return;

        float MoveX = actions.DiscreteActions[0];
        float MoveZ = actions.DiscreteActions[1];

        Vector3 MoveDirection = new Vector3(0, 0, 0);
        switch (MoveX)
        {
            case 0: MoveDirection.x = 0; break;
            case 1: MoveDirection.x = -1; break;
            case 2: MoveDirection.x = 1; break;
        }

        switch (MoveZ)
        {
            case 0: MoveDirection.z = 0; break;
            case 1: MoveDirection.z = -1; break;
            case 2: MoveDirection.z = 1; break;
        }

        _rb.velocity = MoveDirection * _moveSpeed + new Vector3(0, _rb.velocity.y, 0);

        if (MoveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(MoveDirection, Vector3.up);
            _agentModel.transform.rotation = Quaternion.RotateTowards(_agentModel.transform.rotation, toRotation, Time.deltaTime * _rotationSpeed);
        }

        if (actions.DiscreteActions[2] == 1 && _availableBombs >= 1) PutBomb();

        if (!_nearToBomb)
        {
            if (_gameManager.InsideFutureExplosion(transform.position, false))
            {
                _nearToBomb = true;
            }
        }
        else
        {
            AddReward(_gameManager.InsideFutureExplosionPenalty);
            if (!_gameManager.InsideFutureExplosion(transform.position, true))
            {
                _nearToBomb = false;
                AddReward(_gameManager.OutFutureExplosionReward);
            }
        }
    }

    void PutBomb()
    {
        _availableBombs--;
        if (_checkDebug) _gameManager.SetAmmoText(_availableBombs);

        _gameManager.PutBomb(this);
        // More reward if place bomb near to other agent
        float bombPlacementReward = _gameManager.PutBombReward(DistanceToNearestAgent());
        AddReward(bombPlacementReward);
    }

    float DistanceToNearestAgent()
    {
        float minDistance = float.MaxValue;
        foreach (BattleAgent agent in _gameManager.Agents)
        {
            if (!agent.IsDead && agent != this)
            {
                float distance = Vector3.Distance(agent.transform.localPosition, transform.localPosition);
                if(distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }
        return minDistance;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> DiscreteActions = actionsOut.DiscreteActions;

        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: DiscreteActions[0] = 1; break;
            case 0: DiscreteActions[0] = 0; break;
            case 1: DiscreteActions[0] = 2; break;
        }        
        
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: DiscreteActions[1] = 1; break;
            case 0: DiscreteActions[1] = 0; break;
            case 1: DiscreteActions[1] = 2; break;
        }

        //DiscreteActions[2] = Input.GetKey(KeyCode.E) ? 1: 0;
    }

    public void RestartGame()
    {
        EndEpisode();
    }

    public bool WinGame()
    {
        AddReward(_gameManager.WinReward);
        return _checkDebug;
    }

    public void LoseGame()
    {
        if (!_isDead) PlayerDies();
    }

    public void KillPlayer()
    {
        AddReward(_gameManager.KillReward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        // LOSE
        if (other.CompareTag("Explosion"))
        {
            BattleAgent bombOwner = other.GetComponentInParent<Bomb>().BombOwner;
            if(bombOwner != this) bombOwner.KillPlayer(); // GIVE REWARD TO AGENT THAT KILLS YOU // ONLY IF YOU DON'T KILL YOURSELF
            PlayerDies();
        }
        else if (other.CompareTag("Ammo"))
        {
            AddReward(_gameManager.GrabAmmoReward); // REWARD GRAB BULLET
            _availableBombs++;
            if (_checkDebug) _gameManager.SetAmmoText(_availableBombs);

            _gameManager.GrabAmmo(other.transform);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (_isDead) return;

        if (other.collider.CompareTag("Walls"))
        {
            AddReward(_gameManager.OnWallPenalty);
        }
    }

    void PlayerDies()
    {
        AddReward(_gameManager.LosePenalty);
        if (_checkDebug) _gameManager.SetAmmoText(-1);

        _isDead = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
        _agentModel.SetActive(false);
        _gameManager.PlayerDead(this);
    }
}
