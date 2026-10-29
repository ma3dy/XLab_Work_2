using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Unit UnitData { get; private set; }
    public Transform WeaponHand;

    [Header("Weapon Offsets")]
    public Vector3 weaponPositionOffset = Vector3.zero;
    public Vector3 weaponRotationOffset = Vector3.zero;

    private Animator animator;
    private UnitVisual unitVisual;
    private GameObject currentWeaponVisual;
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
                if (param.nameHash == HitHash && param.type == AnimatorControllerParameterType.Trigger) hasHitTrigger = true;
                if (param.nameHash == AttackHash && param.type == AnimatorControllerParameterType.Trigger) hasAttackTrigger = true;
            }
        }
    }

    public void Setup(Unit unitData, GameObject weaponPrefab)
    {
        UnitData = unitData;
        gameObject.name = UnitData.Name;
        EquipWeaponVisual(weaponPrefab);
    }

    public void EquipWeaponVisual(GameObject weaponPrefab)
    {
        if (currentWeaponVisual != null) Destroy(currentWeaponVisual);

        if (weaponPrefab != null && WeaponHand != null)
        {
            currentWeaponVisual = Instantiate(weaponPrefab, WeaponHand);
            currentWeaponVisual.transform.localPosition = weaponPositionOffset;
            currentWeaponVisual.transform.localRotation = Quaternion.Euler(weaponRotationOffset);
        }
    }

    public GameObject DropWeaponVisual()
    {
        if (currentWeaponVisual == null) return null;

        GameObject dropped = currentWeaponVisual;
        dropped.transform.SetParent(null);
        currentWeaponVisual = null;
        return dropped;
    }

    public void SetCurrentWeaponVisual(GameObject visual)
    {
        if (currentWeaponVisual != null && currentWeaponVisual != visual) Destroy(currentWeaponVisual);
        currentWeaponVisual = visual;
    }

    public void PlayAttackAnimation()
    {
        if (animator != null && hasAttackTrigger) animator.SetTrigger(AttackHash);
    }

    public void PlayHitReaction()
    {
        if (unitVisual != null) unitVisual.PlayHitEffect();
        if (animator != null && hasHitTrigger) animator.SetTrigger(HitHash);
    }

    public void PlayDeathReaction()
    {
        if (unitVisual != null) unitVisual.PlayDeathEffect();
        if (animator != null) animator.SetTrigger(DieHash);
    }
}