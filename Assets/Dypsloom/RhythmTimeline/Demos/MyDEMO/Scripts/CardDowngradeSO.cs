using UnityEngine;


[CreateAssetMenu()]
public class CardDowngradeSO : ScriptableObject, IEquipable
{
    public string Name;
    public string description;
    public Sprite sprite;
    public bool isUpgrade = false;
    public bool isDowngrade = true;


    string IEquipable.Name => Name;
    bool IEquipable.isUpgrade => isUpgrade;
    bool IEquipable.isDowngrade => isDowngrade;
}
