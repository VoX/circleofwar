using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AmmoUI : NetworkBehaviour
{
    [SerializeField]
    public Text gunText;

    [SerializeField]
    public Text ammoText;
}
