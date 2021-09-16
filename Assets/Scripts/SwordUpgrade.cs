﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordUpgrade : Upgrade
{
    public override void ApplyUpgrade(Player upgrader, PlayerStats stats)
    {
        stats.baseDamage += GetDamage();
    }

    private int GetDamage() => Mathf.RoundToInt(10f * Mathf.Pow(1.07f, lvl));

    public override int GetUpgradeCost() => Mathf.RoundToInt(100f * Mathf.Pow(1.07f, lvl));
    public override UpgradeType GetUpgradeType() => UpgradeType.Sword;
    public override string GetDescription() => $"Add {GetDamage()} damage to your attack";

    public override string GetName() => "Sword";
    public override int UnlockCost() => 0;
}
