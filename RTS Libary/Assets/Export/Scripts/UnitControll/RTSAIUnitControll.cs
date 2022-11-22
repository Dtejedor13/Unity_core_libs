using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(RTSUnitController))]
public class RTSAIUnitControll : MonoBehaviour
{
    private List<RTSEnemyTarget> targets { get; set; } = new List<RTSEnemyTarget>();
    private RTSEnemyTarget? currentTarget
    {
        get
        {
            if (targets.Count == 0) return null;
            targets.OrderByDescending(x => x.AggroValue);
            RTSEnemyTarget first = targets.First();
            if (first.gameObject == null)
            {
                targets.RemoveAt(0);
                OnTargetDestroyed();
                return null;
            }
            return first;
        }
    }

    private NavMeshAgent navAgent;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // call this after RTSUnitController Update method
    private void LateUpdate()
    {
        CheckAggroRangeAndAddTargets();

        if (!GetComponent<RTSCombat>().FreezeTimeIsUp)
        {
            HandleAggroAndRangeZone();
        }
    }

    public void HandleAttack(GameObject attacker, int damageDealed)
    {
        if (!GameObjectIsInTargetList(attacker)) {
            targets.Add(new RTSEnemyTarget
            {
                gameObject = attacker,
                AggroValue = CalcProducedAggro(damageDealed)
            });
        } 
        else {
            RTSEnemyTarget target = targets.FirstOrDefault(x => x.gameObject == attacker);
            target.AggroValue += CalcProducedAggro(damageDealed);
        }
    }

    private void CheckAggroRangeAndAddTargets()
    {
        foreach (var collider in GetComponent<RTSUnitController>().inAggroRange)
        {
            if (collider != null && targets.Where(x => x.gameObject == collider.gameObject).Count() == 0)
            {
                targets.Add(new RTSEnemyTarget
                {
                    gameObject = collider.gameObject,
                    AggroValue = 0
                });
            }
        }
    }

    private void HandleAggroAndRangeZone()
    {
        navAgent.isStopped = true;

        if (currentTarget.HasValue)
        {
            Collider inRange = GetComponent<RTSUnitController>().inAttackRange.FirstOrDefault(x => x.gameObject == currentTarget.Value.gameObject);
            if (inRange != null)
            {
                if (GetComponent<RTSCombat>().AttackIsOnCooldown == false && GetComponent<RTSUnitController>().CurrentMovmentSpeed <= 0.1f)
                {
                    // when a target exixts and attack cooldown is off and target is in attack range
                    GetComponent<RTSCombat>().PeformAttack(currentTarget.Value.gameObject);
                    // if not in swing
                    transform.LookAt(currentTarget.Value.gameObject.transform);
                }
            }
            else if (inRange == null)
            {
                // when target exists but not in attack range
                navAgent.isStopped = false;
                navAgent.destination = currentTarget.Value.gameObject.transform.position;
            }
        }
        else
        {
            // get component to patrol or stay at position
        }
    }

    private void OnTargetDestroyed()
    {
        Debug.Log("Target destroyed");
    }

    private bool GameObjectIsInTargetList(GameObject go)
    {
        foreach(RTSEnemyTarget target in targets) {
            if (target.gameObject == go)
                return true;
        }

        return false;
    }

    private int CalcProducedAggro(int damageDealed)
    {
        return damageDealed * 2;
    }
}
