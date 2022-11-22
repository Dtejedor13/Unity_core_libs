using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RTSCombat))]
public class RTSUnitController : MonoBehaviour
{
    public float AttackRange;
    public float AggroRange;

    public Transform IsSelectedMarker;
    public bool EnableMovement = false;

    public float CurrentMovmentSpeed { get; private set; }
    public List<Collider> inAggroRange { get; private set; }
    public List<Collider> inAttackRange { get; private set; }
    NavMeshAgent navAgennt;

    private RTSAIUnitControll ai;
    private Vector3 PreviousFramePosition = Vector3.zero;
    private GameObject declaredActionTarget;

    private void Start()
    {
        navAgennt = GetComponent<NavMeshAgent>();
        RTSUnitSelections.Instance.UnitList.Add(this.gameObject);
        TryGetComponent(out ai);
        if (IsSelectedMarker != null)
            IsSelectedMarker.gameObject.SetActive(false);
    }

    private void Update()
    {
        MesureMovementSpeed();
        DetectObjectsInRange();
        if (GetComponent<RTSCombat>().FreezeTimeIsUp) {
            // do nothing 
            return;
        }
        if (ai != null) 
            return;

        // check if target is destroyed
        if (declaredActionTarget != null && declaredActionTarget.transform == null)
            declaredActionTarget = null;

        HandleDeclaredAction();
        HandleAggroAndRangeZone();
    }

    public void SetMoveDestination(Vector3 point)
    {
        if (EnableMovement) {
            navAgennt.SetDestination(point);
            navAgennt.isStopped = false;
        }
    }

    public bool IsAttackable(GameObject go)
    {
        RTSCombat combatController;
        go.TryGetComponent<RTSCombat>(out combatController);
        if (combatController == null) return false;
        return combatController.factionID != GetComponent<RTSCombat>().factionID;
    }

    public void DeclareAction(GameObject target)
    {
        Debug.Log($"Declare action to {target.name}");
        declaredActionTarget = target;
        navAgennt.destination = target.transform.position;
    }

    private void HandleDeclaredAction()
    {
        if (declaredActionTarget != null) {
            navAgennt.isStopped = inAttackRange.Contains(declaredActionTarget.GetComponent<Collider>());
        }
    }

    private void DetectObjectsInRange()
    {
        inAggroRange = Physics.OverlapSphere(transform.position, AggroRange).Where(x => IsAttackable(x.gameObject)).ToList();
        inAttackRange = Physics.OverlapSphere(transform.position, AttackRange).Where(x => IsAttackable(x.gameObject)).ToList();
    }

    private void HandleAggroAndRangeZone()
    {
        if (inAttackRange.Count > 0 && CurrentMovmentSpeed <= 0.1f) {
            // enemy is detected and is in range
            GameObject target = declaredActionTarget != null ? declaredActionTarget : inAttackRange.First().gameObject;
            transform.LookAt(target.transform);
            GetComponent<RTSCombat>().PeformAttack(target);
        }
        else if (inAttackRange.Count == 0 && inAggroRange.Count > 0) {
            // enemy detected but not in range 
            transform.LookAt(inAggroRange.First().transform);
        }
        
    }

    private void OnDestroy()
    {
        RTSUnitSelections.Instance.UnitList.Remove(this.gameObject);
    }

    private void MesureMovementSpeed()
    {
        float movementPerFrame = Vector3.Distance(PreviousFramePosition, transform.position);
        CurrentMovmentSpeed = movementPerFrame / Time.deltaTime;
        PreviousFramePosition = transform.position;
    }
}