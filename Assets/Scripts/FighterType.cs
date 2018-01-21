using System.Collections;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "FighterType", menuName = "FighterType", order = 1)]
public class FighterType : ScriptableObject
{
    public string fighterTypeName;

    public int maxHealth = 100;
    public int maxEnergy = 100;
    public float sprintRate = 0.5f;
    public float energyRegenRate = 0.2f;
    public int sprintEnergy = 7;
    public float acceleration = 0.04f;
    public Sprite skinBody;
}
