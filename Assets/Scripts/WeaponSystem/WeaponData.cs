using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
[System.Serializable]
public class WeaponData : ScriptableObject
{
    public enum FireMode { Auto, Burst, Single }

    [Header("Composition")]
    public Projectile _projectile;


    [Header("Weapon Mode Setup")]
    public FireMode _fireMode;
    public float _timeGap;
    public float _reloadTime = 0.2f;
    public int _burstCount;
    public int _projectilesPerMagazine;

    [Header("Recoil")]
    public Vector2 _recoilKickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 _recoilRotationMinMax = new Vector2(5f, 10f);
    public float _recoilReturnMovementTime = 0.1f;
    public float _recoilReturnRotationTime = 0.1f;
}

