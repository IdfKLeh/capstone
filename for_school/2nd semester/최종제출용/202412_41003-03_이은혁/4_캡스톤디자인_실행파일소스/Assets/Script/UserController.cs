using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EventStageEventNameSpace;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UserController : MonoBehaviour
{
    //저장을 위한 변수
    UserData userData = new UserData();
    private string saveDataPath;
    private string saveDataDirectoryPath;
    
    void Awake()
    {
        saveDataDirectoryPath = Path.Combine(Application.persistentDataPath,"Save");
        saveDataPath = Path.Combine(saveDataDirectoryPath, "userData.json");
        LoadData();
        
    }

    public void NewGame(){
        userData = new UserData();
        userData.phyStat = 200;
        userData.intStat = 200;
        userData.staStat = 200;
        userData.speStat = 200;
        userData.medStat = 200;
        userData.weaStat = 200;
        userData.money = 100;
        SetMaxAndCurrentHealth();
        EmptyCurrentEvent();
        SaveData();
    } //새 게임 시작시 설정되는 기본 스탯

    
    public void LoadData(){
        if (File.Exists(saveDataPath)){
            string saveJson = File.ReadAllText(saveDataPath);
            userData = JsonUtility.FromJson<UserData>(saveJson);
            Debug.Log("Data Loaded: time of last save to be added here");
        }
        else{
            Debug.Log("No save file found at: " + saveDataPath);
        }
    }//Json파일에서 내용을 읽어 게임 유저에 덮어쓰는 함수

    
    public void SaveData(){
        if (!Directory.Exists(saveDataDirectoryPath))
        {
            Directory.CreateDirectory(saveDataDirectoryPath);
        }
        try{
            string saveJson = JsonUtility.ToJson(userData);
            File.WriteAllText(saveDataPath, saveJson);
            Debug.Log("userData saved to:" + saveDataPath);
            Debug.Log("saved Json file: "+saveJson);
        }
        catch(Exception e){
            Debug.Log("Failed to save data:"+e.Message);
        }
        
    }//user정보를 저장하는 함수

    
    public void ChangeStat(int amount, string statType){
        switch(statType)
        {
            case "phyStat":
                userData.phyStat += amount;
                break;
            case "intStat":
                userData.intStat += amount;
                break;
            case "staStat":
                userData.staStat += amount;
                break;
            case "medStat":
                userData.medStat += amount;
                ApplyMaxHealthChange();
                break;
            case "speStat":
                userData.speStat += amount;
                break;
            case "weaStat":
                userData.weaStat += amount;
                break;
            default:
                Debug.LogError("Unknown Stat Name"+statType);
                break;
        }
    }//특정 수치를 올리거나 감소시키는 함수.

    public int GetStatValue(string statType){
        switch(statType)
        {
            case "phyStat":
                return userData.phyStat;
            case "intStat":
                return userData.intStat;
            case "staStat":
                return userData.staStat;
            case "medStat":
                return userData.medStat;
            case "speStat":
                return userData.speStat;
            case "weaStat":
                return userData.weaStat;
            default:
                Debug.LogError("Unknown Stat Name"+statType);
                return 0;
        }
    }//특정 user stat 수치를 반환하는 함수.
    internal int StaminaCalc(int amount){

        float minMultiplier = 0.1f;
        float maxMultiplier = 2.0f;
        float referenceMultiplier = 1.0f;

        int staStat = userData.staStat;

        // staStat이 기준값 이하일 때와 이상일 때를 나누어 처리
        float multiplier;
        if (staStat <= GameBalance.referenceStaStat)
        {
            // 기준값 이하일 때 비례적으로 감소
            float t = (float)staStat / GameBalance.referenceStaStat;
            multiplier = Mathf.Lerp(minMultiplier, referenceMultiplier, t);
        }
        else
        {
            // 기준값 이상일 때 비례적으로 증가
            float t = (float)(staStat - GameBalance.referenceStaStat) / (GameBalance.maxStaStat - GameBalance.referenceStaStat);
            multiplier = Mathf.Lerp(referenceMultiplier, maxMultiplier, t);
        }

        // 조정된 amount 계산
        int adjustedAmount = Mathf.RoundToInt(amount * multiplier);
        Debug.Log("calc amount is "+adjustedAmount);
        return adjustedAmount;
    }//stamina 값을 반영해서 추가되는 수치를 조정하는 함수

    public void StaminaCalcChangeStat(string statType)
    {
        int amount = 0;
        if (statType == userData.mainStat)
        {
            amount += GameBalance.mainStatAdditionalValue;
        }   
        else if (statType == userData.subStat)
        {
            amount += GameBalance.subStatAdditionalValue;
        }
        ChangeStat(StaminaCalc(GameBalance.basicTrainingValue)+amount,statType);
    }//training 상황에서 stamina 값을 반영해서 스탯을 바꾸는 함수. 그냥 두개 함수를 합친거임.

    public float PossibilityCalc(string statType, bool startAtHalf)
    {
        int stat = 0;
        int referenceStat = 300;
        int maxStat = 1000;

        float minPossibility;
        float maxPossibility;
        float refPossibility;

        switch(statType)
        {
            case "medStat":
                stat = userData.medStat;
                referenceStat = GameBalance.referenceMedStat;
                maxStat = GameBalance.maxMedStat;
                break;
            case "speStat":
                stat = userData.speStat;
                referenceStat = GameBalance.referenceSpeStat;
                maxStat = GameBalance.maxSpeStat;
                break;
            default:
                stat = userData.medStat;
                referenceStat = GameBalance.referenceMedStat;
                maxStat = GameBalance.maxMedStat;
                break;
        }
        
        // 조건에 따라 가능성 범위를 설정
        if (startAtHalf)
        {
            minPossibility = -0.4f;
            maxPossibility = 0.3f;
            refPossibility = 0.0f;
        }
        else
        {
            minPossibility = 0.05f;
            maxPossibility = 0.5f;
            refPossibility = 0.2f;
        }

        float possibility;
        if (stat <= referenceStat)
        {
            float t = (float)stat / referenceStat;
            possibility = Mathf.Lerp(minPossibility, refPossibility, t);
            if (startAtHalf)
                possibility += 0.5f; // startAtHalf인 경우 0.5 더하기
        }
        else
        {
            float t = (float)(stat - referenceStat) / (maxStat - referenceStat);
            possibility = Mathf.Lerp(refPossibility, maxPossibility, t);
            if (startAtHalf)
                possibility += 0.5f; // startAtHalf인 경우 0.5 더하기 *후에 
        }

        return possibility*100;
    }//medStat에 따라 50프로에서 증감하거나, 0프로에서 시작하는 경우의 확률을 계산하여 반환하는 함수.
    //근데 지금 medStat이 max일때 최대 확률이 50프로인데, 이러면 karma에서 good 25, normal 50, bad 25가 됨. 좀 안나옴 그래서 수정 요망
    public void SetSeed(int seed){
        userData.thisRunSeed = seed;
        SaveData();
    }//시드를 userData에 저장하는 함수

    public int GetSeed(){
        return userData.thisRunSeed;
    }//현재 시드를 반납하는 함수

    public void SetMainStat(string pickedStat){
        userData.mainStat = pickedStat;
        ChangeStat(GameBalance.mainStatExtraValue, pickedStat);
    }//고른 특성을 메인으로 설정하는 함수

    public void SetSubStat(string pickedStat){
        userData.subStat = pickedStat;
        ChangeStat(GameBalance.subStatExtraValue, pickedStat);
    }//고른 특성을 서브로 설정하는 함수

    public void SetStageBefore(string stageKindInput, string stageTypeInput)
    {
        userData.stageBeforeInfo = new StageInfo{stageKind = stageKindInput, stageType = stageTypeInput};
        SaveData();
    }//이전 스테이지의 정보를 저장. 보통 각 스테이지가 끝날때 호출 될 예정

    public StageInfo GetStageBefore()
    {
        return userData.stageBeforeInfo;
    }//이전 스테이지의 정보를 반환

    public string GetKarma(string statType, bool startAtHalf){
        string karma;
        if(GameFunctions.IsSuccessful(PossibilityCalc(statType,startAtHalf)))
        {
            if(GameFunctions.IsSuccessful(PossibilityCalc(statType,startAtHalf))){
                karma = "Good";
            }
            else
            {
                karma = "Normal";
            }
        }
        else
        {
            if(GameFunctions.IsSuccessful(PossibilityCalc(statType,startAtHalf))){
                karma = "Normal";
            }
            else{
                karma = "Bad";
            }
        }
        //Debug.Log("Current Karma: " + karma);
        return karma;
    }//카르마 값을 세개의 선택지(Good, Normal, Bad) 중에 뽑아서 반환하는 함수

    public bool IsRestrictionPassed(Restriction restriction){

        if (restriction == null || string.IsNullOrEmpty(restriction.stats)) 
        {
            Debug.Log("Restriction or restriction.stats is null or empty.");
            return true;
        }
        switch(restriction.stats)
        {
            case "phyStat":
                return(userData.phyStat >= restriction.amount);
            case "intStat":
                return(userData.intStat >= restriction.amount);
            case "staStat":
                return(userData.staStat >= restriction.amount);
            case "medStat":
                return(userData.medStat >= restriction.amount);
            case "speStat":
                return(userData.speStat >= restriction.amount);
            case "weaStat":
                return(userData.weaStat >= restriction.amount);
            default:
                Debug.LogError("Unknown Stat Name"+restriction.stats);
                return false;
        }
    }//restriction이 통과했는지 확인하는 함수.

    public void SetStageCounter(int num){
        userData.stageCounter = num;
    }

    public void AddStageCounter(){
        userData.stageCounter += 1;
    }

    public int GetStageCounter(){
        return userData.stageCounter;
    }//위의 세개는 차례대로 stageCounter값을 특정 수로 설정하고, 1만 더하고, 반환하는 함수

    public void SetLevelCounter(int num){
        userData.levelCounter = num;
    }

    public void AddLevelCounter(){
        userData.levelCounter += 1;
    }

    public int GetLevelCounter(){
        return userData.levelCounter;
    }//위의 세개는 차례대로 levelCounter값을 특정 수로 설정하고, 1만 더하고, 반환하는 함수

    public List<string> GetNextEnemy(){
        Debug.Log("Next Enemy: "+userData.nextEnemy[0]);
        return userData.nextEnemy;
    }//다음 enemy id 를 반환하는 함수.

    public List<string> GetWeaponID(){
        Debug.Log("Main Weapon: "+userData.mainWeapon[0]);
        return userData.mainWeapon;
    }//weaponID 를 반환하는 함수.

    public List<string> GetFriendName(){
        return userData.friendList;
    }//friendList를 반환하는 함수.

    public void SetNextEnemy(List<string> enemyID){
        userData.nextEnemy = enemyID;
    }//다음 enemy id를 설정하는 함수.
    public void SetMainWeapon(List<string> weaponID){
        userData.mainWeapon = weaponID;
    }//main weapon을 설정하는 함수.

    public void SetRandomNextEnemy()
    {
        EnemyController enemyController = FindObjectOfType<EnemyController>();
        SetNextEnemy(enemyController.GetRandomEnemyID("EnemyList.json", GetLevelCounter(), 1,false));//나중 argument 두개는 enemyNum과 visibleLevel인데, 적을 몇마리 소환할지랑 homelevel이 아닌 곳에서도 등장시킬지임.
    }//enemyController에서 랜덤으로 enemyID를 받아와서 nextEnemy에 저장하는 함수.

    public void SetMoney(int amount)
    {
        userData.money = amount;
    }

    public void AddMoney(int amount)
    {
        userData.money += amount;
    }

    public int GetMoney()
    {
        return userData.money;
    }//돈을 설정하고, 더하고, 반환하는 함수.

    public void SetMaxAndCurrentHealth()
    {
        userData.maxHealth = MaxHealthCalc();
        userData.currentHealth = userData.maxHealth;
    }//아마 처음에만 쓸, 최대 체력과 현재 체력(만땅) 설정하는 함수.

    public int MaxHealthCalc()
    {
        return (int)Math.Round((double)(userData.medStat * GameBalance.maxHealthMultiplier));
    }//medStat을 기반으로 최대 체력을 계산하는 함수.

    private void ApplyMaxHealthChange()
    {
        int healthDifference = userData.maxHealth - MaxHealthCalc();
        if(healthDifference != 0)
        {
            userData.maxHealth = MaxHealthCalc();
            userData.currentHealth += healthDifference;
        }
    }//최대 체력을 계산하여 생긴 차이를 userData의 maxHealth와 currentHealth에 적용하는 함수.

    public void SetCurrentHealth(int amount)
    {
        userData.currentHealth = amount;
    }
    
    public void AddCurrentHealth(int amount)
    {
        userData.currentHealth += amount;
    }

    public int GetCurrentHealth()
    {
       return userData.currentHealth;
    }//currentHealth를 설정하고, 더하고, 반환하는 함수.

    public string GetCurrentEvent()
    {
            return userData.currentEvent; 
    }//만약 저장된게 있다면 currentEvent를 반환하는 함수. 없으면 "Empty"를 반환.

    public void SetCurrentEvent(string eventID)
    {
        userData.currentEvent = eventID;
    }//currentEvent를 설정하는 함수.

    public void EmptyCurrentEvent()
    {
        userData.currentEvent = "Empty";
    }//currentEvent를 비우는 함수.


    public void BattleLostHealthChange(float winRatePercentage)
    {
        int losingHealth = GameBalance.basicBattleLossLosingHealth;//깎일 체력
        int referenceLostValue = 50;//체력이 깎이기 시작하는 기준 확률
        if(userData.intStat >= 500)
        {
            referenceLostValue=(int)Math.Round((double)userData.intStat/10);
        }
        referenceLostValue = 100-referenceLostValue;
        if(winRatePercentage >= referenceLostValue)
        {
            AddCurrentHealth(-losingHealth);
        }
        else
        {
            losingHealth += referenceLostValue - (int)winRatePercentage;
            AddCurrentHealth(-losingHealth);
        }
    }//전투 패배시 intStat에 따라 체력이 깎이는 비율을 조정하고, 이에 따라 체력을 깎는 함수.

    public void AddItem(string itemID)
    {
        userData.normalItemInventory.Add(itemID);
    }

    public void RemoveItem(string itemID)
    {
        userData.normalItemInventory.Remove(itemID);
    }//위의 두 함수는 아이템을 추가하고, 제거하는 함수.

    public string LoadGameSceneInfoFunc()
    {
        return userData.stageBeforeInfo.stageKind;
    }//LoadGameSceneInfoFunc는 userData에 저장된 이전 스테이지 정보를 반환하는 함수.

}
