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
    public int sprintEnergy = 7;
    public int energyRegen = 2;

    public float rotateSpeed = 80f;
    public float acceleration = 0.04f;
    public float topRunSpeed = 3.0f;
    public float topSprintSpeed = 4.0f;
    public float speed = 0.0f;

    public Sprite skinBody;
}
