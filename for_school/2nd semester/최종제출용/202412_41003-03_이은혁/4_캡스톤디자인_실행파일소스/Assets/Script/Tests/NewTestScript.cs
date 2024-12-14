using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StaminaCalcTests
{
    private UserController userController;


    [SetUp]
    public void Setup()
    {

        GameObject gameObject = new GameObject();
        userController = gameObject.AddComponent<UserController>();
        userController.NewGame();
    }

    [Test]
    public void NewGame_SaveGame_GetStatValue_Test()
    {
        userController.NewGame();

        // NewGame() 호출 후 userData가 제대로 초기화되었는지 확인
        Assert.AreEqual(200, userController.GetStatValue("phyStat"), "phyStat 초기화 실패");
        if(userController.GetStatValue("phyStat") == 200)
            Debug.Log("phyStat normally initialized");
        Assert.AreEqual(200, userController.GetStatValue("intStat"), "intStat 초기화 실패");
        if (userController.GetStatValue("intStat") == 200)
            Debug.Log("intStat normally initialized");
        Assert.AreEqual(200, userController.GetStatValue("staStat"), "staStat 초기화 실패");
        if (userController.GetStatValue("staStat") == 200)
            Debug.Log("staStat normally initialized");
        Assert.AreEqual(200, userController.GetStatValue("speStat"), "speStat 초기화 실패");
        if (userController.GetStatValue("speStat") == 200)
            Debug.Log("speStat normally initialized");
        Assert.AreEqual(200, userController.GetStatValue("medStat"), "medStat 초기화 실패");
        if (userController.GetStatValue("medStat") == 200)
            Debug.Log("medStat normally initialized");
        Assert.AreEqual(200, userController.GetStatValue("weaStat"), "weaStat 초기화 실패");
        if (userController.GetStatValue("weaStat") == 200)
            Debug.Log("weaStat normally initialized");
        

        // SaveData() 호출 후 save파일이 제대로 생성되었는지 확인
        userController.SaveData();
        Assert.IsTrue(System.IO.File.Exists(Path.Combine(Application.persistentDataPath, "Save","userData.json")), "SaveData() 호출 실패");
        if (System.IO.File.Exists(Path.Combine(Application.persistentDataPath, "Save", "userData.json")))
            Debug.Log("Data saved successfully");
    }//newgame, savegame, getstatvalue 테스트

    [Test]
    public void ChangeStat_Test()
    {
        userController.NewGame();
        //스탯 초기화

        userController.ChangeStat(50, "phyStat");
        Assert.AreEqual(250, userController.GetStatValue("phyStat"), "phyStat 증가 실패");
        if (userController.GetStatValue("phyStat") == 250)
            Debug.Log("phyStat increased successfully. Current value: " + userController.GetStatValue("phyStat"));

        userController.ChangeStat(-50, "intStat");
        Assert.AreEqual(150, userController.GetStatValue("intStat"), "intStat 감소 실패");
        if (userController.GetStatValue("intStat") == 150)
            Debug.Log("intStat decreased successfully. Current value: " + userController.GetStatValue("intStat"));
    }//changestat 테스트



    [Test]
    public void StaminaCalc_WhenStaStatIsBelowReference_Test()
    {
        userController.NewGame();
        //스탯 초기화

        userController.ChangeStat(-50,"staStat");
        int amount = 8;

        int result = userController.StaminaCalc(amount);

        // 예상되는 결과 계산: staStat이 기준값의 절반일 때
        float expectedMultiplier = Mathf.Lerp(0.1f, 1.0f, 0.5f);
        int expectedAmount = Mathf.RoundToInt(amount * expectedMultiplier);

        Assert.AreEqual(expectedAmount, result, "Stamina 계산이 예상 값과 일치하지 않습니다.");
        if(result == expectedAmount)
            Debug.Log("increasement calculated successfully. Expected value: " + expectedAmount + " Result: " + result);
    }//미리 지정된 reference 값(300)보다 staStat이 낮을 때(150)의 StaminaCalc 테스트

    [Test]
    public void StaminaCalc_WhenStaStatIsAboveReference_Test()
    {
        userController.NewGame();
        //스탯 초기화

        userController.ChangeStat(450, "staStat");
        int amount = 8;

        int result = userController.StaminaCalc(amount);

        // 예상되는 결과 계산: staStat이 기준값의 절반일 때
        float expectedMultiplier = Mathf.Lerp(1.0f, 2.0f, 0.5f);
        int expectedAmount = Mathf.RoundToInt(amount * expectedMultiplier);

        Assert.AreEqual(expectedAmount, result, "Stamina 계산이 예상 값과 일치하지 않습니다.");
        if (result == expectedAmount)
            Debug.Log("increasement calculated successfully. Expected value: " + expectedAmount + " Result: " + result);
    }//미리 지정된 reference 값보다 staStat이 높을 때(650)의 StaminaCalc 테스트

    [Test]
    public void StaminaCalc_WhenStaStatIsAtReference_Test()
    {
        userController.ChangeStat(100, "staStat");
        int amount = 8;

        int result = userController.StaminaCalc(amount);

        Assert.AreEqual(amount, result, "Stamina가 기준값일 때, 결과는 변경되지 않아야 합니다.");
        if (result == amount)
            Debug.Log("Stamina is at reference value. Expected value: " + amount+ " Result: " + result);
    }//미리 지정된 reference 값과 staStat이 같을 때(300)의 StaminaCalc 테스트

    [Test]
    public void StaminaCalcChangeStat_Test()
    {
        userController.NewGame();
        //스탯 초기화

        userController.StaminaCalcChangeStat("intStat");
        //intStat을 증가시키는 함수 호출

        if(userController.GetStatValue("intStat") > 200)
            Debug.Log("intStat increased succesfully. Current value: " + userController.GetStatValue("intStat"));

        Assert.AreEqual(userController.GetStatValue("intStat"), 200+userController.StaminaCalc(8), "Stamina 계산이 예상 값과 일치하지 않습니다.");
        if(userController.GetStatValue("intStat") == 200 + userController.StaminaCalc(8))
            Debug.Log("Expected intStat value: " + (200 + userController.StaminaCalc(8)) + " Result: " + userController.GetStatValue("intStat"));
    }//StaminaCalcChangeStat로 phyStat을 증가한 뒤, 증가 값이 제대로 반영되었는지 확인



}

public class KarmaTests
{
    private UserController userController;

    [SetUp]
    public void SetUp()
    {
        GameObject gameObject = new GameObject();
        userController = gameObject.AddComponent<UserController>();
        userController.NewGame();
        userController.ChangeStat(200,"speStat");
    }

    [Test]
    public void PossibilityCalc_ReturnsCorrectValue_WhenStatIsBelowReference()
    {
        // Arrange
        string statType = "medStat";
        bool startAtHalf = false;

        // Act
        float result = userController.PossibilityCalc(statType, startAtHalf);

        // Assert
        // medStat = 200, referenceMedStat = 300
        // minPossibility = 0.05, refPossibility = 0.2
        float expected = Mathf.Lerp(0.05f, 0.2f, (float)200 / 300) * 100;
        Debug.Log("Expected: " + expected+ " Result: "+result);
        Assert.AreEqual(expected, result, 0.01f, "Possibility should be correctly calculated for stats below the reference.");
    }//reference 값보다 낮은 medStat일 때의 PossibilityCalc 테스트

    [Test]
    public void PossibilityCalc_ReturnsCorrectValue_WhenStatIsAboveReference()
    {
        // Arrange
        string statType = "speStat";
        bool startAtHalf = true;

        // Act
        float result = userController.PossibilityCalc(statType, startAtHalf);

        // Assert
        // speStat = 400, referenceSpeStat = 500, maxSpeStat = 1200
        // minPossibility = -0.4, maxPossibility = 0.3, refPossibility = 0.0
        float t = (400f - 300f) / (1000f - 300f);
        float expected = (Mathf.Lerp(0.0f, 0.3f, t) + 0.5f) * 100;
        Debug.Log("Expected: " + expected + " Result: " + result);
        Assert.AreEqual(expected, result, 0.01f, "Possibility should be correctly calculated for stats above the reference.");
    }//reference 값보다 높은 speStat일 때의 PossibilityCalc 테스트

    [Test]
    public void PossibilityCalc_HandlesEdgeCase_WhenStatIsExactlyReference()
    {
        // Arrange
        userController.NewGame();
        userController.ChangeStat(100,"medStat"); // Exactly at referenceMedStat
        string statType = "medStat";
        bool startAtHalf = false;

        // Act
        float result = userController.PossibilityCalc(statType, startAtHalf);

        // Assert
        float expected = 0.2f * 100; // refPossibility is used directly
        Debug.Log("Expected: " + expected + " Result: " + result);
        Assert.AreEqual(expected, result, 0.01f, "Possibility should match the reference value when stat equals reference.");
    }//reference 값과 같은 medStat일 때의 PossibilityCalc 테스트

    [Test]
    public void PossibilityCalc_HandlesEdgeCase_WhenStatIsZero()
    {
        // Arrange
        userController.NewGame();
        userController.ChangeStat(-200, "medStat");
        string statType = "medStat";
        bool startAtHalf = true;

        // Act
        float result = userController.PossibilityCalc(statType, startAtHalf);

        // Assert
        float expected = (-0.4f + 0.5f) * 100; // minPossibility + 0.5
        Debug.Log("Expected: " + expected + " Result: " + result);
        Assert.AreEqual(expected, result, 0.01f, "Possibility should match the minimum value when stat is zero.");
    }//0인 medStat일 때의 PossibilityCalc 테스트

    [Test]
    public void PossibilityCalc_HandlesEdgeCase_WhenStatIsMaxStat()
    {
        // Arrange
        userController.NewGame();
        userController.ChangeStat(800, "medStat"); // Exactly at maxMedStat
        string statType = "medStat";
        bool startAtHalf = true;

        // Act
        float result = userController.PossibilityCalc(statType, startAtHalf);

        // Assert
        float expected = (0.3f + 0.5f) * 100; // maxPossibility + 0.5
        Debug.Log("Expected: " + expected + " Result: " + result); 
        Assert.AreEqual(expected, result, 0.01f, "Possibility should match the maximum value when stat equals maxStat.");
    }//maxMedStat인 medStat일 때의 PossibilityCalc 테스트

    [Test]
    public void Test_GetKarma_Probabilities()
    {
        userController.NewGame();
        userController.ChangeStat(800,"medStat");
        // medStat을 max값인 1000으로 설정하고 테스트 실행

        // 반복 횟수 설정 (1000번 정도)
        int iterations = 1000;

        // 각 결과를 카운팅
        int goodCount = 0;
        int normalCount = 0;
        int badCount = 0;

        // medStat을 max값인 1000으로 설정하고 테스트 실행
        for (int i = 0; i < iterations; i++)
        {
            string karma = userController.GetKarma("medStat", false); // medStat과 startAtHalf = false
            if (karma == "Good")
            {
                goodCount++;
            }
            else if (karma == "Normal")
            {
                normalCount++;
            }
            else if (karma == "Bad")
            {
                badCount++;
            }
        }

        // 출력으로 각 결과의 비율 확인
        Debug.Log($"Good: {goodCount / (float)iterations * 100}% expectations: 25%");
        Debug.Log($"Normal: {normalCount / (float)iterations * 100}% expectations: 50%");
        Debug.Log($"Bad: {badCount / (float)iterations * 100}% expectations: 25%");

        // 각 결과의 비율이 예상 확률과 근사하는지 확인
        Assert.AreEqual(25f, (goodCount / (float)iterations * 100),5f, "Good 비율이 25%가 아닙니다.");
        Assert.AreEqual(50f, (normalCount / (float)iterations * 100), 5f, "normal 비율이 50%가 아닙니다.");
        Assert.AreEqual(25f, (badCount / (float)iterations * 100), 5f, "bad 비율이 25%가 아닙니다.");
    }//GetKarma의 결과값이 +-5%의 오차 범위 내에서 각 결과의 비율이 예상 확률과 근사하는지 확인

}

public class GameBalanceTests{
    [Test]
    public void GameBalance_ReferenceValues_Test()
    {
        Assert.AreEqual(8, GameBalance.basicTrainingValue, "basicTrainingValue 값이 일치하지 않습니다.");
        Assert.AreEqual(4, GameBalance.mainStatAdditionalValue, "mainStatAdditionalValue 값이 일치하지 않습니다.");
        Assert.AreEqual(2, GameBalance.subStatAdditionalValue, "subStatAdditionalValue 값이 일치하지 않습니다.");
        Assert.AreEqual(100, GameBalance.mainStatExtraValue, "mainStatExtraValue 값이 일치하지 않습니다.");
        Assert.AreEqual(50, GameBalance.subStatExtraValue, "subStatExtraValue 값이 일치하지 않습니다.");
        Assert.AreEqual(1000, GameBalance.maxStaStat, "maxStaStat 값이 일치하지 않습니다.");
        Assert.AreEqual(1000, GameBalance.maxMedStat, "maxMedStat 값이 일치하지 않습니다.");
        Assert.AreEqual(1000, GameBalance.maxSpeStat, "maxSpeStat 값이 일치하지 않습니다.");
        Assert.AreEqual(300, GameBalance.referenceStaStat, "referenceStaStat 값이 일치하지 않습니다.");
        Assert.AreEqual(300, GameBalance.referenceMedStat, "referenceMedStat 값이 일치하지 않습니다.");
        Assert.AreEqual(300, GameBalance.referenceSpeStat, "referenceSpeStat 값이 일치하지 않습니다.");
        Assert.AreEqual(70f, GameBalance.positiveFirstLockReference, "positiveFirstLockReference 값이 일치하지 않습니다.");
        Assert.AreEqual(30f, GameBalance.negativeFirstLockReference, "negativeFirstLockReference 값이 일치하지 않습니다.");
        Assert.AreEqual(80f, GameBalance.positiveSecondLockReference, "positiveSecondLockReference 값이 일치하지 않습니다.");
        Assert.AreEqual(20f, GameBalance.negativeSecondLockReference, "negativeSecondLockReference 값이 일치하지 않습니다.");
        Assert.AreEqual(90f, GameBalance.positiveThirdLockReference, "positiveThirdLockReference 값이 일치하지 않습니다.");
        Assert.AreEqual(10f, GameBalance.negativeThirdLockReference, "negativeThirdLockReference 값이 일치하지 않습니다.");
        Assert.AreEqual(0.5f, GameBalance.firstLockValue, "firstLockValue 값이 일치하지 않습니다.");
        Assert.AreEqual(0.2f, GameBalance.secondLockValue, "secondLockValue 값이 일치하지 않습니다.");
        Assert.AreEqual(0.1f, GameBalance.thirdLockValue, "thirdLockValue 값이 일치하지 않습니다.");
        Assert.AreEqual(3.0f, GameBalance.smallestAmountTextReference, "smallestAmountTextReference 값이 일치하지 않습니다.");
        Assert.AreEqual(7.0f, GameBalance.smallerAmountTextReference, "smallerAmountTextReference 값이 일치하지 않습니다.");
        Assert.AreEqual(12.0f, GameBalance.smallAmountTextReference, "smallAmountTextReference 값이 일치하지 않습니다.");
        Assert.AreEqual(18.0f, GameBalance.mediumAmountTextReference, "mediumAmountTextReference 값이 일치하지 않습니다.");
        Assert.AreEqual(30.0f, GameBalance.largeAmountTextReference, "largeAmountTextReference 값이 일치하지 않습니다.");
        Assert.AreEqual(60.0f, GameBalance.largerAmountTextReference, "largerAmountTextReference 값이 일치하지 않습니다.");
        Assert.AreEqual(0.1f, GameBalance.maxHealthMultiplier, "maxHealthMultiplier 값이 일치하지 않습니다.");
        Assert.AreEqual(1, GameBalance.basicBattleLossLosingHealth, "basicBattleLossLosingHealth 값이 일치하지 않습니다.");
    }//GameBalance의 reference 값들이 제대로 저장되어 있는지 확인

}

public class GameFunctionsTests
{
    [Test]
    public void IsSuccessful_AlwaysFails_WhenSuccessRateIsZero()
    {
        // Arrange
        float successRate = 0;

        // Act
        bool result = GameFunctions.IsSuccessful(successRate);

        // Assert
        Assert.IsFalse(result, "With a 0% success rate, the result should always be false.");
        if(result == false)
            Debug.Log("0% success rate. Result: false");
        
    }//성공확률이 0%일 때 항상 실패하는지 확인

    [Test]
    public void IsSuccessful_AlwaysSucceeds_WhenSuccessRateIsHundred()
    {
        // Arrange
        float successRate = 100;

        // Act
        bool result = GameFunctions.IsSuccessful(successRate);

        // Assert
        Assert.IsTrue(result, "With a 100% success rate, the result should always be true.");
        if(result == true)
            Debug.Log("100% success rate. Result: true");
    }// 성공확률이 100%일 때 항상 성공하는지 확인

    [Test]
    public void IsSuccessful_Approximately50Percent_WhenSuccessRateIsFifty()
    {
        // Arrange
        float successRate = 50;
        int iterations = 1000; // Run the function multiple times
        int successCount = 0;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            if (GameFunctions.IsSuccessful(successRate))
            {
                successCount++;
            }
        }

        float successRateAchieved = (float)successCount / iterations * 100;
        Debug.Log("Success rate achieved: " + successRateAchieved);

        // Assert
        Assert.That(successRateAchieved, Is.InRange(45, 55), 
            $"With a 50% success rate, the actual success rate ({successRateAchieved}%) should be within ±5%.");
    }//성공확률이 50%일 때 실제 성공률이 45%~55% 사이인지 확인
}


/*테스트 짜는 단계
NewGame() 새로운 userData가 지정한 스탯으로 만들어지는지 테스트
SaveData() userData가 당시의 정보를 기반으로 제대로 save파일을 제작하고 갱신하는지 확인

GetStatValue(string statType) 지정한 스탯의 값을 제대로 가져오는지 확인

ChangeStat(int amount, string statType) 지정한 값만큼 해당 스탯을 변경하는지 확인

StaminaCalc(int amount) staStat 값을 반영해서 수치를 조정하여 올바르게 추가하는지 확인
StaminaCalcChangeStat(string statType) staStat값, mainStat, subStat을 반영하여 값을 올바르게 추가하는지 확인



여기까지가 UserController의 StaminaCalc에 대한 테스트

필요하다면 하나의 예시를 더 넣자.
GameBalance 에 제대로 reference 값들이 저장되어있는지 확인

GameFunctions.IsSuccessful() 제대로 성공확률에 따라 결과를 반환하는지 확인

PossibilityCalc(string statType, bool startAtHalf) 특정 stat에 따라 확률을 제대로 계산하는지, startAtHalf가 true일 때 50프로의 확률에서 시작하는지 확인
GetKarma(string statType, bool startAtHalf)) 카르마 값을 특정 stat에 따라 제대로 반환하는지 확인.
*/