using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BaseMissile : AudioObject, IEntity {

    public event EventHandler<MissileEventArgs> OnExploded;

    [SerializeField]
    protected GameObject explosionPrefab = null;
    [SerializeField, Range(0.1f, 10.0f)]
    protected float maxMissileSpeed = 1.0f;
    [SerializeField, Range(1.0f, 100.0f)]
    protected float explosionDamage = 50.0f;
    [SerializeField]
    protected LayerMask collisionLayers;

    private float smoothTime = 0.5f;
    protected Vector3 currentVelocity = Vector3.zero;
    protected GameObject target = null;

    /// <summary>
    /// Gets or sets the current target-<see cref="GameObject"/>
    /// </summary>
    public virtual GameObject Target
    {
        get { return target; }
        set
        {
            if (target == null && value != null)
                target = value;
        }
    }

    public virtual void Die(IEntity killer, DethCause cause)
    {
        //TODO: Explosion
        if (OnExploded != null)
            OnExploded.Invoke(this, new MissileEventArgs(this));

        Destroy(Instantiate(explosionPrefab, transform.position, transform.rotation), 10.0f);

        Destroy(gameObject);
    }

    protected virtual void Awake() { }

    protected override void Start()
    {
        base.Start();

        if (target != null)
        {
            Vector3 vectorToTarget = target.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg + 180;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    protected virtual void Update()
    {
        if (Target != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + (target.transform.position - transform.position) * 2, ref currentVelocity, smoothTime, maxMissileSpeed, Time.deltaTime);

            IDamageable damageable = null;

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.up.x, transform.up.y), 0.05f);
            if (hit)
            {
                if(hit.transform.gameObject.TryGetComponent(out damageable))
                    damageable.AddDamage(this, explosionDamage);
                Die(this, DethCause.Exploded);
            }
        }
    }
}

public class MissileEventArgs : EventArgs
{
    public MissileEventArgs(BaseMissile missile)
    {
        Missile = missile;
    }

    public BaseMissile Missile { get; private set; }
}