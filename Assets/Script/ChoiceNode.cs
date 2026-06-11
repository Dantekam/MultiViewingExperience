using UnityEngine;

[System.Serializable]
public class ChoiceNode
{
    public string nodeId;

    public VideoEntry video;

    public string choiceAText;
    public string choiceBText;

    public ChoiceNode nextA;
    public ChoiceNode nextB;
}