using UnityEngine;


[CreateAssetMenu()]
public class CardUpgradeSO : ScriptableObject, IEquipable
{
    public string Name;
    public string description;
    public Sprite sprite;
    public bool isUpgrade = true;
    public bool isDowngrade = false;


    string IEquipable.Name => Name;

    bool IEquipable.isUpgrade => isUpgrade;
    bool IEquipable.isDowngrade => isDowngrade;
}
