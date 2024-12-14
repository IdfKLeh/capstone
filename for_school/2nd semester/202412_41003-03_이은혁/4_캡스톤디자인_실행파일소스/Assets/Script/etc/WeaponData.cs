using System.Collections;
using System.Collections.Generic;
using EventStageEventNameSpace;
using UnityEngine;
using UnityEngine.UI;

public class WeaponData
{
    public string weaponID { get; set; }
    public string weaponType { get; set; }
    public string weaponName { get; set; }
    public int hitCount { get; set; }//해당 무기의 소유자가 자신의 턴때 한번에 몇번 공격하는지
    public float phyStatRate { get; set; }
    public List<WeaStatRate> weaStatRate { get; set; }
    public List<WeaponAction> weaponAction { get; set; }
    public List<WeaponDescription> weaponDescription { get; set; }
}

public class WeaStatRate
{
    public string calc { get; set; }
    public float rate { get; set; }
}

public class WeaponAction
{

}

public class WeaponDescription
{
    public List<Restriction> restriction {get; set;}
    public string text { get; set; }
}

public class WeaponDataWrapper
{
    public List<WeaponData> weapon { get; set; }
}