using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class BattleText
{
    public AmountText amountText { get; set; }
    public WeaponText weaponText { get; set; }
}

public class AmountText
{
    public List<string> smallest { get; set; }
    public List<string> smaller { get; set; }
    public List<string> small { get; set; }
    public List<string> medium { get; set; }
    public List<string> large { get; set; }
    public List<string> larger { get; set; }
    public List<string> largest { get; set; }
}

public class WeaponText
{
    public List<string> broadSword {get; set;}
    public List<string> dagger {get; set;}
    public List<string> fist {get; set;}
}

public class BattleTextWrapper
{
    public BattleText battleText { get; set; }
}