using System.Collections;
using System.Collections.Generic;
using EventStageEventNameSpace;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyData
{
    public string enemyID {get; set;}
    public string enemyName {get; set;}
    public int homeLevel {get; set;}
    public List<int> visibleLevel {get; set;}
    public Stat stat {get; set;}
    public string weaponType {get; set;}
    public string weaponID {get; set;}
    public List<Skill> enemySkill {get; set;}
    public List<string> enemyTrait {get; set;}
    public List<Description> enemyDescription {get; set;}
    public List<Action> winAction {get; set;}
    public List<Action> loseAction {get; set;}
    
}

public class Stat
{
    public int phyStat {get; set;}
    public int intStat {get; set;}
    public int staStat {get; set;}
    public int medStat {get; set;}
    public int speStat {get; set;}
    public int weaStat {get; set;}
}

public class Description{
    public List<Restriction> restriction {get; set;}
    public string text {get; set;}
}

public class EnemyDataWrapper
{
    public List<EnemyData> enemy {get; set;}
}

public class Skill{
    public string skillName {get; set;}
    public float skillDamage {get; set;}
    public float skillRate {get; set;}
}