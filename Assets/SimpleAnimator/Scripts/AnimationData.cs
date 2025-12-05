using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

using Random = UnityEngine.Random;

namespace AionGames.SimpleAnimatorPackage {
    // ScriptableObject that defines animation data
    [CreateAssetMenu(fileName = "AnimationData", menuName = "Simple Animator/AnimationData")]
    public class AnimationData : ScriptableObject {
	    // Custom Unity event triggered during animation transitions
	    [System.Serializable] public class AnimationEvent : UnityEvent<Animator, StateMachineLayer, AnimationData> { }
	    
	    private int ID; // Internal identifier (not used in this script)
	    public string name; // Animation name
	    [HideInInspector] public int nameID; // Hashed version of the animation name
        
        public AnimationClip animationClip; // Reference to the animation clip
        public MotionTree[] motionTree;
        
        public bool applyRootMotion = true; // Determines if root motion is applied
        public bool waitAnimation = false; // If true, the next animation must wait until this one finishes
        public bool waitAnimationCancelable = true; // If true, this animation can be interrupted
        public float speed = 1f; // Animation speed multiplier
        public bool mirror = false; // If true, the animation will be mirrored
        
        public float endTransitionTime = 0.25f; // Transition time when the animation ends
        
        // Events triggered during the animation lifecycle
        [Header("Events")]
        [Header("Triggered when the animation starts playing.")]
        public AnimationEvent onStartEvent;
        [Header("Triggered when the animation reaches its final frame.")]
        public AnimationEvent onFinishEvent;
        [Header("Triggered when the animation state is exited, either by transition or interruption.")]
        public AnimationEvent onExitEvent;
        
        
        private int previousMotionCount = 0;
        
        // Ensures nameID is updated whenever the asset is modified
        public void OnValidate() {
            nameID = Animator.StringToHash(name);
            
            //Default value
            if (motionTree.Length > previousMotionCount) {
                for (int i = previousMotionCount; i < motionTree.Length; i++) {
                    if (motionTree[i] != null)
                        motionTree[i].speed = 1f;
                }
                previousMotionCount = motionTree.Length;
            }

            if (motionTree.Length < previousMotionCount) {
                previousMotionCount = motionTree.Length;
            }
        }
        
        private SimpleAnimator simpleAnimator; // Reference to the animation system
        
        // Plays the animation using the default clip and parameters
        public bool Play(Animator animator, StateMachineLayer layer) {
            return Play(animator, layer, animationClip, speed, mirror);
        }
        
        // Plays a specific animation clip with default parameters
        public bool Play(Animator animator, StateMachineLayer layer, AnimationClip clip) {
            return Play(animator, layer, clip, speed, mirror);
        }
        
        // Plays a specific animation clip with a given speed
        public bool Play(Animator animator, StateMachineLayer layer, AnimationClip clip, float speed) {
            return Play(animator, layer, clip, speed, mirror);
        }
        
        // Plays a specific animation clip with given speed and mirroring option
        public bool Play(Animator animator, StateMachineLayer layer, AnimationClip clip, float speed, bool mirror) {
            if (layer.waitAnimation) return false; // Prevents playing if waiting for a previous animation
            
            if (simpleAnimator == null) simpleAnimator = animator.GetComponent<SimpleAnimator>();
            
            AnimationData lastAnimation = layer.animation;
            
            // Updates layer animation data
            layer.animation = this;
            layer.animationClip = clip;
            layer.speed = speed;
            layer.mirror = mirror;
            
            animator.SetInteger(layer.layerSwitchHash, layer.ASIndex);
            layer.isAnimationFinish = false;
            
            // Handles waitAnimation logic
            if (this.waitAnimation) layer.waitAnimation = true;
            else if (layer.animation != null) {
                if (layer.animation.waitAnimationCancelable) layer.waitAnimation = this.waitAnimation;
            } else layer.waitAnimation = this.waitAnimation;

            // Triggers events when transitioning animations
            if (lastAnimation != this) lastAnimation?.onExitEvent?.Invoke(animator, layer, this);
            this.onStartEvent?.Invoke(animator, layer, this);
            
            return true;
        }
        
        // Plays an animation from the motion tree by index
        public bool Play(Animator animator, StateMachineLayer layer, int clipIndex) {
            return Play(animator, layer, motionTree[clipIndex].animationClip, speed, mirror);
        }
        
        // Plays an animation from the motion tree with a specified speed
        public bool Play(Animator animator, StateMachineLayer layer, int clipIndex, float speed) {
            return Play(animator, layer, motionTree[clipIndex].animationClip, speed, mirror);
        }
        
        // Plays an animation from the motion tree with speed and mirroring options
        public bool Play(Animator animator, StateMachineLayer layer, int clipIndex, float speed, bool mirror) {
            return Play(animator, layer, motionTree[clipIndex].animationClip, motionTree[clipIndex].speed * speed, this.mirror & mirror);
        }
        
        // Plays an animation ignoring the waitAnimation flag
        public bool PlayNW(Animator animator, StateMachineLayer layer) {
            return PlayNW(animator, layer, speed, mirror);
        }
        
        public bool PlayNW(Animator animator, StateMachineLayer layer, float speed) {
            return PlayNW(animator, layer, speed, mirror);
        }
        
        public bool PlayNW(Animator animator, StateMachineLayer layer, float speed, bool mirror) {
            if (this == layer.animation) return false;
            layer.waitAnimation = false;
            return Play(animator, layer, animationClip, speed, mirror);
        }
        
        // Plays a random animation from the motion tree
        public bool PlayRandom(Animator animator, StateMachineLayer layer) {
            return PlayRandom(animator, layer, speed, mirror);
        }
        
        public bool PlayRandom(Animator animator, StateMachineLayer layer, float speed) {
            return PlayRandom(animator, layer, speed, mirror);
        }
        
        public bool PlayRandom(Animator animator, StateMachineLayer layer, float speed, bool mirror) {
            return Play(animator, layer, Random.Range(0, this.motionTree.Length), speed * this.speed, mirror);
        }
        
        // Plays the animation as a pose (no transitions, no blend tree)
        public bool PlayPose(StateMachineLayer layer) {
            if (layer.waitAnimation) return false;
            
            layer.animation = this;
            layer.animationClip = this.animationClip;
            layer.isAnimationFinish = false;
            
            return true;
        }
        
        // Overloaded Play functions with lastAnimationData (not used in logic)
        public void Play(Animator animator, StateMachineLayer layer, AnimationData lastAnimationData) {
            Play(animator, layer);
        }
        public void PlayRandom(Animator animator, StateMachineLayer layer, AnimationData lastAnimationData) {
            PlayRandom(animator, layer);
        }
        public void PlayPose(Animator animator, StateMachineLayer layer, AnimationData lastAnimationData) {
            PlayPose(layer);
        }
    }
}
