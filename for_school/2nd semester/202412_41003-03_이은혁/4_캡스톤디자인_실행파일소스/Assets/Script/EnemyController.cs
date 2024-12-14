using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using EventStageEventNameSpace;
public class EnemyController : MonoBehaviour
{
    private List<EnemyData> enemyData = new List<EnemyData>();
    private Dictionary<string, EnemyData> enemyDict = new Dictionary<string, EnemyData>();


    public void LoadEnemyData(string fileName, List<string> enemyID)
    {
        LoadEnemyList(fileName);
        SetEnemyData(enemyID);
        // Step 2: Add matching EnemyData to enemyData
    }//enemyDataPath에 저장된 데이터를 enemyData에 저장하는 함수. 먼저 LoadEnemyList를 통해 enemyDict에 데이터를 저장하고, SetEnemyData를 통해 enemyData에 저장함.

    private void LoadEnemyList(string fileName)
    {
        string enemyDataPath = Path.Combine(Application.streamingAssetsPath, "Characters", "Enemy", fileName);
        Debug.Log("Loading events from:" + enemyDataPath);
    
        List<EnemyData> enemyDataList = new List<EnemyData>();
    
        if (!File.Exists(enemyDataPath))
        {
            Debug.LogError("Cannot find file" + enemyDataPath);
            return;
        }
    
        try
        {
            string json = File.ReadAllText(enemyDataPath);
            Debug.Log("JSON content: " + json);
    
            var root = JsonConvert.DeserializeObject<EnemyDataWrapper>(json);
            if (root != null)
            {
                enemyDataList = root.enemy;
                Debug.Log("EnemyDatas loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to parse Json.");
            }
        }
        catch (JsonException ex)
        {
            Debug.LogError("Error parsing Json: " + ex.Message);
        }
    
        // Step 1: Create a dictionary to map enemy IDs to EnemyData objects
        foreach (EnemyData e in enemyDataList)
        {
            if (e != null && !enemyDict.ContainsKey(e.enemyID))
            {
                enemyDict[e.enemyID] = e;
            }
        }
    }// enemyDataPath에 저장된 데이터를 enemyDict에 저장하는 함수. enemyDataPath에 저장된 데이터를 읽어와서 enemyDataList에 저장한 후, enemyDataList에 저장된 데이터를 enemyDict에 저장함.

    private void SetEnemyData(List<string> enemyID)
    {
        foreach (string id in enemyID)
        {
            if (enemyDict.TryGetValue(id, out EnemyData enemy))
            {
                enemyData.Add(enemy);
            }
        }
        Debug.Log("EnemyData set to :"+ enemyData[0].enemyID);
    }// enemyDict에 저장된 데이터 중 enemyID에 해당하는 데이터를 enemyData에 저장하는 함수.

    public List<string> GetRandomEnemyID(string fileName, int level, int enemyNum, bool visibleLevel)
    {
        if (enemyDict == null || enemyDict.Count == 0)
        {
            LoadEnemyList(fileName);
        }
    
        Dictionary<string, EnemyData> appropriateLevelEnemies = GetEnemiesByLevel(level, visibleLevel); // 원래 visibleLevel 쓸건지에 따라 후자를 true,false 로 골라야함.
        List<string> randomEnemies = new List<string>();
    
        if (appropriateLevelEnemies.Count == 0)
        {
            Debug.Log("NO ENEMIES FOUND AT LEVEL " + level+ ", COME FIX ME AT ENEMYCONTROLLER");
            return null; // Return null if no enemies are found at the specified level
        }
    
        System.Random random = new System.Random();

        for (int i = 0; i < enemyNum; i++)
        {
            if (appropriateLevelEnemies.Count == 0)
            {
                break; // Break if there are no more enemies to select
            }

            int randomIndex = random.Next(appropriateLevelEnemies.Count);
            string randomEnemyID = appropriateLevelEnemies.ElementAt(randomIndex).Key;
            randomEnemies.Add(randomEnemyID);

            // Remove the selected enemy to avoid duplicates
            appropriateLevelEnemies.Remove(randomEnemyID);
        }

        return randomEnemies;
    }// enemyDict에 저장된 데이터 중 level에 해당하는 데이터를 랜덤으로 enemyNum개 만큼 가져오는 함수.

    public Dictionary<string, EnemyData> GetEnemiesByLevel(int level, bool useVisibleLevel = true)
    {
        Dictionary<string, EnemyData> result = new Dictionary<string, EnemyData>();
    
        if (useVisibleLevel)
        {
            foreach (var enemy in enemyDict.Values.Where(e => e.visibleLevel.Contains(level)))
            {
                result[enemy.enemyID] = enemy;
            }
        }
        else
        {
            foreach (var enemy in enemyDict.Values.Where(e => e.homeLevel == level))
            {
                result[enemy.enemyID] = enemy;
            }
        }
    
        return result;
    }// enemyDict에 저장된 데이터 중 level에 해당하는 목록을 전부 가져오는 함수. visibleLevel이 true면 visibleLevel에 해당하는 데이터를, false면 homeLevel에 해당하는 데이터를 가져옴.

    public List<string> GetWeaponID()
    {
        List<string> enemyWeaponID = new List<string>();
        foreach (EnemyData enemy in enemyData)
        {
            enemyWeaponID.Add(enemy.weaponID);
        }
        return enemyWeaponID;
    }//enemyData에 저장된 데이터에서 weaponID들을 가져오는 함수

    public List<string> GetEnemyName()
    {
        List<string> enemyName = new List<string>();
        foreach (EnemyData enemy in enemyData)
        {
            enemyName.Add(enemy.enemyName);
        }
        return enemyName;
    }//enemyData에 저장된 데이터에서 enemyName들을 가져오는 함수

    public List<(string, Skill)> GetEnemySkillData()
    {
        List<(string, Skill)> enemySkillDataList = new List<(string, Skill)>();
        foreach (EnemyData enemy in enemyData)
        {
            foreach (Skill enemySkill in enemy.enemySkill)
            {
                enemySkillDataList.Add((enemy.enemyName, enemySkill));
            }
        }
        return enemySkillDataList;
    }//각 적의 각 스킬에 대한 정보를 가져오는 함수

    public List<Action> GetWinAction()
    {
        List<Action> winAction = new List<Action>();
        foreach (EnemyData enemy in enemyData)
        {
            winAction.AddRange(enemy.winAction);
        }
        return winAction;
    }//enemyData에 저장된 데이터에서 winAction들을 가져오는 함수
    
    public List<Action>GetLoseAction()
    {
        List<Action> loseAction = new List<Action>();
        foreach (EnemyData enemy in enemyData)
        {
            loseAction.AddRange(enemy.loseAction);
        }
        return loseAction;
    }//enemyData에 저장된 데이터에서 loseAction들을 가져오는 함수

    public List<int> GetEnemyMedStat()
    {
        List<int> enemyMedStat = new List<int>();
        foreach (EnemyData enemy in enemyData)
        {
            enemyMedStat.Add(enemy.stat.medStat);
        }
        return enemyMedStat;
    }//enemyData에 저장된 데이터에서 medStat들을 가져오는 함수

    



}
