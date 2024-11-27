using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Dialogue
{
    public string characterName;
    public string text;
    public string characterAvatar;
    public string side;
}

[Serializable]
public class DialogueData
{
    public string opening;
    public List<Dialogue> dialogues;
}
