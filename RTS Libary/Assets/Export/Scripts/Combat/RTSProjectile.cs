using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class RTSProjectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 2f;
    private RTSCombat caster;
    private int attackPower;

    public void ShootProjectile(RTSCombat caster, Vector3 force, int attackPower)
    {
        this.caster = caster;
        this.attackPower = attackPower;
        GetComponent<Rigidbody>().AddForce(force);
        StartCoroutine(CalcLifeTime());
    }

    private IEnumerator CalcLifeTime()
    {
        float elasped = 0;
        while (elasped < lifeTime) {
            elasped += Time.deltaTime;
            yield return elasped;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        RTSCombat target;
        collider.TryGetComponent(out target);
        if (target != null && target.factionID != caster.factionID) {
            target.HandleAttack(attackPower, caster.gameObject);
            Destroy(gameObject);
        }
    }
}
