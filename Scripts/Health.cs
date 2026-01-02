using UnityEngine;
using Sirenix.OdinInspector;    // namespace for all Odin stuff
using UnityEngine.Events;
using System;

public class Health : MonoBehaviour, IDamageable
{
    // fields
    [BoxGroup("Stats"), SerializeField] private float _current = 100f;
    [BoxGroup("Stats"), SerializeField] private float _max = 100f;

    // death
    [BoxGroup("Death"), SerializeField] private string _deathLayer = "Corpse";

    // properties
    [BoxGroup("Debug"), ShowInInspector] public float CurrentHealth => _current;
    [BoxGroup("Debug"), ShowInInspector] public float CurrentPercentage => _current / _max;
    [BoxGroup("Debug"), ShowInInspector] public float MissingHealth => _max - _current;
    [BoxGroup("Debug"), ShowInInspector] public bool IsAlive => _current >= 1f;

    public UnityEvent<DamageInfo> OnDamage;
    public UnityEvent<DamageInfo> OnDeath;                    // UnityEvent is Unity specific, will appear in inspector
    public event EventHandler<DamageInfo> OnDamageCSharp;     // EventHandler is C# specific, won't appear in inspector
                                                              // 'event' allows restricted access to the event, we can add/remove our listeners, but we can't null the event itself 

    public void Damage(DamageInfo damageInfo)
    {
        if (!IsAlive) return;                       // stop if already dead
        if (damageInfo.Amount < 1f) return;         // ignore bad value

        // reduce health current value
        _current -= damageInfo.Amount;
        _current = Mathf.Clamp(_current, 0f, _max);

        // invoke the damage event
        // listening scripts will receive the event
        OnDamage.Invoke(damageInfo);    // UnityEvents check for null internally, we don't need ?.
        OnDamageCSharp?.Invoke(this, damageInfo);       // C# events DO NOT check for null, we need to use ?.
                                                        // C# events require a 'sender' parameter, typically using 'this'
                                                        // handle death
        if (!IsAlive)
        {
            OnDeath.Invoke(damageInfo);
            gameObject.layer = LayerMask.NameToLayer(_deathLayer);
        }
    }

    [Button("Damage Test 10%")]
    public void DamageTest()
    {
        float amount = _max * 0.1f;
        DamageInfo damageInfo = new DamageInfo(amount, DamageType.True, false, gameObject, gameObject, gameObject);
        Damage(damageInfo);
    }
}