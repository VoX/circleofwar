using UnityEngine;
using System.Collections;

public class FighterTypeManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        singleton = this;
    }

    static FighterTypeManager singleton;

    public FighterType[] fighters;

    static public FighterType Random()
    {
        int index = UnityEngine.Random.Range(0, singleton.fighters.Length);
        return singleton.fighters[index];
    }

    static public FighterType Lookup(string name)
    {
        foreach (var ft in singleton.fighters)
        {
            if (ft.fighterTypeName == name)
                return ft;
        }
        return null;
    }
}
