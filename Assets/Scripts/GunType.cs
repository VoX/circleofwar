using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GunType", menuName = "GunType", order = 2)]
public class GunType : ScriptableObject
{
    public string gunTypeName;

    public int startMaxAmmo;
    public int extMaxAmmo;

    public float fireRate;
    public float shotSpeed;
    public bool spread;

    public int damage;
}
