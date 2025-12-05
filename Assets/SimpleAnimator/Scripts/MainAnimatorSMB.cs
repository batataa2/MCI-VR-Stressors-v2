using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    public class MainAnimatorSMB : StateMachineBehaviour {
        SimpleAnimator simpleAnimator;
        
        float currentAnimationTime;
        float endTransitionTime;
        AnimationData currentAnimation;
        AnimationLayer _animationLayer;
        StateMachineLayer stateMachineLayer;
        
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (simpleAnimator == null) simpleAnimator = animator.GetComponent<SimpleAnimator>();
            if (simpleAnimator.stateMachineLayers.Count == 0) return;
            
            stateMachineLayer = simpleAnimator.stateMachineLayers[layerIndex];

            
            int ASIndex = animator.GetInteger(stateMachineLayer.layerSwitchHash);
            
            simpleAnimator.overrideController[stateMachineLayer.layerStateName + stateMachineLayer.ASIndex] = stateMachineLayer.animationClip;
            simpleAnimator.animator.runtimeAnimatorController = simpleAnimator.overrideController;
            
            animator.applyRootMotion = stateMachineLayer.animation.applyRootMotion;

            currentAnimation = stateMachineLayer.animation;
            float finalAnimSpeed = stateMachineLayer.speed;
            currentAnimationTime = (stateMachineLayer.animationClip == null) ? 0 : (stateMachineLayer.animationClip.length / animator.speed) / finalAnimSpeed;
            
            if (currentAnimationTime < currentAnimation.endTransitionTime) endTransitionTime = currentAnimationTime;
            else endTransitionTime = currentAnimation.endTransitionTime;
            
            animator.SetInteger(stateMachineLayer.layerSwitchHash, ASIndex);
            stateMachineLayer.ASIndex = ASIndex;
            
            if (ASIndex == 2) {
                animator.SetFloat(stateMachineLayer.S2SpeedHash, finalAnimSpeed);
                animator.SetBool(stateMachineLayer.S2MirrorHash, stateMachineLayer.mirror);
                stateMachineLayer.ASIndex = 1;
            } else {
                animator.SetFloat(stateMachineLayer.S1SpeedHash, finalAnimSpeed);
                animator.SetBool(stateMachineLayer.S1MirrorHash, stateMachineLayer.mirror);
                stateMachineLayer.ASIndex = 2;
            }
        }
        
        override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
            if (currentAnimationTime > 0){
                currentAnimationTime -= Time.deltaTime;
                if (currentAnimationTime < currentAnimation.endTransitionTime){
                    stateMachineLayer.waitAnimation = false;
                    stateMachineLayer.isAnimationFinish = true;
                    currentAnimation.onFinishEvent?.Invoke(animator, stateMachineLayer, currentAnimation);
                    //currentAnimation.nextAnimation?.Play(animator, stateMachineLayer);
                    currentAnimationTime = 0;
                }
            }
        }
    }
}
