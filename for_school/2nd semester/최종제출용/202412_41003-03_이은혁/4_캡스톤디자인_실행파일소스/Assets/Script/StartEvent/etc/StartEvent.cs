namespace StartEventNameSpace
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class StartEvent
    {
        public int stage;
        public string stageDescription;
        public string eventType;
        public int optionNumber;
        public List<string> dialogues;
        public List<Option> allOptions;
        [NonSerialized] public List<Option> selectedOptions;
    }

    [System.Serializable]
    public class Option
    {
        public string optionText;
        public string action;
    }
//선택지에 들어가는 내용들
}