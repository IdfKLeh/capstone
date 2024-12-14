using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class BattleTextController : MonoBehaviour
{
    private BattleText battleText;
    private System.Random random = new System.Random();

    void Start()
    {
        LoadBattleTextList();
    }

    void LoadBattleTextList()
    {
        string battleTextPath = Path.Combine(Application.streamingAssetsPath, "BattleStages", "BattleText.json");
        Debug.Log("Loading events from:" + battleTextPath);

        if (File.Exists(battleTextPath))
        {
            string dataAsJson = File.ReadAllText(battleTextPath);
            var wrapper = JsonConvert.DeserializeObject<BattleTextWrapper>(dataAsJson);
            if (wrapper != null)
            {
                battleText = wrapper.battleText;
                Debug.Log("Battle text loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to parse JSON.");
            }
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }// BattleText.json 파일을 읽어와서 battleTextList에 저장

    public string GetChosenBattleText(string type, string smallerType)
    {
        List<string> filteredTexts = new List<string>();
        if (type == "AmountText")
        {
            var amountTextMap = new Dictionary<string, List<string>>()
            {
                { "smallest", battleText.amountText.smallest },
                { "smaller", battleText.amountText.smaller },
                { "small", battleText.amountText.small },
                { "medium", battleText.amountText.medium },
                { "large", battleText.amountText.large },
                { "larger", battleText.amountText.larger },
                { "largest", battleText.amountText.largest }
            };

            if (amountTextMap.ContainsKey(smallerType))
            {
                filteredTexts.AddRange(amountTextMap[smallerType]);
            }
            else
            {
                Debug.LogError("Invalid AmountText type: " + smallerType);
            }
        }
        else if (type == "WeaponText")
        {
            var weaponTextMap = new Dictionary<string, List<string>>()
            {
                { "broadSword", battleText.weaponText.broadSword },
                { "dagger", battleText.weaponText.dagger },
                { "fist", battleText.weaponText.fist }
                
                // Add other weapon types here
            };

            if (weaponTextMap.ContainsKey(smallerType))
            {
                filteredTexts.AddRange(weaponTextMap[smallerType]);
            }
            else
            {
                Debug.LogError("Invalid WeaponText type: " + smallerType);
            }
        }
        else
        {
            Debug.LogError("Invalid type: " + type);
        }
        

        if (filteredTexts.Count > 0)
        {
            int index = random.Next(filteredTexts.Count);
            return filteredTexts[index];
        }
        else
        {
            return "No matching text found.";
        }
    }// type과 smallerType에 따라 필터링된 battleTextList에서 랜덤으로 하나의 텍스트를 반환
    //근데 딕셔너리로 만드는 부분이 ㅈㄴ 비효율적. 나중에 좀 바꿔보자. 딕셔너리를 아예 없애도 될듯.
}