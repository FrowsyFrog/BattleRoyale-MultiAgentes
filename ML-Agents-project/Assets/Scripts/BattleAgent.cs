using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BattleAgent : Agent
{
    [SerializeField] bool _checkDebug = false;
    [SerializeField] int _availableBombs = 0;
    [SerializeField] float _moveSpeed = 1;
    [SerializeField] BombManager _bombManager;
    [SerializeField] GameObject _agentModel;
    Rigidbody _rb;
    bool _isDead = false;
    public bool IsDead { get => _agentModel.activeSelf; }
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
        _isDead = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if(_isDead) return;

        sensor.AddObservation(Position);
        sensor.AddObservation(_availableBombs);

        // TOTAL 4
        //sensor.AddObservation(Position); // 2
        //sensor.AddObservation(_availableBombs); // 1
        //sensor.AddObservation(_bombManager.TimeLeft); // 1
        //
        //// TOTAL 76
        //foreach(BattleAgent agent in _bombManager.Agents) // * 20 - 1 = 19
        //{
        //    if (agent == this) continue;
        //    sensor.AddObservation(agent.IsDead); // 1
        //    sensor.AddObservation(agent.Position); // 2
        //    sensor.AddObservation(agent.AvailableBombs); // 1
        //}
        //
        //// TOTAL 40
        //foreach (Bomb bomb in _bombManager.Bombs) // * 10
        //{
        //    sensor.AddObservation(bomb.Position); // 2
        //    sensor.AddObservation(bomb.CurExplodeTime); // 1
        //    sensor.AddObservation(bomb.State); // 1
        //}
        //
        //// TOTAL 30
        //foreach(Transform ammo in _bombManager.Ammos) // * 10
        //{
        //    sensor.AddObservation(ammo.gameObject.activeSelf); // 1
        //    sensor.AddObservation(new Vector2(ammo.position.x, ammo.position.z)); // 2
        //}
    }

    Vector3 _lastPosition;
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (_isDead) return;

        float MoveX = actions.DiscreteActions[0];
        float MoveZ = actions.DiscreteActions[1];

        Vector3 AddForce = new Vector3(0, 0, 0);
        switch (MoveX)
        {
            case 0: AddForce.x = 0; break;
            case 1: AddForce.x = -1; break;
            case 2: AddForce.x = 1; break;
        }

        switch (MoveZ)
        {
            case 0: AddForce.z = 0; break;
            case 1: AddForce.z = -1; break;
            case 2: AddForce.z = 1; break;
        }

        _rb.velocity = AddForce * _moveSpeed + new Vector3(0, _rb.velocity.y, 0);

        if (actions.DiscreteActions[2] == 1 && _availableBombs >= 1)
        {
            _availableBombs--;
            _bombManager.PutBomb(this);

            // More reward if place bomb near to other agent
            //float bombPlacementReward = 0.35f * (1.0f / (1.0f + DistanceToNearestAgent()));
            //AddReward(bombPlacementReward);
        }

        //if(Vector3.Distance(_lastPosition, transform.position) >= 1)
        //{
        //    _lastPosition = transform.position;
        //    AddReward(0.00025f);
        //}
        //
        //
        //float closestBombDistance = float.MaxValue;
        //bool thereIsBomb = false;
        //foreach (Bomb bomb in _bombManager.Bombs)
        //{
        //    if (!bomb.gameObject.activeSelf) continue;
        //    thereIsBomb = true;
        //    float distanceToBomb = Vector3.Distance(transform.position, bomb.transform.position);
        //    closestBombDistance = Mathf.Min(closestBombDistance, distanceToBomb);
        //}
        //
        //if (thereIsBomb)
        //{
        //    if (closestBombDistance < _bombManager.MinDistanceToBomb)
        //    {
        //        float proximityPenalty = -0.0088f;
        //        AddReward(proximityPenalty);
        //    }
        //    else if(closestBombDistance <= _bombManager.MaxDistanceToBomb)
        //    {
        //        // Reward for moving away from bombs
        //        float distanceReward = 0.0095f * (closestBombDistance / _bombManager.MaxDistanceToBomb);
        //        AddReward(distanceReward);
        //
        //    }
        //}
    }

    float DistanceToNearestAgent()
    {
        float minDistance = float.MaxValue;
        foreach (BattleAgent agent in _bombManager.Agents)
        {
            if (!agent.IsDead && agent != this)
            {
                float distance = Vector3.Distance(agent.transform.position, transform.position);
                minDistance = Mathf.Min(minDistance, distance);
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
            AddReward(-1f);
            _isDead = true;
        }
        EndEpisode();
    }

    public void KillPlayer()
    {
        if (_isDead) return;
        AddReward(0.75f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        // LOSE
        if (other.CompareTag("Explosion"))
        {
            other.GetComponentInParent<Bomb>().BombOwner().KillPlayer();
            PlayerDies();
        }
        else if (other.CompareTag("Ammo"))
        {
            if (_isDead) return;
            AddReward(0.119f); // REWARD GRAB BULLET
            _availableBombs++;
            _bombManager.GrabAmmo(other.transform);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (_isDead) return;

        if (other.collider.CompareTag("Walls"))
        {
            AddReward(-0.5f * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isDead) return;

        if (other.CompareTag("Walls"))
        {
            AddReward(0.00125f * Time.deltaTime);
        }
    }

    void PlayerDies()
    {
        _isDead = true;
        AddReward(-1);
        _agentModel.SetActive(false);
        _bombManager.PlayerDead(this);
    }
}
