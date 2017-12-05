using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;


public interface IDamageable
{
    ReactiveProperty<bool> UnderAttack { get; set; }
    void TakeDamage(int damage);
}

