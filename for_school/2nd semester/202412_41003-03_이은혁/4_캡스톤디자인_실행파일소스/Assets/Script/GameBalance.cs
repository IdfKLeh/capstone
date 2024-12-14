using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameBalance//게임의 밸런스에 영향을 주는 수들의 스크립트
{
    public static int basicTrainingValue = 8;

    //main, sub 스탯과 관련된 값
    public static int mainStatAdditionalValue = 4;
    public static int subStatAdditionalValue = 2;
    public static int mainStatExtraValue = 100;
    public static int subStatExtraValue = 50;

    //최대 stat과 관련된 값
    public static int maxStaStat = 1000;
    public static int maxMedStat = 1000;
    public static int maxSpeStat = 1000;

    //함수에서 사용되는 middle 과 관련된 값
    public static int referenceStaStat = 300;
    public static int referenceMedStat = 300;
    public static int referenceSpeStat = 300;

    //Battle에서 winrate를 계산하기 위해 사용되는 값들
    //몇 퍼센트에서 락을 걸지 기준점
    public static float positiveFirstLockReference = 70f;
    public static float negativeFirstLockReference = 30f;
    public static float positiveSecondLockReference = 80f;
    public static float negativeSecondLockReference = 20f;
    public static float positiveThirdLockReference = 90f;
    public static float negativeThirdLockReference = 10f; 
    //락을 얼마나 걸지의 값
    public static float firstLockValue = 0.5f;
    public static float secondLockValue = 0.2f;
    public static float thirdLockValue = 0.1f;

    //BattleText 중 amountText의 기준점이 되는 값들.
    public static float smallestAmountTextReference = 3.0f;
    public static float smallerAmountTextReference = 7.0f;
    public static float smallAmountTextReference = 12.0f;
    public static float mediumAmountTextReference = 18.0f;
    public static float largeAmountTextReference = 30.0f;
    public static float largerAmountTextReference = 60.0f;

    //최대 체력 값과 관련된 값들
    public static float maxHealthMultiplier = 0.1f;

    //전투와 관련된 값들
    public static int basicBattleLossLosingHealth = 1;

    
}
