using UnityEngine;
using System;

[RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class ShieldUpgrade : BaseUpgrade, IDamageable {

    [SerializeField, Range(0, 10)]
    private int hitsPerTier = 2;
    [SerializeField, Range(1.0f, 10.0f)]
    private float radius = 1.0f;

    private int remainingHits = 0;
    private CircleCollider2D shieldCollider = null;
    private new SpriteRenderer renderer = null;

    public float Health { get { return remainingHits; } }

    protected override void Start()
    {
        base.Start();
        transform.localScale = Vector3.one * radius;
        if(!gameObject.TryGetComponent(out shieldCollider))
            Debug.LogError("No CircleCollider2D attached!");

        if(!gameObject.TryGetComponent(out renderer))
            Debug.LogError("No Spriterenderer attached to gameObject!");

        UpdateShield();
    }

    protected override void OnDestroy()
    {
        controller.OnAttackEnded -= OnAttackEnded;
    }

    protected override void OnPreparationEnded(object sender, EventArgs e)
    {
        base.OnPreparationEnded(sender, e);

        UpdateShield();
    }

    public void AddDamage(IEntity damager, float damage)
    {
        remainingHits = Mathf.Clamp(remainingHits - 1, 0, hitsPerTier * Tier);
        if(remainingHits <= 0)
        {
            Die(damager, DethCause.Exploded);
        }
    }

    public void Die(IEntity killer, DethCause cause)
    {
        shieldCollider.enabled = false;
        renderer.enabled = false;
    }

    private void UpdateShield()
    {
        if (playerBase.IsIntact && upgradeTier > 0)
        {
            remainingHits = hitsPerTier * Tier;
            shieldCollider.enabled = renderer.enabled = true;
        }
        else
        {
            shieldCollider.enabled = renderer.enabled = false;
        }
    }
}
