using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    void Die(IEntity killer, DethCause cause);
}

public interface IDamageable : IEntity
{
    float Health { get; }
    void AddDamage(IEntity damager, float damage);
}

public enum DethCause : byte
{
    Void = 0,
    Exploded = 1
}