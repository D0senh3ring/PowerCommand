using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(ParticleSystem))]
public class PlayerBase : MonoBehaviour, IDamageable {

    public event EventHandler<BaseDestroyedEventArgs> OnBaseDestroyed;

    [SerializeField, Range(1.0f, 100.0f)]
    private float health;
    [SerializeField, Range(1.0f, 100.0f)]
    private float powerProduction = 5.0f;

    private new SpriteRenderer renderer = null;
    private new ParticleSystem particleSystem = null;
    private List<BaseUpgrade> upgrades = new List<BaseUpgrade>();

    /// <summary>
    /// Returns the <see cref="BaseUpgrade"/>s for this <see cref="PlayerBase"/>
    /// </summary>
    public List<BaseUpgrade> Upgrades { get { return upgrades; } }
    /// <summary>
    /// Returns the current health of this <see cref="PlayerBase"/>
    /// </summary>
    public float Health { get { return health; } }
    /// <summary>
    /// Returns the Power that is produced at the beginning of the preparation-phase
    /// </summary>
    public float PowerProduction { get { return powerProduction; } }
    /// <summary>
    /// Returns whether this <see cref="PlayerBase"/> is still intact
    /// </summary>
    public bool IsIntact { get { return health > 0; } }

    public void AddDamage(IEntity damager, float damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
        if(health == 0.0f)
        {
            Die(damager, DethCause.Exploded);
        }
    }

    /// <summary>
    /// Updates the power which is produced before each preparation-phase
    /// </summary>
    public void UpdatePowerProduction(float production)
    {
        powerProduction += production;
    }

    public void Die(IEntity killer, DethCause cause)
    {
        //TODO: Sprite-swap, explostion, clear upgrades
        //gameObject.SetActive(false);
        if (OnBaseDestroyed != null)
            OnBaseDestroyed.Invoke(this, new BaseDestroyedEventArgs(this));

        powerProduction = 0.0f;

        renderer.material.SetFloat("_SwapVal", 1);
        particleSystem.Play();
    }

    private void Start ()
    {
        if(!gameObject.TryGetComponent(out renderer))
        {
            Debug.LogError("No sprite-renderer attached!");
        }

        if(!gameObject.TryGetComponent(out particleSystem))
        {
            Debug.LogError("No ParticleSystem attached to gameObject!");
        }

        upgrades = gameObject.GetComponentsInChildren(typeof(BaseUpgrade)).Select(_comp => _comp.GetComponent<BaseUpgrade>()).ToList();
    }
}

public class BaseDestroyedEventArgs : EventArgs
{
    public BaseDestroyedEventArgs(PlayerBase destoryedBase)
    {
        DestoryedBase = destoryedBase;
    }

    public PlayerBase DestoryedBase { get; private set; }
}