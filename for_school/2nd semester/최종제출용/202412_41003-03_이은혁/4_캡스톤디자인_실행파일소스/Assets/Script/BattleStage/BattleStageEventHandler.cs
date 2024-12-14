using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


//아직 작동 안됨. 지금 먼저 이전 스테이지 단계로 가서 무기랑 다음 적 정해주는 거부터 만들거임.
public class BattleStageEventHandler : MonoBehaviour
{
    private UserController userController;
    private EnemyController enemyController;
    private WeaponController weaponController;
    private BattleTextController battleTextController;
    private BattleStageButtonController battleStageButtonController;
    private Dictionary<string,float> battleLogToShow = new Dictionary<string, float>();//계산이 끝난 이후 출력을 위해 보낼 battleLog.
    private List<BattleLogData> battleLogInOrder = new List<BattleLogData>();//순서 정돈이 끝난 battleLogList.
    private List<BattleLogData> userBattleLogDataList = new List<BattleLogData>();//string화 되기 전의 모든 user의 battleLogData를 저장하는 리스트
    private List<BattleLogData> friendBattleLogDataList = new List<BattleLogData>();//string화 되기 전의 모든 friend의 battleLogData를 저장하는 리스트
    private List<BattleLogData> enemyBattleLogDataList = new List<BattleLogData>();//string화 되기 전의 모든 enemy의 battleLogData를 저장하는 리스트
    private List<float> damagePercentageAfterCalculationList = new List<float>();//계산이 끝난 데미지 비율을 저장하는 리스트
    private float returningWinRatePercentage = 50.0f;//최종 데미지 비율.
    private List<int> userHitCountList = new List<int>();//유저의 무기 및 아이템, 스킬에 따른 히트 수를 저장하는 리스트
    private List<int> friendHitCountList = new List<int>();//동료의 무기 및 아이템, 스킬에 따른 히트 수를 저장하는 리스트
    private List<int> enemyHitCountList = new List<int>();//적의 무기 및 아이템, 스킬에 따른 히트 수를 저장하는 리스트
    // Start is called before the first frame update
    void Start()
    {
        userController = FindObjectOfType<UserController>();
        enemyController = FindObjectOfType<EnemyController>();
        weaponController = FindObjectOfType<WeaponController>();
        battleTextController = FindObjectOfType<BattleTextController>();
        battleStageButtonController = FindObjectOfType<BattleStageButtonController>();

        enemyController.LoadEnemyData("EnemyList.json",userController.GetNextEnemy());
        weaponController.LoadAllWeapons(userController.GetWeaponID(),null,enemyController.GetWeaponID());//아직 friend를 안만들어서 null로 넣음
        StartBattle();
    }

    void StartBattle()
    {
        if (battleTextController == null || battleStageButtonController == null)
            {
                Debug.LogError("BattleTextController or BattleStageButtonController is not initialized.");
                return;
            }
        Debug.Log("StartBattle called.");
        LogTextCalc("user");
        LogTextCalc("friend");
        LogTextCalc("enemy");
        BattleLogOrderAndPercentageCalc();
        MakeBattleLogToShow();
        battleStageButtonController.SetBattleLog(battleLogToShow,returningWinRatePercentage);
    }//순서대로 유저, 동료, 적의 로그를 계산하여 battleLogBeforeCalculation에 저장한 후, BattleLogOrderCalc를 호출하여 battleLogToShow에 저장.
    //아직 friend, enemy의 경우 이름 정보를 전달해줘야하는데 그걸 아직 안했음.

    void LogTextCalc(string person)
    {
        Debug.Log("LogTextCalc called with person:" + person);
        WeaponTextCalc(person);
        //ItemTextCalc(person);
        if(person == "enemy")
        {
            SkillTextCalc(person);
        }
        //SkillTextCalc(person);
    }//순서대로 무기, 아이템, 스킬의 로그를 계산

    void SkillTextCalc(string person)
    {
        Debug.Log("SkillTextCalc called with person: " + person);
        List<(string,Skill)> targetSkillData = new List<(string, Skill)>();
        List<int> enemyMedStat = enemyController.GetEnemyMedStat();
        int didEnemyChangeCounter = 0;
        if(person == "enemy")
        {
            targetSkillData = enemyController.GetEnemySkillData();
        }// 제작중인데 enemyContoller.GetEnemySkillData는 enemy의 이름을 반납하지 않고 그냥 스킬 이름만 적어둬서 로그 작성에 문제. 나중에 시간나면 수정할 것.

        for (int i = 0; i < targetSkillData.Count; i++)
        {
            string enemyName = targetSkillData[i].Item1;
            Skill skill = targetSkillData[i].Item2;
            if(GameFunctions.IsSuccessful(skill.skillRate*enemyMedStat[didEnemyChangeCounter]))
            {
                BattleLogData skillHitData;
                switch(person)
                {
                    case "enemy":
                        skillHitData = new BattleLogData(enemyName,"Foe", null, "Skill", skill.skillName, skill.skillDamage);
                        enemyBattleLogDataList.Add(skillHitData);
                        break;
                    default:
                        break;
                }
            }
            if(i < targetSkillData.Count-1)
            {
                if(enemyName != targetSkillData[i+1].Item1)
                {
                    didEnemyChangeCounter++;
                }
            }
        }
    }//스킬을 사용한 hit의 text들을 계산하여 battleLogDataList에 저장.
    //근데 함수 너무 거지같다.. didEnemyChangeCounter를 쓰는게 맞는건지도 모르겠음. 나중에 더 나은 방법을 찾아야할듯.
    void WeaponTextCalc(string person)
    {
        Debug.Log("WeaponTextCalc called with person: " + person);
        List <WeaponData> targetWeaponData = new List<WeaponData>();
        if(person == "user")
        {
            targetWeaponData = weaponController.GetUserWeaponData();
            foreach(WeaponData weapon in targetWeaponData)
            {
                userHitCountList.Add(weapon.hitCount);
            }
            Debug.Log("User weapon data retrieved.");
        }//유저인 경우의 케이스. 유저의 무기 데이터를 받아와서 계산.

        else if(person=="friend")
        {
            targetWeaponData = weaponController.GetFriendWeaponData();
            foreach(WeaponData weapon in targetWeaponData)
            {
                friendHitCountList.Add(weapon.hitCount);
            }
        }//동료인 경우의 케이스. 동료의 무기 데이터를 받아와서 계산.
        
        else if(person == "enemy")
        {
            targetWeaponData = weaponController.GetEnemyWeaponData();
            foreach(WeaponData weapon in targetWeaponData)
            {
                enemyHitCountList.Add(weapon.hitCount);
            }
        }//적인 경우의 케이스. 적의 무기 데이터를 받아와서 계산.

        if (targetWeaponData == null)
        {
            Debug.LogError($"Weapon data for {person} is not initialized.");
            return;
        }
    

        int hitrate = 0;
        float damagePercentage = 0.0f;
        if(targetWeaponData.Count == 0 || targetWeaponData == null)
        {
            Debug.Log("targetWeaponData is empty or null.");
            return;
        }
        for(int i = 0; i < targetWeaponData.Count; i++)
        {
            WeaponData weapon = targetWeaponData[i];
            damagePercentage = (float)Math.Round(userController.GetStatValue("phyStat") * weapon.phyStatRate,2);
            foreach(WeaStatRate weaStat in weapon.weaStatRate)
                switch(weaStat.calc)
                {
                    case "Hit Rate":
                        hitrate = (int)Math.Ceiling(weaStat.rate * userController.GetStatValue("weaStat"));
                        Debug.Log("Hit rate calculated: " + hitrate);
                        break;
                    default:
                        break;
                }
            
            for (int j = 0; j < hitrate; j++)
            {
                BattleLogData weaponHitData;
                switch(person)
                {
                    case "user":
                        weaponHitData = new BattleLogData("You","Friendly", null, battleTextController.GetChosenBattleText("WeaponText",weapon.weaponType), weapon.weaponName, damagePercentage);
                        userBattleLogDataList.Add(weaponHitData);
                        Debug.Log("User's weapon hit data added.");
                        break;
                    case "friend":
                        weaponHitData = new BattleLogData(userController.GetFriendName()[i],"Friendly", null, battleTextController.GetChosenBattleText("WeaponText",weapon.weaponType), weapon.weaponName, damagePercentage);//아직 friend를 반환하는 함수를 안만들어서 null로 해놨음. 만들면 userController.GetFriendName()[i]으로 바꿔야함.
                        friendBattleLogDataList.Add(weaponHitData);
                        break;
                    case "enemy":
                        weaponHitData = new BattleLogData(enemyController.GetEnemyName()[i],"Foe", null, battleTextController.GetChosenBattleText("WeaponText",weapon.weaponType), weapon.weaponName, damagePercentage);
                        enemyBattleLogDataList.Add(weaponHitData);
                        break;
                    default:
                        break;
                }
            }
        }
    }//무기를 사용한 hit의 text들을 계산하여 battleLogDataList에 저장

    void BattleLogOrderAndPercentageCalc()
    {
        BattleLogPercentageCalc(BattleLogOrderCalc());
    }//순서대로 BattleLogOrderCalc를 호출하고, BattleLogPercentageCalc를 호출.
    List<BattleLogData> BattleLogOrderCalc()
    {
        int userIndex = 0;
        int friendIndex = 0;
        int enemyIndex = 0; //각각의 logList에서 현재 몇번째 문장을 출력중인지 index를 나타내는 변수
        
        int userHitCountOrder = 0;
        int friendHitCountOrder = 0;
        int enemyHitCountOrder = 0; // 
    
        List<BattleLogData> orderedBattleLogDataList = new List<BattleLogData>();
    
        while (userIndex < userBattleLogDataList.Count || friendIndex < friendBattleLogDataList.Count || enemyIndex < enemyBattleLogDataList.Count)
        {
            if(userIndex < userBattleLogDataList.Count || friendIndex < friendBattleLogDataList.Count)
            {
                if(userIndex < userBattleLogDataList.Count)
                {
                    for(int i = 0; i < userHitCountList[userHitCountOrder]; i++)
                    {
                        if(userIndex >= userBattleLogDataList.Count)
                            break;
                        orderedBattleLogDataList.Add(userBattleLogDataList[userIndex]);
                        if(DidTheAttackerChange(userBattleLogDataList,userIndex))
                        {
                            userHitCountOrder++; //여긴 오류날듯? userBattleLogDataList에서 모든 attacker은 user로 저장되어있는데 무기가 바뀐다고해서 attacker가 바뀌는건 아님 = 인식이 안됨.
                            break;
                        }
                        userIndex++;
                            
                    }
                }
                else
                {
                    for(int i = 0; i < friendHitCountList[friendHitCountOrder]; i++)
                    {
                        if(friendIndex >= friendBattleLogDataList.Count)
                            break;
                        orderedBattleLogDataList.Add(friendBattleLogDataList[friendIndex]);
                        if(DidTheAttackerChange(friendBattleLogDataList,friendIndex))
                        {
                            friendHitCountOrder++;
                            break;
                        }
                        friendIndex++;
                    }
                }
            }
            if(enemyIndex < enemyBattleLogDataList.Count)
            {
                for(int i = 0; i < enemyHitCountList[enemyHitCountOrder]; i++)
                {
                    if(enemyIndex >= enemyBattleLogDataList.Count)
                        break;
                    orderedBattleLogDataList.Add(enemyBattleLogDataList[enemyIndex]);
                    if(DidTheAttackerChange(enemyBattleLogDataList,enemyIndex))
                    {
                        enemyHitCountOrder++;
                        break;
                    }
                    enemyIndex++;
                }
            }
        }
    
        battleLogInOrder = orderedBattleLogDataList;
        return orderedBattleLogDataList;
    }//순서대로 아군, 적군의 로그를 정렬하고, 퍼센티지 계산 전의 battleLogDataListRightBeforePercentageCalc에 저장. 만약 아군 적군 중 특정한 한쪽이 먼저 끝나면 다른 한쪽의 로그를 모두 출력.

    void BattleLogPercentageCalc(List<BattleLogData> orderedBattleLogDataList)
    {
        float positiveFirstLockReference = GameBalance.positiveFirstLockReference;
        float negativeFirstLockReference = GameBalance.negativeFirstLockReference;
        float positiveSecondLockReference = GameBalance.positiveSecondLockReference;
        float negativeSecondLockReference = GameBalance.negativeSecondLockReference;
        float positiveThirdLockReference = GameBalance.positiveThirdLockReference;
        float negativeThirdLockReference = GameBalance.negativeThirdLockReference;
        
        float firstLockValue = GameBalance.firstLockValue;
        float secondLockValue = GameBalance.secondLockValue;
        float thirdLockValue = GameBalance.thirdLockValue;

        float percentageMultiplier = 1.0f;

        foreach(BattleLogData battleLogData in orderedBattleLogDataList)
        {
            float targetHitPercentage = battleLogData.damagePercentage;
            if(battleLogData.typeOfAttacker == "Foe")
            {
                Debug.Log("Foe's hit percentage: " + targetHitPercentage);
                targetHitPercentage *= -1; 
                if (returningWinRatePercentage < 50 && returningWinRatePercentage >= negativeFirstLockReference)
                {
                    float t = (50f - returningWinRatePercentage) / (50f - negativeFirstLockReference);
                    percentageMultiplier = Mathf.Lerp(1.0f, firstLockValue, t);
                }
                else if (returningWinRatePercentage < negativeFirstLockReference && returningWinRatePercentage >= negativeSecondLockReference)
                {
                    float t = (50f - returningWinRatePercentage) / (50f - negativeSecondLockReference);
                    percentageMultiplier = Mathf.Lerp(firstLockValue, secondLockValue, t);
                }
                else if (returningWinRatePercentage < negativeSecondLockReference && returningWinRatePercentage >= negativeThirdLockReference)
                {
                    float t = (50f - returningWinRatePercentage) / (50f - negativeThirdLockReference);
                    percentageMultiplier = Mathf.Lerp(secondLockValue, thirdLockValue, t);
                }
                else if (returningWinRatePercentage < negativeThirdLockReference)
                {
                    float t = (50f - returningWinRatePercentage) / 50f;
                    percentageMultiplier = Mathf.Lerp(thirdLockValue, 0f, t);
                }
            }
            else
            {
                Debug.Log("Friendly's hit percentage: " + targetHitPercentage);
                if(returningWinRatePercentage > 50 && returningWinRatePercentage <= positiveFirstLockReference)
                {
                    float t = returningWinRatePercentage / positiveFirstLockReference;
                    percentageMultiplier = Mathf.Lerp(1.0f, firstLockValue, t);
                }
                else if (returningWinRatePercentage > positiveFirstLockReference && returningWinRatePercentage <= secondLockValue)
                {
                    float t = returningWinRatePercentage / positiveSecondLockReference;
                    percentageMultiplier = Mathf.Lerp(firstLockValue, secondLockValue, t);
                }
                else if (returningWinRatePercentage > positiveSecondLockReference && returningWinRatePercentage <= positiveThirdLockReference)
                {
                    float t = returningWinRatePercentage / positiveThirdLockReference;
                    percentageMultiplier = Mathf.Lerp(secondLockValue, thirdLockValue, t);
                }
                else if (returningWinRatePercentage > positiveThirdLockReference)
                {
                    float t = returningWinRatePercentage / 100;
                    percentageMultiplier = Mathf.Lerp(thirdLockValue, 0f, t);
                }
            }
            targetHitPercentage *= percentageMultiplier;
            targetHitPercentage = (float)Math.Round(targetHitPercentage,2);
            damagePercentageAfterCalculationList.Add(targetHitPercentage);
            Debug.Log("Damage percentage after calculation: " + targetHitPercentage);
            returningWinRatePercentage += targetHitPercentage;

            //여기부턴 targetHitPercentage를 활용해 데미지의 크기에 따라 다른 amountOfDamage text를 설정하는 코드
            float absoluteValue = Math.Abs(targetHitPercentage);
            string damageAmountText;

            if(absoluteValue <= GameBalance.smallestAmountTextReference)
            {
                damageAmountText = battleTextController.GetChosenBattleText("AmountText","smallest");
            }
            else if(absoluteValue <= GameBalance.smallerAmountTextReference)
            {
                damageAmountText = battleTextController.GetChosenBattleText("AmountText","smaller");
            }
            else if(absoluteValue <= GameBalance.smallAmountTextReference)
            {
                damageAmountText = battleTextController.GetChosenBattleText("AmountText","small");
            }
            else if(absoluteValue <= GameBalance.mediumAmountTextReference)
            {
                damageAmountText = battleTextController.GetChosenBattleText("AmountText","medium");
            }
            else if(absoluteValue <= GameBalance.largeAmountTextReference)
            {
                damageAmountText = battleTextController.GetChosenBattleText("AmountText","large");
            }
            else if(absoluteValue <= GameBalance.largerAmountTextReference)
            {
                damageAmountText = battleTextController.GetChosenBattleText("AmountText","larger");
            }
            else
            {
                damageAmountText = battleTextController.GetChosenBattleText("AmountText","largest");
            }
            battleLogData.amountOfDamage = damageAmountText;
        }//현재 최종 winRate에 따라 각 hit의 데미지 비율을 계산하고, 그 비율을 더하여 최종 데미지 비율을 다시 계산하고 해당 값들을 damagePercentageAfterCalculationList에 저장.
        battleLogInOrder = orderedBattleLogDataList;
    }

    public bool DidTheAttackerChange(List<BattleLogData> battleLogDataList, int index)
    {
        if (battleLogDataList == null || battleLogDataList.Count == 0)
        {
            Console.WriteLine("The list is empty or null.");
            return false;
        }
        if(index ==0 )
        {
            return false;
        }
        if (index < 0 || index >= battleLogDataList.Count)
        {
            Debug.LogError($"Index out of range: {index}. List count: {battleLogDataList.Count}");
            return false; // or handle the error as appropriate
        }

        string previousAttacker = battleLogDataList[index-1].attacker;
        string currentAttacker = battleLogDataList[index].attacker;
        return previousAttacker != currentAttacker;
    }//공격자가 바뀌었는지 확인하는 함수. 바뀌었다면 true, 아니면 false를 반환.

    void MakeBattleLogToShow()
    {
        for(int i = 0; i < battleLogInOrder.Count; i++)
        {
            string exampleLog;
            if(battleLogInOrder[i].typeOfDamage == "Skill")
            {
                exampleLog = battleLogInOrder[i].attacker + " used " + battleLogInOrder[i].amountOfDamage+" " + battleLogInOrder[i].weaponName + "!";
            }
            else
            {
                exampleLog = battleLogInOrder[i].attacker + ": " + battleLogInOrder[i].amountOfDamage + " " + battleLogInOrder[i].typeOfDamage + " with " + battleLogInOrder[i].weaponName + "!";
            }
            
            // 동일한 키가 있으면 고유한 숫자 인덱스를 추가
            int uniqueCounter = 1;
            string uniqueLog = exampleLog;
            while (battleLogToShow.ContainsKey(uniqueLog))
            {
                uniqueLog = $"{exampleLog} ({uniqueCounter})";
                uniqueCounter++;
            }

            battleLogToShow.Add(uniqueLog, damagePercentageAfterCalculationList[i]);
            Debug.Log(exampleLog);
        }
    }//BattleStageButtonController에서 사용할 수 있도록 battleLogToShow에 저장.

    public bool DidUserWin()
    {
        return GameFunctions.IsSuccessful(returningWinRatePercentage);
    }//유저가 이겼는지 졌는지 반환하는 함수

    public List<string> ResultHandler(bool didUserWin)
    {
        List<EventStageEventNameSpace.Action> actions = new List<EventStageEventNameSpace.Action>();
        List<string> resultText = new List<string>();
        if(didUserWin)
        {
            userController.SetStageBefore("BattleStage","Positive");
            actions = enemyController.GetWinAction();
            foreach(EventStageEventNameSpace.Action action in actions)
            {
                if(action.type == "moneyGet")
                {
                    userController.AddMoney(action.amount);
                    resultText.Add("You got " + action.amount + " money!");
                }
                if(action.type == "statChange")
                {
                    userController.ChangeStat(action.amount,action.stats);
                    resultText.Add("Your " + action.stats + " has changed by " + action.amount + "!");
                }
                if(action.type == "itemGet")
                {
                    //여기에 아이템 추가하는 코드 추가
                }
            }
        }
        else
        {
            userController.SetStageBefore("BattleStage","Negative");
            userController.BattleLostHealthChange(returningWinRatePercentage);
            actions = enemyController.GetLoseAction();
            foreach(EventStageEventNameSpace.Action action in actions)
            {
                if(action.type == "moneyGet")
                {
                    userController.AddMoney(action.amount);
                    resultText.Add("You lost " + action.amount + " money!");
                }
                if(action.type == "statChange")
                {
                    userController.ChangeStat(action.amount,action.stats);
                    resultText.Add("Your " + action.stats + " has changed by " + action.amount + "!");
                }
                if(action.type == "itemGet")
                {
                    //여기에 아이템 추가하는 코드 추가
                }
                if(action.type == "gameOver")
                {
                    userController.SetCurrentHealth(0);
                    resultText.Add("You lost all your health and the game is over.");
                }
            }
        }

        /*if(userController.GetStageCounter() < 10){
            userController.SetNextEnemy(enemyController.GetRandomEnemyID("EnemyList.json",userController.GetLevelCounter(),1,false));//나중에 여길 보고 특정 스테이지부터 1마리가 아니라 여러마리를 소환해야하는 경우 1을 바꾸는 옵션 넣을것
        }
        else
        {
            userController.SetNextEnemy(enemyController.GetRandomEnemyID("BossList.json",userController.GetLevelCounter(),1,true));
        }//다음 적을 정하는 코드. 현재 스테이지가 10보다 작으면 일반 적을, 10이상이면 보스를 소환.
        //아무래도 이건 MainPlay로 옮기는게 좋을 듯. 왜냐하면 MainPlay에서 Training을 했을 시에도 다음에 만나는 적을 변경해야하기 때문.
        //마찬가지로 bossList에서 불러오는 코드도 충돌이 나기 쉬워서 일단은 제외하겠음.
        */

        userController.SetNextEnemy(enemyController.GetRandomEnemyID("EnemyList.json", userController.GetLevelCounter(), 1, false));
        //임시로 추가해놓은 코드. 제대로 enemy리셋하는 거랑 보스 시스템 추가하면 제거 할 것.


        userController.SaveData();



        return resultText;
    }//이겼는지 졌는지에 따라 결과를 반영하는 함수.

    public void FinishBattleStage(){
        SceneManager.LoadScene("EventStage");
    }//씬 전환해주는 함수
    
}
