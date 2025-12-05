using UnityEngine;

namespace AionGames.SimpleAnimatorPackage{
    
    [CreateAssetMenu(fileName = "AnimationStatusList", menuName = "Simple Animator/AnimationStatusList")]
    public class AnimationStatusList : ScriptableObject{
        public AnimationStatus[] status;
        
        // Automatically update animation status IDs when modified in the editor
        private void OnValidate(){
            if (status != null){
                foreach (AnimationStatus statusField in status){
                    statusField.ID = Animator.StringToHash(statusField.name);
                }
            }
        }
    }
}