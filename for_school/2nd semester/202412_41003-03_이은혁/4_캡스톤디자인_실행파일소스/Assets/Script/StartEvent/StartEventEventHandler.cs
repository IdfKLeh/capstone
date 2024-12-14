using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using StartEventNameSpace;
using UnityEngine.SceneManagement;

public class StartEventEventHandler : MonoBehaviour
{
    public List<StartEvent> startEvent = new List<StartEvent>();
    private StartEvent currentEvent;
    private UserController userController;
    private int currentDialogueIndex = 0;
    public int maxStage=-1;

    void Awake()
    {
        userController = FindObjectOfType<UserController>();
        
        LoadStartEvents("StartEventList.json");
        SetEvent(0);
    }//json파일에서 이벤트 리스트를 불러오고, 해당 리스트 중 특정 스테이지에 있는 이벤트만 저장하여 해당 이벤트 중 특정 이벤트를 currentevent로 설정함.

    void Start()
    {
        MakeNewSaveFile();
    }//Awake때문에 UserData가 꼬이는걸 방지하기 위해 Start에서 새로운 게임 시작을 만들어놈.


    void LoadStartEvents(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        Debug.Log("Loading events from: " + filePath);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log("JSON content: " + json);

            Wrapper<StartEvent> wrapper = JsonUtility.FromJson<Wrapper<StartEvent>>(json);
            if (wrapper != null)
            {
                startEvent = wrapper.events;
                Debug.Log("Events loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to parse JSON.");
            }
        }
        else
        {
            Debug.LogError("Cannot find file: " + filePath);
        }

        // 스테이지별로 이벤트 그룹화
        Dictionary<int, List<StartEvent>> eventsByStage = new Dictionary<int, List<StartEvent>>();
        foreach (StartEvent e in startEvent)
        {
            if (!eventsByStage.ContainsKey(e.stage))
            {
                eventsByStage[e.stage] = new List<StartEvent>();
            }
            eventsByStage[e.stage].Add(e);
        }

        // 가장 높은 스테이지 찾기
        foreach (int stage in eventsByStage.Keys)
        {
            if (stage > maxStage)
            {
                maxStage = stage;
            }
        }
    }//json에서 이벤트들을 읽어 startEvent에 저장하고, 마지막 스테이지의 수를 저장해두는 함수

    void SetEvent(int stage)
    {
        List<StartEvent> events = GetEventsByStage(stage);
        if (events != null && events.Count > 0)
        {
            SetCurrentEvent(events[GetRandomEventNumber(events.Count)]);
        }
        else
        {
            Debug.LogError("No events found for stage " + stage);
        }
    }//현재 이벤트로 특정 스테이지의 이벤트 중 랜덤한 걸 뽑아 설정하는 함수

    List<StartEvent> GetEventsByStage(int stage)
    {
        if (startEvent == null)
        {
            Debug.LogError("startEvent list is null.");
            return null;
        }
        Debug.Log("Finding events for stage: " + stage);
        List<StartEvent> eventsForStage = new List<StartEvent>();

        foreach (StartEvent e in startEvent)
        {
            if (e == null)
            {
                Debug.LogWarning("Encountered a null StartEvent object.");
                continue;
            }

            if (e.stage == stage)
            {
                eventsForStage.Add(e);
            }
        }

        return eventsForStage;
    }//대상 startEvent에서 stage에 따라 특정 이벤트들 반환하는 함수

    void SetCurrentEvent(StartEvent gameEvent)
    {
        if (gameEvent == null)
        {
            Debug.LogError("The gameEvent is null.");
            return;
        }
        currentEvent = gameEvent;
        currentDialogueIndex = 0;
        SelectRandomOptions();
        Debug.Log("Current event set: " + currentEvent.stageDescription);
    }//대상 StartEvent를 currentEvent로 설정하고 dialogueIndex를 설정하는 함수

    void SelectRandomOptions()
    {
        currentEvent.selectedOptions = new List<Option>();
        List<int> selectedIndices = new List<int>();
        int optionsCount = currentEvent.allOptions.Count;

        for (int i = 0; i < currentEvent.optionNumber; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, optionsCount);
            } while (selectedIndices.Contains(randomIndex));
            selectedIndices.Add(randomIndex);
            currentEvent.selectedOptions.Add(currentEvent.allOptions[randomIndex]);
        }
    }//해당 이벤트의 모든 옵션 중 표시할 개수 뽑기
    public string GetNextDialogue()
    {
        if (currentEvent != null && currentDialogueIndex < currentEvent.dialogues.Count)
        {
            return currentEvent.dialogues[currentDialogueIndex++];
        }
        return null;
    }//currentEvent의 다음 dialogue를 가져오는 함수

    public bool HasMoreDialogues()
    {
        return currentEvent != null && currentDialogueIndex < currentEvent.dialogues.Count;
    }//currentEvent에 다음 dialogue가 있는지 확인하는 함수

    public int GetNumberOfOptions(){
        return currentEvent.selectedOptions.Count;
    }//currentEvent에 존재하는 option수를 반환하는 함수

    public string GetOptionDialogue(int optionIndex)
    {
        if (optionIndex >= 0 && optionIndex < currentEvent.selectedOptions.Count)
        {
            return currentEvent.selectedOptions[optionIndex].optionText;
        }
        return null;
    }//option에 넣을 dialogue를 반환하는 함수

    public void OnOptionSelected(int optionIndex)
    {
        if (optionIndex >= 0 && optionIndex < currentEvent.selectedOptions.Count)
        {
            ExecuteAction(currentEvent.stage, currentEvent.selectedOptions[optionIndex].action);
        }
    }//선택한 option의 action을 실행하는 함수

    private int GetRandomEventNumber(int eventListCount)
    {
        return Random.Range(0, eventListCount);
    }//주어진 이벤트 수 중에서 랜덤한 번호 출력

    private void MakeThisRunSeed()
    {
        long ticks = System.DateTime.Now.Ticks;

        // System.Random을 사용하여 난수 생성
        System.Random random = new System.Random((int)(ticks & 0xFFFFFFFF) ^ (int)(ticks >> 32));
        int seed = random.Next();

        // Unity Random 시드 설정
        Random.InitState(seed);
        userController.SetSeed(seed);
        
    }//시드를 만들고 유저파일에 저장하는 함수.

    public void SetThisRunSeed(){
        
        Random.InitState(userController.GetSeed());

    }//로드 시 등 필요하다면 저장된 파일에서 시드를 불러와 랜덤함수에 설정하는 함수

    private void MakeNewSaveFile(){
        userController.NewGame();
    }//새로운 세이브 파일을 생성하는 함수. StartEvent 씬 Awake시 호출되기 때문에 세이브 파일 관련하여 해야할 사항이 있다면 이 함수에 넣자

    void ExecuteAction(int stage, string action)
    {
        List<string> weaponList = new List<string>();
        switch (stage)
        {
            case 0:
                switch (action)
                {
                    case "Main Phy":
                        PickedAsMain("phyStat");
                        break;
                    case "Main Int":
                        PickedAsMain("intStat");
                        break;
                    case "Main Sta":
                        PickedAsMain("staStat");
                        break;
                    case "Main Med":
                        PickedAsMain("medStat");
                        break;
                    case "Main Spe":
                        PickedAsMain("speStat");
                        break;
                    case "Main Wea":
                        PickedAsMain("weaStat");
                        break;
                    default:
                        Debug.LogWarning("Unknown action: " + action);
                        break;
                }
                break;
            case 1:
                switch (action)
                {
                    case "Sub Phy":
                        PickedAsSub("phyStat");
                        break;
                    case "Sub Int":
                        PickedAsSub("intStat");
                        break;
                    case "Sub Sta":
                        PickedAsSub("staStat");
                        break;
                    case "Sub Med":
                        PickedAsSub("medStat");
                        break;
                    case "Sub Spe":
                        PickedAsSub("speStat");
                        break;
                    case "Sub Wea":
                        PickedAsSub("weaStat");
                        break;
                    default:
                        Debug.LogWarning("Unknown action: " + action);
                        break;
                }
                break;
            case 2:
                switch(action)
                {
                    case "Weapon Broadsword":
                        weaponList.Add("rustyBroadSword");
                        break; 
                    case "Weapon Dagger":
                        weaponList.Add("rustyDagger");
                        break;
                    case "Weapon Fist":
                        weaponList.Add("bareFist");
                        break; 
                }
                userController.SetMainWeapon(weaponList);
                break;
            case 3:
                break;
            default:
                Debug.LogWarning("Unknown stage:"+ stage);
                break;
        }
        if (IsThisLastEvent()){
            FinalStartEventAction();
            userController.SaveData();
        }
        if(stage != maxStage){
            SetEvent(stage + 1);
        }
        
    }//option 선택시 해당 액션 실행하는 함수. stage 0의 경우 main 고르기 단계.
    //stage 1는 sub 고르기 단계.

    void FinalStartEventAction()
    {
        userController.SetLevelCounter(1);
        userController.SetStageCounter(0);
        userController.SetRandomNextEnemy();
    }//마지막 이벤트에서 실행되는 함수. 이후 MainPlay 씬으로 넘어감.
    void PickedAsMain(string pickedStat)
    {
        userController.SetMainStat(pickedStat);
    }//userController에서 메인 스탯에 수를 더하고 메인 스탯으로 지정.

    void PickedAsSub(string pickedStat)
    {
        userController.SetSubStat(pickedStat);
    }//userController에서 서브 스탯에 수를 더하고 서브 스탯으로 지정.

    public bool IsThisLastEvent()
    {
        // 현재 이벤트가 null인지 확인
        if (currentEvent == null)
        {
            Debug.LogWarning("Current event is null.");
            return false;
        }

        // startEvent 리스트가 null인지 확인
        if (startEvent == null)
        {
            Debug.LogError("startEvent list is null.");
            return false;
        }

        // 현재 이벤트의 스테이지가 가장 높은 스테이지인지 확인
        return currentEvent.stage == maxStage;
    }//현재 이벤트의 스테이지가 가장 높은 스테이지인지 확인하는 함수. 이벤트가 전환되기 전에 확인해야겠지?

    public void EndStartEvent(string targetlevel)
    {
        userController.SetStageBefore("StartEvent", "Neutral");
        userController.SaveData();
        SceneManager.LoadScene(targetlevel);
    }
}

[System.Serializable]
public class Wrapper<T>
{
    public List<T> events;
}//json 변환을 위한 wrapper
