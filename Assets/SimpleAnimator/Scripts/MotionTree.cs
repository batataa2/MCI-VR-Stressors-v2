using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    [System.Serializable]
    public class MotionTree {
        public AnimationClip animationClip;
        public Vector2 pos;
        public float speed = 1f;
        public bool mirror = false;
        
        public MotionTree() {
            speed = 1f;
        }
    }
}
