using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class UserData
{
    [SerializeField]
    private int PhyStat;//근력 수치 변수
    public int phyStat{
        get{ return PhyStat; }
        set{ PhyStat = value; }
    }//수치의 encapsulation을 위한 getter과 setter 함수

    [SerializeField]
    private int IntStat;//지력 수치 변수
    public int intStat{
        get{ return IntStat; }
        set{ IntStat = value; }
    }

    [SerializeField]
    private int StaStat;//체력 수치 변수
    public int staStat{
        get{ return StaStat; }
        set{ StaStat = value; }
    }

    [SerializeField]
    private int MedStat;//명상 수치 변수
    public int medStat{
        get{ return MedStat; }
        set{ MedStat = value; }
    }

    [SerializeField]
    private int SpeStat;//화술 수치 변수
    public int speStat{
        get{ return SpeStat; }
        set { SpeStat = value; }
    }

    [SerializeField]
    private int WeaStat;//무기 수치 변수
    public int weaStat{
        get{ return WeaStat; }
        set{ WeaStat = value; }
    }

    [SerializeField]
    private int Money;
    public int money{
        get{ return Money; }
        set{ Money = value; }
    }

    [SerializeField]
    private int MaxHealth;
    public int maxHealth{
        get{ return MaxHealth; }
        set{ MaxHealth = value; }
    }

    [SerializeField]
    private int CurrentHealth;
    public int currentHealth{
        get{ return CurrentHealth; }
        set{ CurrentHealth = value; }
    }

    [SerializeField]
    private int ThisRunSeed;//이번 시드
    public int thisRunSeed{
        get{return ThisRunSeed;}
        set{ThisRunSeed = value;}
    }

    [SerializeField]
    private string MainStat;//캐릭터의 메인 스탯
    public string mainStat{
        get{ return MainStat;}
        set{MainStat = value;}
    }

    [SerializeField]
    private string SubStat;//캐릭터의 서브 스탯
    public string subStat{
        get{return SubStat;}
        set{SubStat = value;}
    }

    [SerializeField]
    private StageInfo StageBeforeInfo; // 이전 스테이지 정보
    public StageInfo stageBeforeInfo
    {
        get { return StageBeforeInfo; }
        set { StageBeforeInfo = value; }
    }

    [SerializeField]
    private int StageCounter; // 현재 스테이지 카운터
    public int stageCounter
    {
        get { return StageCounter; }
        set { StageCounter = value; }
    }

    [SerializeField]
    private int LevelCounter;
    public int levelCounter
    {
        get { return LevelCounter; }
        set { LevelCounter = value; }
    }
    

    [SerializeField]
    private List<string> MainWeapon; // 착용중인 무기
    public List<string> mainWeapon
    {
        get { return MainWeapon; }
        set { MainWeapon = value; }
    }

    [SerializeField]
    private List<string> NormalItemInventory; // 소지품 목록
    public List<string> normalItemInventory
    {
        get { return NormalItemInventory; }
        set { NormalItemInventory = value; }
    }


    [SerializeField]
    private List<string> NextEnemy; // 다음 예정된 적
    public List<string> nextEnemy
    {
        get { return NextEnemy; }
        set { NextEnemy = value; }
    }

    [SerializeField]
    private List<string> FriendList; // 동료 목록
    public List<string> friendList
    {
        get { return FriendList; }
        set { FriendList = value; }
    }

    [SerializeField]
    private string CurrentEvent; // 만약 진행 중이었다면, 로드를 위한 현재 이벤트
    public string currentEvent
    {
        get { return CurrentEvent; }
        set { CurrentEvent = value; }
    }
}

[System.Serializable]
public struct StageInfo
{
    public string stageKind; // 스테이지 종류
    public string stageType; // 스테이지 결과 (예: 승리, 패배 등)
}
