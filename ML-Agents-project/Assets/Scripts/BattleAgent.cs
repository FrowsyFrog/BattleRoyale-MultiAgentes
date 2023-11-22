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
    [SerializeField] GameManager _bombManager;
    [SerializeField] GameObject _agentModel;
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

    public override void OnEpisodeBegin()
    {
        _agentModel.SetActive(true);
        transform.position = _bombManager.GetRandomSpawnPos();
        _availableBombs = 0;
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

        if (actions.DiscreteActions[2] == 1 && _availableBombs >= 1)
        {
            _availableBombs--;
            _bombManager.PutBomb(this);
            // More reward if place bomb near to other agent
            float bombPlacementReward = _bombManager.PutBombReward(DistanceToNearestAgent());
            AddReward(bombPlacementReward);
        }

        if (!_nearToBomb)
        {
            if (_bombManager.InsideFutureExplosion(transform.position, false))
            {
                _nearToBomb = true;
            }
        }
        else
        {
            AddReward(_bombManager.InsideFutureExplosionPenalty);
            if (!_bombManager.InsideFutureExplosion(transform.position, true))
            {
                _nearToBomb = false;
                AddReward(_bombManager.OutFutureExplosionReward);
            }
        }
    }

    float DistanceToNearestAgent()
    {
        float minDistance = float.MaxValue;
        foreach (BattleAgent agent in _bombManager.Agents)
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

        DiscreteActions[2] = Input.GetKey(KeyCode.E) ? 1: 0;
    }

    public void WinGame()
    {
        AddReward(2.5f);
        EndEpisode();
    }

    public void LoseGame()
    {
        if (!_isDead) {
            AddReward(_bombManager.LosePenalty);
            _isDead = true;
        }
        EndEpisode();
    }

    public void KillPlayer()
    {
        AddReward(_bombManager.KillReward);
        EndEpisode(); // QUITAR EN JUEGO
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        // LOSE
        if (other.CompareTag("Explosion"))
        {
            BattleAgent bombOwner = other.GetComponentInParent<Bomb>().BombOwner;
            if(bombOwner != this) bombOwner.KillPlayer(); // GIVE REWARD TO AGENT THAT KILLS YOU
            PlayerDies();
        }
        else if (other.CompareTag("Ammo"))
        {
            AddReward(_bombManager.GrabAmmoReward); // REWARD GRAB BULLET
            _availableBombs++;
            _bombManager.GrabAmmo(other.transform);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (_isDead) return;

        if (other.collider.CompareTag("Walls"))
        {
            AddReward(_bombManager.OnWallPenalty);
        }
    }

    void PlayerDies()
    {
        _isDead = true;
        AddReward(_bombManager.LosePenalty);
        _agentModel.SetActive(false);
        _bombManager.PlayerDead(this);
        EndEpisode(); // QUITAR LUEGOOO...
    }
}
