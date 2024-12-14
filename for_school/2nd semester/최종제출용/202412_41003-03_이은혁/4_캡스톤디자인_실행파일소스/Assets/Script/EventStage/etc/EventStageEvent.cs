namespace EventStageEventNameSpace
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Newtonsoft.Json;

    public class EventStageEvent
    {
        public string eventID { get; set; }
        public string conditions { get; set; }
        public string eventType { get; set; }
        public string eventKarma { get; set; }
        public int basicOptionNumber { get; set; }
        public List<Dialogue> dialogues { get; set; }
        public List<Option> allOptions { get; set; }
    }

    public class Dialogue
    {
        public string text { get; set; }
        public List<Restriction> restriction { get; set; }
    }

    public class Option
    {
        public List<Restriction> restriction { get; set; }
        public string text { get; set; }
        public List<Action> action { get; set; }
        public List<Dialogue> afterText { get; set; }
        public string afterOptionText { get; set; }
    }

    public class Restriction
    {
        public string type {get; set;}
        public string stats { get; set; }
        public int amount { get; set; }  
    }

    public class Action
    {
        public string type { get; set; }
        public string item { get; set; }
        public string stats { get; set; }
        public int amount { get; set; }
    }

    public class Root
    {
        public List<EventStageEvent> events { get; set; }
    }
}