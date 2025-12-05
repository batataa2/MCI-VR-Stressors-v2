using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    public class StateMachineLayer{
        public AnimationData animation;
        public AnimationClip animationClip;
        public float speed;
        public bool mirror;
        
        public int index {get;}
        public int layerNameHash {get;}
        public int layerSwitchHash {get;}
        public int S1SpeedHash {get;}
        public int S1MirrorHash {get;}
        public int S2SpeedHash {get;}
        public int S2MirrorHash {get;}
        public string layerStateName{get;}
        public int layerStateNameHash{get;}
        public int ASIndex = 1;
        public bool waitAnimation = false;
        public bool isAnimationFinish = true;
        public Vector2 motionPos;

        public bool WaitAnimation {
            get{ return waitAnimation;}
            set {
                if (value) waitAnimation = value;
                else if (animation.waitAnimationCancelable) waitAnimation = value;
            }
        }
        
        public StateMachineLayer(int index, string layerName) {
            this.index = index;
            this.layerNameHash = Animator.StringToHash(layerName);
            this.layerSwitchHash = Animator.StringToHash(layerName + "_switch");
            this.S1SpeedHash = Animator.StringToHash(layerName + "_S1_speed");
            this.S1MirrorHash = Animator.StringToHash(layerName + "_S1_mirror");
            this.S2SpeedHash = Animator.StringToHash(layerName + "_S2_speed");
            this.S2MirrorHash = Animator.StringToHash(layerName + "_S2_mirror");
            this.layerStateName = layerName + " S";
            this.layerStateNameHash = Animator.StringToHash(this.layerStateName);
        }
    }
}