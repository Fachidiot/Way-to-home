using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NPCDialogue", menuName = "Settings/Dialogue/NPC", order = 1)]
    public class NPCDialogue : ScriptableObject
    {
        [Header("Interactables")]
        public string npcName;
        public string interactPrompt;
    }
}