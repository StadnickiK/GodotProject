using Godot;
using System;


public interface IDamageCalculator
{
    void CalculateDamage(IDamagable source, IDamagable to);
}

public partial class DamageCalculator : Node, IDamageCalculator
{
    [Export]
    public bool Logging { get; set; } = true;
    GameLogger gameLogger = GameLogger.Instance;

    public void CalculateDamage(IDamagable source, IDamagable to)
    {
        var attack = source.StatManager.GetStat(GlobalStatNames.Attack);
        var unitDefence = to.StatManager.GetStat(GlobalStatNames.Defence);
        var unitHP = to.StatManager.GetStat(GlobalStatNames.Health);
        var unitShield = to.StatManager.GetStat(GlobalStatNames.Shield);
        if (Logging) gameLogger.LogInfo("Attack " + attack.CurrentValue + "");
        if (Logging) gameLogger.LogInfo("Target Defence " + unitDefence.CurrentValue + " Target HP " + unitHP.CurrentValue);
        if (unitShield == null)
        {
            DamageToHealth(unitHP, attack, unitDefence);
        }
        else
        {
            if (unitShield.CurrentValue <= 0)
            {
                DamageToHealth(unitHP, attack, unitDefence);
            }
            else
            {
                DamageToHealth(unitShield, attack, unitDefence);
            }
        }
            
    }

    void DamageToHealth(IStat unitHP, IStat attack, IStat unitDefence) {
        if (attack.CurrentValue > unitDefence.CurrentValue)
            {
                var EffectiveAttack = attack.CurrentValue - unitDefence.CurrentValue;
                unitHP.CurrentValue -= EffectiveAttack;
                if (Logging) gameLogger.LogInfo("Damage dealt " + EffectiveAttack + " Target HP " + unitHP.CurrentValue);
            }
            else
            {
                unitHP.CurrentValue -= 1; // if defence is higher than attack deal minimal dmg
                if (Logging) gameLogger.LogInfo("Damage dealt " + 1 + " Target HP " + unitHP.CurrentValue);
            }
    }
}
