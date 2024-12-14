
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private List<WeaponData> userWeaponData = new List<WeaponData>();
    private List<WeaponData> friendWeaponData = new List<WeaponData>();
    private List<WeaponData> enemyWeaponData = new List<WeaponData>();
    private UserController userController;
    private Dictionary<string, WeaponData> weaponDict = new Dictionary<string, WeaponData>();


    public void LoadAllWeapons(List<string> userWeaponID, List<string> friendWeaponID, List<string> enemyWeaponID)
    {
        LoadWeaponList();
    
        // Step 2: Add matching WeaponData to userWeaponData if userWeaponID is not empty
        if (userWeaponID != null && userWeaponID.Count > 0)
        {
            foreach (string id in userWeaponID)
            {
                if (weaponDict.TryGetValue(id, out WeaponData weapon))
                {
                    userWeaponData.Add(weapon);
                }
            }
        }
    
        // Step 3: Add matching WeaponData to friendWeaponData if friendWeaponID is not empty
        if (friendWeaponID != null && friendWeaponID.Count > 0)
        {
            foreach (string id in friendWeaponID)
            {
                if (weaponDict.TryGetValue(id, out WeaponData weapon))
                {
                    friendWeaponData.Add(weapon);
                }
            }
        }
    
        // Step 4: Add matching WeaponData to enemyWeaponData if enemyWeaponID is not empty
        if (enemyWeaponID != null && enemyWeaponID.Count > 0)
        {
            foreach (string id in enemyWeaponID)
            {
                if (weaponDict.TryGetValue(id, out WeaponData weapon))
                {
                    enemyWeaponData.Add(weapon);
                }
            }
        }
    }//weaponDict에 저장된 데이터를 userWeaponData, friendWeaponData, enemyWeaponData에 저장하는 함수

    private void LoadWeaponList()
    {
        string weaponDataPath = Path.Combine(Application.streamingAssetsPath, "Items", "Weapon.json");
        Debug.Log("Loading weapons from:" + weaponDataPath);

        if (File.Exists(weaponDataPath))
        {
            string dataAsJson = File.ReadAllText(weaponDataPath);
            var wrapper = JsonConvert.DeserializeObject<WeaponDataWrapper>(dataAsJson);
            if (wrapper != null && wrapper.weapon != null)
            {
                foreach (var weapon in wrapper.weapon)
                {
                    if (!weaponDict.ContainsKey(weapon.weaponID))
                    {
                        weaponDict[weapon.weaponID] = weapon;
                    }
                }
                Debug.Log("Weapons loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to parse JSON.");
            }
        }
        else
        {
            Debug.LogError("Cannot load weapon data!");
        }
    }//weapon.json 파일을 읽어서 weaponDict에 저장하는 함수

    public List<WeaponData> GetUserWeaponData()
    {
        Debug.Log("User Weapon Data: " + userWeaponData[0].weaponName);
        return userWeaponData;
    }

    public List<WeaponData> GetFriendWeaponData()
    {
        try
        {
            Debug.Log("Friend Weapon Data: " + friendWeaponData[0].weaponName);
        }
        catch
        {
            Debug.LogError("Friend Weapon Data is empty");

        }
        return friendWeaponData;
    }

    public List<WeaponData> GetEnemyWeaponData()
    {
        Debug.Log("Enemy Weapon Data: " + enemyWeaponData[0].weaponName);
        return enemyWeaponData;
    }//위의 3개는 순서대로 userWeaponData, friendWeaponData, enemyWeaponData를 반환하는 함수



}
