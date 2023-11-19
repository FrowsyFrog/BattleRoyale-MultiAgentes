using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BattleAgent : Agent
{
    [SerializeField] List<Bomb> _bombs;
    [SerializeField] float _moveSpeed = 1;
    [SerializeField] Material _winMat;
    [SerializeField] Material _loseMat;
    [SerializeField] MeshRenderer _floorMeshRenderer;
    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        foreach (Bomb bomb in _bombs)
        {
            sensor.AddObservation(bomb.Position);
            sensor.AddObservation(bomb.CurExplodeTime);
            sensor.AddObservation(bomb.State);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float MoveX = actions.ContinuousActions[0];
        float MoveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(MoveX, 0, MoveZ) * Time.deltaTime * _moveSpeed;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> ContinuousActions = actionsOut.ContinuousActions;
        ContinuousActions[0] = Input.GetAxisRaw("Horizontal");
        ContinuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void SurvivedBomb()
    {
        // WIN
        _floorMeshRenderer.material = _winMat;
        SetReward(1);
        EndEpisode();
    }


    private void OnTriggerEnter(Collider other)
    {
        // LOSE
        if (other.CompareTag("Explosion"))
        {
            _floorMeshRenderer.material = _loseMat;

            foreach (Bomb bomb in _bombs)
            {
                bomb.PlayerDied();
            }

            SetReward(0);
            EndEpisode();
        }


    }
}
