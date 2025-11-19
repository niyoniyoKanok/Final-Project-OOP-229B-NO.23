using UnityEngine;

[CreateAssetMenu(fileName = "PathData", menuName = "Scriptable Objects/PathData")]
public class PathData : ScriptableObject
{
    public PathType pathType; 
    public string pathName;
    public Sprite pathIcon; 
    [TextArea] public string pathDescription;

    [Header("Skills")]
    public string skill1Name;
    public Sprite skill1Icon;
    [TextArea] public string skill1Desc;

    public string skill2Name;
    public Sprite skill2Icon;
    [TextArea] public string skill2Desc;

    public string skill3Name;
    public Sprite skill3Icon;
    [TextArea] public string skill3Desc;
}
