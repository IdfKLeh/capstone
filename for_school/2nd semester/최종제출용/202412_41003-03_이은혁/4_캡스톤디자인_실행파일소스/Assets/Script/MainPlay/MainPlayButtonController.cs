using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class MainPlayButtonController : MonoBehaviour
{

    [SerializeField] private MainPlayCanvas mainPlayCanvas;
    private MainPlayEventHandler mainPlayEventHandler;
    List<Button> buttons = new List<Button>();
    List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();
    private GameObject currentActiveGroup;//현재 화면에 활성화된 애들을 확인하는 코드.
    private void Start()
    {
        mainPlayEventHandler = FindObjectOfType<MainPlayEventHandler>();
        OpenMain();
    }//시작시엔 메인 화면을 열음.

    public void OpenMain()
    {
        EnableUIGroup(mainPlayCanvas.mainStuff);
    }//메인 화면을 여는 함수.

    public void OpenTraining()
    {
        EnableUIGroup(mainPlayCanvas.trainingStuff);
    }//training 화면을 여는 함수

    private void EnableUIGroup(GameObject group)
    {
        if (currentActiveGroup != null)
        {
            currentActiveGroup.SetActive(false);
        }

        ClearButtons();
        // mainStuff 하위의 모든 버튼을 찾음
        Button[] allButtons = group.GetComponentsInChildren<Button>();

        // 찾은 버튼들을 리스트에 추가
        foreach (Button btn in allButtons)
        {
            buttons.Add(btn);
            TextMeshProUGUI textComponent = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                buttonTexts.Add(textComponent);
            }
        }

        group.SetActive(true);
        currentActiveGroup = group;

        for(int i=0; i<buttons.Count;i++)
        {
            string buttonName = buttons[i].name;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnOptionSelection(buttonName,group));
        }


    }//특정 화면의 그룹을 활성화 하는 함수. 호출시 활성화되어 있는 그룹이 있다면 비활성화 하고 진행.

    private void ClearButtons()
    {
        buttons.Clear();
        buttonTexts.Clear();
    }//현재 버튼 리스트에 들어있는 버튼 목록들을 해제하는 함수.

    private void OnOptionSelection(string buttonName, GameObject group)
    {
        switch (group.name)
        {
            case "MainStuff":
                switch (buttonName)
                {
                    case "TrainingButton":
                        OpenTraining();
                        break;
                    case "BattleButton":
                        OpenPopUpWindow("Are you sure?",buttonName,group);
                        break;
                    case "OthersButton":
                        break;
                    case "InfoButton":
                        break;
                    default:
                        Debug.LogError("Unknown Button Name" + buttonName);
                        break;
                }
                break;
            case "TrainingStuff":
                OpenPopUpWindow("Are you sure?",buttonName,group);
                break;
        }
    }//옵션 선택시 어떤 행동을 할지 정하는 함수. 만약 화면내의 변화만 존재한다면 여기에서 해결하고, 팝업을 띄워야 한다면 해당 팝업의 yes no 버튼에서 MainPlayEventHandler로 전달하게 함.
    //주의 할 점은 개발 과정에서 얼마나의 정보가 필요할지 몰라서 buttonName과 group을 모두 전달하게끔 했는데, 나중에 그럴 필요가 없다면 group은 없애줘도 된다. 내부 switch문에서 group을 이미 관리중이기 때문.
    //만약 그렇게 할거라면 yes no 버튼에서 switch를 늘려줘야할 수도 있다.
    //그게 아니면 그냥 나머지 case를 trainingstuff, 뭐시기 로 다 추가하지 말고 걍 째도 될듯.

    private void OpenPopUpWindow(string message,string buttonName, GameObject group)
    {
        mainPlayCanvas.popUpWindow.gameObject.SetActive(true);
        mainPlayCanvas.popUpWindow.yesButton.onClick.RemoveAllListeners();
        mainPlayCanvas.popUpWindow.noButton.onClick.RemoveAllListeners();
        mainPlayCanvas.popUpWindow.yesButton.onClick.AddListener(() => OnYesButtonClicked(buttonName, group.name));
        mainPlayCanvas.popUpWindow.noButton.onClick.AddListener(OnNoButtonClicked);
        mainPlayCanvas.popUpWindow.messageText.text = message;
    }//popup여는 함수

    public void OpenGameOver()
    {
        mainPlayCanvas.popUpWindow.gameObject.SetActive(true);
        mainPlayCanvas.popUpWindow.yesButton.onClick.RemoveAllListeners();
        mainPlayCanvas.popUpWindow.noButton.onClick.RemoveAllListeners();
        mainPlayCanvas.popUpWindow.yesButton.onClick.AddListener(() => OnYesButtonClicked("GameOverButton","MainStuff"));
        mainPlayCanvas.popUpWindow.noButton.onClick.AddListener(() => OnYesButtonClicked("GameOverButton", "MainStuff"));
        mainPlayCanvas.popUpWindow.messageText.text = "Your health is 0. Game Over!";
    }//게임오버 팝업을 여는 함수.

    private void OnYesButtonClicked(string buttonName, string groupName)
    {
        mainPlayCanvas.popUpWindow.gameObject.SetActive(false);
        switch (groupName)
        {
            case "TrainingStuff":
                mainPlayEventHandler.ExecuteTrainingAction(buttonName);
                break;
            case "MainStuff":
                switch (buttonName)
                {
                    case "BattleButton":
                        mainPlayEventHandler.ExecuteBattleAction();
                        break;
                    case "GameOverButton":
                        mainPlayEventHandler.EndGameAction();
                        break;
                }
                break;
        }
        
    }//pop의 yes가 눌렸을 때

    private void OnNoButtonClicked()
    {
        mainPlayCanvas.popUpWindow.gameObject.SetActive(false);
    }//popup의 no가 눌렸을 때

}
