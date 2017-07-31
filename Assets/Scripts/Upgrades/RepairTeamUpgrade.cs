using UnityEngine;
using System;

public class RepairTeamUpgrade : BaseUpgrade {

    [SerializeField, Range(1.0f, 20.0f)]
    private float repairPerTier = 10.0f;

    protected override void OnAttackEnded(object sender, EventArgs e)
    {
        if(upgradeTier > 0 && playerBase.IsIntact && controller.CurrentPower + powerProduction > 0)
        {
            playerBase.AddDamage(null, -(repairPerTier * upgradeTier));
            controller.AddPower(powerProduction);
        }
    }
}
