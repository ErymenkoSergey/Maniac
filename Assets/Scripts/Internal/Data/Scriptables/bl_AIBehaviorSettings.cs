using UnityEngine;

namespace MFPS.Runtime.AI
{
    [CreateAssetMenu(fileName = "AI Behavior Settings", menuName = "MFPS/AI/Behavior Settings")]
    public class bl_AIBehaviorSettings : ScriptableObject
    {
        [Header("Settings")]
        public AIAgentBehave agentBehave = AIAgentBehave.Agressive;
        public AIWeaponAccuracy weaponAccuracy = AIWeaponAccuracy.Casual;
        public bool GetRandomTargetOnStart = true;
        public bool forceFollowAtHalfHealth = true;
        public bool checkEnemysWhenHaveATarget = true;
        public AITargetOutRangeBehave targetOutRangeBehave = AITargetOutRangeBehave.KeepFollowingBasedOnState;

        [Header("Cover")]
        public float maxCoverTime = 10;
        [Tooltip("probability of get a cover point as random destination")]
        [Range(0, 1)] public float randomCoverProbability = 0.1f;
    }
}