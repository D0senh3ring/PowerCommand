using UnityEngine;
using System;

public abstract class BaseUpgrade : MonoBehaviour {

    [SerializeField]
    protected PlayerBase playerBase = null;
    [SerializeField, Range(-100.0f, 100.0f)]
    protected float powerProduction = 0.0f;
    [SerializeField, Range(0, 5)]
    protected int upgradeTier = 0;
    [SerializeField, Range(-100.0f, 100.0f)]
    protected float powerProductionPerTier = 0.0f;
    [SerializeField, Range(0.0f, 100.0f)]
    protected float upgradeCost = 5.0f;

    protected GameController controller = null;

    /// <summary>
    /// Returns the current Tier of this <see cref="BaseUpgrade"/>
    /// </summary>
    public virtual int Tier { get { return upgradeTier; } }
    /// <summary>
    /// Returns the amount of power which is required to upgrade this <see cref="BaseUpgrade"/>
    /// </summary>
    public virtual float UpgradeCost { get { return upgradeCost; } }

    /// <summary>
    /// Upgrades the <see cref="BaseUpgrade"/> by one tier
    /// </summary>
    public virtual void Upgrade()
    {
        if (upgradeTier < 5)
        {
            upgradeTier++;
            powerProduction += powerProductionPerTier;

            playerBase.UpdatePowerProduction(powerProduction);
            controller.AddPower(-upgradeCost);
        }
    }

    protected virtual void Start ()
    {
        playerBase.UpdatePowerProduction(powerProduction);

        GameObject controllerObj = GameObject.FindWithTag("GameController");
        if(controllerObj == null || !controllerObj.TryGetComponent(out controller))
        {
            Debug.LogError("No GameController in scene!");
        }

        controller.OnAttackEnded += OnAttackEnded;
        controller.OnPreparationEnded += OnPreparationEnded;
    }

    protected virtual void Update () { }

    protected virtual void OnDestroy() { }

    protected virtual void OnAttackEnded(object sender, EventArgs e)
    {
        if(playerBase.IsIntact && powerProduction >= 0.0f)
        {
            controller.AddPower(powerProduction);
        }
    }

    protected virtual void OnPreparationEnded(object sender, EventArgs e)
    {
        if (playerBase.IsIntact && powerProduction < 0.0f)
        {
            controller.AddPower(powerProduction);
        }
    }
}
