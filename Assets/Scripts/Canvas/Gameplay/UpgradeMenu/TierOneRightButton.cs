using UnityEngine;

public class TierOneRightButton : UpgradeMenuButton
{
    public override void AppliedEffect()
    {
        Player.Instance.GetComponentInChildren<RangeAbility>().UpdateCurrentMaxCharge(1);
    }

    public override void RemoveEffect()
    {
        Player.Instance.GetComponentInChildren<RangeAbility>().UpdateCurrentMaxCharge(-1);
    }
}