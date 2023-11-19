using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] List<BattleAgent> agents = new List<BattleAgent>();
    Dictionary<BattleAgent, bool> _surviedAgents = new Dictionary<BattleAgent, bool>();
    [SerializeField] Transform _minPos;
    [SerializeField] Transform _maxPos;

    private void Awake()
    {
        foreach(var agent in agents)
        {
            _surviedAgents.Add(agent, true);
        }
    }

    public void KillAgent(BattleAgent agent)
    {
        _surviedAgents[agent] = false;
    }

    public void RoundEnded()
    {
        foreach (var agent in agents)
        {
            if (_surviedAgents[agent])
                agent.SurvivedBomb();
            else _surviedAgents[agent] = true;
        }
    }

    public Vector3 GetRandomSpawnPos()
    {
        return new Vector3(
            Random.Range(_minPos.localPosition.x, _maxPos.localPosition.x),
            _minPos.localPosition.y,
            Random.Range(_minPos.localPosition.z, _maxPos.localPosition.z)
            );
    }
}
