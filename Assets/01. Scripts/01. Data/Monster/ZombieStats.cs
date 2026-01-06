using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ZombieStats", menuName = "DungBeetle/Settings/MonsterStats/ZombieStats", order = 1)]
    public class ZombieStats : MonsterStats
    {
        public AnimationClip clip;
    }
}