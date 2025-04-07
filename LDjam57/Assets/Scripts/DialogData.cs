using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Scriptable Objects/Dialog Data")]
public class DialogData : ScriptableObject
{
    public List<string> lines;
}