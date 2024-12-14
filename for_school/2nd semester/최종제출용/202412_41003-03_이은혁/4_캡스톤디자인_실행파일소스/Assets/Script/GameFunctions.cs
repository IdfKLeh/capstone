using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFunctions : MonoBehaviour
{
    public static bool IsSuccessful(float successRate){
        //Debug.Log("Current Rate: "+ successRate);
        if (successRate <= 0)
        {
            return false;
        }
        if (successRate >= 100)
        {
            return true;
        }

        // 0과 100 사이의 난수를 생성하여 successRate와 비교
        float randomValue = Random.Range(0f, 100f);
        //Debug.Log("randomValue: "+randomValue);
        return randomValue < successRate;
    }// 확률값을 받아 해당 행동이 성공했는지를 반환하는 함수

    public static string GetRandomSelection(Dictionary<string, float> itemProbabilities){
        // 아이템과 확률의 쌍을 딕셔너리로 받아옵니다.
        // itemProbabilities 예시: {"Sword": 50.0f, "Shield": 30.0f, "Potion": 20.0f}

        float totalProbability = 0f;
        foreach (var probability in itemProbabilities.Values)
        {
            totalProbability += probability;
        }

        // 누적 확률을 계산합니다.
        float cumulativeProbability = 0f;
        Dictionary<string, float> cumulativeProbabilities = new Dictionary<string, float>();

        foreach (var kvp in itemProbabilities)
        {
            cumulativeProbability += kvp.Value;
            cumulativeProbabilities[kvp.Key] = cumulativeProbability / totalProbability; // 전체 1로 정규화
        }

        // 0과 1 사이의 난수를 생성합니다.
        float randomValue = UnityEngine.Random.Range(0f, 1f);

        // 난수와 누적 확률을 비교하여 해당 아이템을 선택합니다.
        foreach (var kvp in cumulativeProbabilities)
        {
            if (randomValue <= kvp.Value)
            {
                return kvp.Key;
            }
        }

        // 안전망으로 첫 번째 아이템을 반환 (이론적으로는 이 줄에 도달할 수 없습니다).
        return itemProbabilities.Keys.GetEnumerator().Current;
    }//타겟들의 리스트와 각 확률을 딕셔너리로 전달하면, 하나를 선택해서 주는 함수.


}
