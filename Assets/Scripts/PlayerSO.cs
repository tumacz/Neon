using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/PlayerSO")]
[System.Serializable]

public class PlayerSO : ScriptableObject
{
    public Vector3 aimPos = new Vector3(0,0,0);

    public Vector3 GetMousePosition()
    {
        return aimPos;
    }

    public void Test()
    {
        Debug.Log("testowanko");
    }
}
