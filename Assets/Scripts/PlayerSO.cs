using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/PlayerSO")]
[System.Serializable]

public class PlayerSO : ScriptableObject
{
    public Vector3 aimPos = new Vector3(0,0,0);
}
