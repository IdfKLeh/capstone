using System.Collections.Generic;

public class BattleLogData
{
    public string attacker { get; set; }
    public string typeOfAttacker { get; set; }
    public string amountOfDamage { get; set; }
    public string typeOfDamage { get; set; }
    public string weaponName { get; set; }
    public float damagePercentage { get; set; }
    
    // 쉽게 argument를 삽입해서 만들기 위한 constructor
    public BattleLogData(string attacker, string typeOfAttacker,string amountOfDamage, string typeOfDamage, string weaponName, float damagePercentage)
    {
        this.attacker = attacker;
        this.typeOfAttacker = typeOfAttacker;
        this.amountOfDamage = amountOfDamage;
        this.typeOfDamage = typeOfDamage;
        this.weaponName = weaponName;
        this.damagePercentage = damagePercentage;
    }
}