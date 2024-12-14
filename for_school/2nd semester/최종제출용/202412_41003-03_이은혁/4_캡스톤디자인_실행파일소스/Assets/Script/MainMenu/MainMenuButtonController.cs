using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenuButtonController : MonoBehaviour
{
    [SerializeField] private MainMenuCanvas mainMenuCanvas; //creating Canvas instance
    private UserController userController;

    private void Awake()
    {   
        mainMenuCanvas.newGameButton.onClick.AddListener(() => onButtonClicked("NewGame"));
        mainMenuCanvas.loadGameButton.onClick.AddListener(() => onButtonClicked("LoadGame"));
        mainMenuCanvas.settingsButton.onClick.AddListener(() => onButtonClicked("Settings"));
        mainMenuCanvas.quitButton.onClick.AddListener(() => onButtonClicked("Quit"));
        userController = FindObjectOfType<UserController>();

    }//시작시에 각 버튼들에 리스너 추가
    private void onButtonClicked(string operType)
    {
        switch (operType)
        {
            case "NewGame":
                openPopUpWindow("Are You Sure?(Save Files will be replaced!)", operType);
                break;
            case "LoadGame":
                openPopUpWindow("Are You Sure?", operType);
                break;
            case "Settings":
                
                break;
            case "Quit":
                openPopUpWindow("Are You Sure?", operType);
                break;
            default:
                openPopUpWindow("Are You Sure?", operType);
                break;
        }
    }//각 버튼별 발생하는 액션
    private void openPopUpWindow(string message, string operType)
    {
        mainMenuCanvas.popUpWindow.gameObject.SetActive(true);

        //기존 리스너 삭제
        mainMenuCanvas.popUpWindow.yesButton.onClick.RemoveAllListeners();
        mainMenuCanvas.popUpWindow.noButton.onClick.RemoveAllListeners();

        //리스너 추가
        mainMenuCanvas.popUpWindow.yesButton.onClick.AddListener(() => onYesButtonClicked(operType));
        mainMenuCanvas.popUpWindow.noButton.onClick.AddListener(() => onNoButtonClicked(operType));
        mainMenuCanvas.popUpWindow.messageText.text = message;
    }//Popup 여는 함수

    private void onYesButtonClicked(string operType)//when clicking the yes button
    {
        switch (operType)
        {
            case "NewGame":
                mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
                sceneChange("StartEvent");
                break;
            case "LoadGame":
                mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
                userController.LoadData();
                string sceneBefore = userController.LoadGameSceneInfoFunc();
                switch (sceneBefore)
                {
                    case "StartEvent":
                        sceneChange("MainPlay");
                        break;
                    case "TrainingStage":
                        sceneChange("EventStage");
                        break;
                    case "BattleSelection":
                        sceneChange("BattleStage");
                        break;
                    case "BattleStage":
                        sceneChange("EventStage");
                        break;
                    case "EventStage":
                        sceneChange("MainPlay");
                        break;
                    default:
                        sceneChange("StartEvent");
                        break;
                }
                break;
            case "Quit":
                //UnityEditor.EditorApplication.isPlaying = false;// 빌드 파일에선 해당 코드 없애기
                Application.Quit();
                break;
            default:
                mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
                Debug.Log("yes clicked");
                break;
        }
        
        
    }

    private void onNoButtonClicked(string operType)//when clicking the no button
    {
        switch (operType)
        {
            case "NewGame":
                mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
                break;
            case "LoadGame":
                mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
                break;
            case "Quit":
                mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
                break;
            default:
                mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
                Debug.Log("No clicked");
                break;
        }
        mainMenuCanvas.popUpWindow.gameObject.SetActive(false);
        Debug.Log("no clicked");
    }

    public void sceneChange(string targetLevel)//fuction for Scene Change through button clicks
    {
        SceneManager.LoadScene(targetLevel);
    }
}
