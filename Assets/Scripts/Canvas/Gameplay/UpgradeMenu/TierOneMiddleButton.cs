using UnityEngine;

public class TierOneMiddleButton : UpgradeMenuButton
{
    public override void AppliedEffect()
    {
        Player.Instance.GetComponentInChildren<BasicAbility>().UpdateMaxFuel(50);

        PlayerDataManager.Instance.PlayerDataRuntime.SetBaseMaxFuel(Player.Instance.GetComponentInChildren<BasicAbility>().MaxFuel);
        PlayerDataManager.Instance.SaveRunTimeDataToPlayerDataSlot();
    }

    public override void RemoveEffect()
    {
        Player.Instance.GetComponentInChildren<BasicAbility>().ResetMaxFuel();

        PlayerDataManager.Instance.PlayerDataRuntime.SetBaseMaxFuel(Player.Instance.GetComponentInChildren<BasicAbility>().MaxFuel);
        PlayerDataManager.Instance.SaveRunTimeDataToPlayerDataSlot();
    }
}