using System.Collections;
using UnityEngine;

public class RTSCombat : MonoBehaviour
{
    [SerializeField] RTSUnitHealthBar healthBar;
    [SerializeField] float attackCooldown = 3f;
    [SerializeField] float attackDurationFreezeTime = 1.5f;
    [SerializeField] GameObject rangeProjectilePrefab;

    [Tooltip("For attack and aggro detection\nsame faction wont attack each other")]
    public int factionID;

    public int MaxHP = 100;
    public bool IsAlive => CurrentHP > 0;
    public int CurrentHP { get; private set; }
    public int Attack;
    public int Defense;
    
    private float cooldownElapsed = 0f;
    public bool AttackIsOnCooldown
    {
        get
        {
            return cooldownElapsed > 0;
        }
    }

    private float freezeTimeIsElapsed = 0f;
    public bool FreezeTimeIsUp
    {
        get
        {
            return freezeTimeIsElapsed > 0;
        }
    }

    private void Start()
    {
        CurrentHP = MaxHP;
    }

    private void LateUpdate()
    {
        bool isVisible = CurrentHP < MaxHP;
        healthBar.gameObject.SetActive(isVisible);
    }

    private IEnumerator StartCooldown()
    {
        float elapsed = 0f;
        cooldownElapsed = 0;
        while (elapsed < attackCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownElapsed = elapsed;
            yield return AttackIsOnCooldown;
        }
        cooldownElapsed = 0;
    }

    private IEnumerator StartFreezeTime()
    {
        float elapsed = 0f;
        freezeTimeIsElapsed = 0f;
        while (elapsed < attackDurationFreezeTime)
        {
            elapsed += Time.deltaTime;
            freezeTimeIsElapsed = elapsed;
            yield return freezeTimeIsElapsed;
        }

        freezeTimeIsElapsed = 0f;
    }

    public void PeformAttack(GameObject target)
    {
        if (!AttackIsOnCooldown) {
            StartCoroutine(StartFreezeTime());
            StartCoroutine(StartCooldown());
            // awsome animations starts 
            Debug.LogWarning($"Attack starts to {target.name}");

            if (rangeProjectilePrefab != null) {
                // do range attack
                Vector3 force = transform.forward * 1200f;
                GameObject go = Instantiate(rangeProjectilePrefab, transform.position, Quaternion.identity);
                go.GetComponent<RTSProjectile>().ShootProjectile(this, force, Attack);
            }
            else {
                // do melee attack
                target.GetComponent<RTSCombat>().HandleAttack(Attack, gameObject);
            }
        }

    }

    public void HandleAttack(int attackValue, GameObject attacker)
    {
        int damage = attackValue - Defense;

        if (damage > 0)
            CurrentHP -= damage;

        healthBar.SetHPPercentage((1f / MaxHP) * CurrentHP);

        if (CurrentHP <= 0) {
            // handle loot exp etc
            Destroy(gameObject);
        }

        RTSAIUnitControll ai;
        gameObject.TryGetComponent(out ai);
        if (ai != null) {
            // is enemy
            ai.HandleAttack(attacker, damage);
        }
    }
}
