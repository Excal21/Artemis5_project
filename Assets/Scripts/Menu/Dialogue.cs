using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Dialogue
{
    public string characterName;
    public string text;
    public string characterAvatar;
}

[Serializable]
public class DialogueData
{
    public string opening;
    public List<Dialogue> dialogues;
}
