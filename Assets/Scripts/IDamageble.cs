using System;

public interface IDamageble {
    public event Action<float> OnDamaged;
    public void Damage(float damage);
}