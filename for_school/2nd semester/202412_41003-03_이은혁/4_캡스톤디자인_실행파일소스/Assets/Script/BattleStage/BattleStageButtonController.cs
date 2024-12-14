using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleStageButtonController : MonoBehaviour
{
    [SerializeField] private BattleStageCanvas battleStageCanvas;
    private BattleStageEventHandler battleStageEventHandler;
    private Button finishButton;
    public GameObject logText;

    void Start()
    {
        battleStageEventHandler = FindObjectOfType<BattleStageEventHandler>();
    }

    private void ShowText(string message){

        GameObject logTextObject = Instantiate(logText, battleStageCanvas.content.transform);

        TextMeshProUGUI textPart = logTextObject.GetComponent<TextMeshProUGUI>();

        if (textPart != null)
        {
            textPart.text = message;

            // 텍스트가 변경된 후 강제로 레이아웃 갱신
            Canvas.ForceUpdateCanvases();

            RectTransform rt = logTextObject.GetComponent<RectTransform>();

            // LayoutRebuilder를 사용하여 강제로 레이아웃 재계산
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

            // 텍스트의 필요한 높이를 가져오기
            float preferredHeight = textPart.preferredHeight;
            Debug.Log("texts height is:" + preferredHeight);

            LayoutElement layoutElement = logTextObject.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.preferredHeight = preferredHeight;
            }
            
        }


        // 스크롤 위치 조정 (맨 아래로)
        ScrollRect scrollRect = battleStageCanvas.GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }// 특정 문장을 길이를 반영하여 view port에 보여주는 함수

    public void SetBattleLog(Dictionary<string, float> battleLogToShow, float finalPercentage)
    {
        foreach (var log in battleLogToShow)
        {
            ShowText(log.Key + " == " + log.Value);
        }
        battleStageCanvas.buttonGroup.SetActive(true);
        finishButton = battleStageCanvas.buttonGroup.transform.Find("FinishButton").GetComponent<Button>();
        if (finishButton != null)
        {
            // Get the TextMeshPro component of the FinishButton
            TextMeshProUGUI finishButtonText = finishButton.GetComponentInChildren<TextMeshProUGUI>();
            finishButtonText.text = finalPercentage.ToString() + "%";
            
            // Add a listener to the FinishButton
            finishButton.onClick.AddListener(() =>
            {
                openWinLossPanel();
            });
            
        }
        else
        {
            Debug.LogError("FinishButton not found in buttonGroup.");
        }
    }// battleLogToShow에 저장된 데이터를 view port에 보여주는 함수

    private void openWinLossPanel()//when PopUp is opened
    {
        battleStageCanvas.winLossPanel.gameObject.SetActive(true);

        TextMeshProUGUI winLossText = battleStageCanvas.winLossPanel.GetComponentInChildren<TextMeshProUGUI>();
        Button okButton = battleStageCanvas.winLossPanel.transform.Find("OkButton").GetComponent<Button>();

        bool didUserWin = battleStageEventHandler.DidUserWin();

        List<string> resultText = battleStageEventHandler.ResultHandler(didUserWin);

        if(didUserWin)
        {
            resultText.Insert(0, "You Win!");
        }
        else
        {
            resultText.Insert(0, "You Lose!");
        }
        winLossText.text = string.Join("\n",resultText);

        okButton.onClick.AddListener(() =>
        {
            battleStageEventHandler.FinishBattleStage();
        });
    }//승패 패널 여는 함수
    
}
