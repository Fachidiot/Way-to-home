using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Settings/Player/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Health")]
    public float maxHp;
    public float defense;
}