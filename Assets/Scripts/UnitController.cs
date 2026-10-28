using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Unit UnitData { get; private set; }

    private Animator animator;
    private UnitVisual unitVisual;
    private bool hasHitTrigger;
    private bool hasAttackTrigger;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");

    void Awake()
    {
        animator = GetComponent<Animator>();
        unitVisual = GetComponent<UnitVisual>();

        if (animator != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.nameHash == HitHash && param.type == AnimatorControllerParameterType.Trigger)
                    hasHitTrigger = true;

                if (param.nameHash == AttackHash && param.type == AnimatorControllerParameterType.Trigger)
                    hasAttackTrigger = true;
            }
        }
    }

    public void Setup(Unit unitData)
    {
        UnitData = unitData;
        gameObject.name = UnitData.Name;
    }

    public void PlayAttackAnimation()
    {
        if (animator != null && hasAttackTrigger)
            animator.SetTrigger(AttackHash);
    }

    public void PlayHitReaction()
    {
        if (unitVisual != null) unitVisual.PlayHitEffect();

        if (animator != null && hasHitTrigger)
            animator.SetTrigger(HitHash);
    }

    public void PlayDeathReaction()
    {
        if (unitVisual != null) unitVisual.PlayDeathEffect();

        if (animator != null) animator.SetTrigger(DieHash);
    }
}