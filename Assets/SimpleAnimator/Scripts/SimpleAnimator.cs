using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace AionGames.SimpleAnimatorPackage {

    [RequireComponent(typeof(Animator))]
    public class SimpleAnimator : MonoBehaviour {
        public AnimationLayerList layerList;
        public AnimationStatusList statusList;
        
        // Hide these fields in the inspector
        [HideInInspector] public AnimatorOverrideController overrideController;
        [HideInInspector] public Animator animator;
        
        public Dictionary<int, StateMachineLayer> stateMachineLayers = new Dictionary<int, StateMachineLayer>();
        
        // Dropdown for selecting the default status
        [DinamicDropdown("statusList.status", "name")]
        public int defaultStatus;
        [DinamicDropdown("statusList.status", "name")]
        public int status;
        
        
        // List of animation sets for different status
        public List<AnimationItemList> animationList = new List<AnimationItemList>();
        
        // Retrieves the animation list for the current status
        public AnimationItemList getAnimationItemList(int layer, int animationID, bool showWarning){
            foreach (AnimationItemList animationItemList in animationList){
                if (animationItemList.layer == layer && animationItemList.status == status && animationItemList.animation?.nameID == animationID){
                    return animationItemList;
                }
                
                
            }
            
            if (showWarning) Debug.LogWarning($"[{typeof(SimpleAnimator).FullName}] Animation not found.");
            return null;
        }
        
        // Retrieves the animation list for the default status
        public AnimationItemList getDefAnimationItemList(int layer, int animationID, bool showWarning){
            foreach (AnimationItemList animationItemList in animationList){
                if (animationItemList.layer == layer && animationItemList.status == defaultStatus && animationItemList.animation?.nameID == animationID){
                    return animationItemList;
                }
            }
            
            if (showWarning) Debug.LogWarning($"[{typeof(SimpleAnimator).FullName}] Animation not found.");
            return null;
        }
        
        private void OnValidate(){
            if (this.layerList != null && this.layerList.isEdited){
                this.layerList.GenerateAnimatorController();
            }
        }
        
        // Plays an animation by name
        public bool PlayAnimation(String animationName){ 
            return PlayAnimation(Animator.StringToHash(animationName), null, null); 
        }
        
        public bool PlayAnimation(String animationName, float speed){ 
            return PlayAnimation(Animator.StringToHash(animationName), speed, null); 
        }
        
        public bool PlayAnimation(String animationName, float speed, bool mirror){ 
            return PlayAnimation(Animator.StringToHash(animationName), speed, mirror); 
        }
        
        

        public bool Play(int ID){
            return PlayAnimation(ID, null, null); 
        }
        
        public bool Play(int ID, float speed){
            return PlayAnimation(ID, speed, null); 
        }

        // Plays an animation by ID
        public bool PlayAnimation(int ID, float? speed, bool? mirror){
            CreateStateMachineLayers();

            if (layerList == null) return false;
            if (statusList == null) return false;
            
            AnimationData animationData = null;
            int layerIndex = 0;
            
            for (int i = 0; i < layerList.layers.Length; i++){
                // Get animation from the current status list
                animationData = getAnimationItemList(i, ID,  false)?.animation;
                if (animationData != null) {
                    layerIndex = i;
                    break;
                }
            }
            
            
            for (int i = 0; i < layerList.layers.Length; i++){
                // If not found, check the default status list
                if (animationData == null) {
                    animationData = getDefAnimationItemList(i, ID,  true)?.animation;
                    layerIndex = i;
                } else break;
            }
            
            if (animationData == null) return false;
            
            // Get the corresponding layer
            StateMachineLayer layer = stateMachineLayers[layerIndex];
            
            // If the same animation is already playing, do nothing
            if (animationData == layer.animation) return false;



            if (speed == null) speed = animationData.speed;
            if (mirror == null) mirror = animationData.mirror;
            
            // Play the animation and reset motion position
            if (animationData.Play(animator, layer, animationData.animationClip, (float) speed, (bool) mirror)){
                layer.motionPos.x = 0;
                layer.motionPos.y = 0;
            }
            
            return true;
        }
        
        // Plays an animation with motion blending
        public bool PlayAnimation(String animationName, float motionPosX, float motionPosY){ 
            return PlayAnimation(Animator.StringToHash(animationName), motionPosX, motionPosY); 
        }

        public bool PlayAnimation(int ID, float motionPosX, float motionPosY){
            CreateStateMachineLayers();
            
            if (layerList == null) return false;
            if (statusList == null) return false;
            
            AnimationData animationData = null;
            int layerIndex = 0;
            
            for (int i = 0; i < layerList.layers.Length; i++){
                // Get animation from the current status list
                animationData = getAnimationItemList(i, ID,  false)?.animation;
                if (animationData != null) {
                    layerIndex = i;
                    break;
                }
            }
            
            
            for (int i = 0; i < layerList.layers.Length; i++){
                // If not found, check the default status list
                if (animationData == null) {
                    animationData = getDefAnimationItemList(i, ID,  true)?.animation;
                    layerIndex = i;
                } else break;
            }
            
            if (animationData == null) return false;
            
            // Get the corresponding layer
            StateMachineLayer layer = stateMachineLayers[layerIndex];

            // If the same animation is already playing with the same motion position, do nothing
            if (layer.animation == animationData && layer.motionPos.x == motionPosX && layer.motionPos.y == motionPosY) 
                return false;
            
            // Find the appropriate motion tree entry
            MotionTree motionTree = GetAnimationMotion(animationData, motionPosX, motionPosY);
            if (motionTree == null) return false;
            
            // Play the animation with motion blending
            if (animationData.Play(animator, layer, motionTree.animationClip, motionTree.speed * animationData.speed, motionTree.mirror)){
                layer.motionPos.x = motionPosX;
                layer.motionPos.y = motionPosY;
            }

            return true;
        }

        // Clears an animation layer by name
        public void ClearLayer(string layerName){
            ClearLayer(Animator.StringToHash(layerName));
        }

        // Clears an animation layer
        private void ClearLayer(StateMachineLayer layer){
            layer.animation = null;
            layer.waitAnimation = false;
            layer.isAnimationFinish = true;

            // Reset animator parameter
            animator.SetInteger(layer.layerSwitchHash, 0);
        }
        
        // Clears an animation layer by ID
        private void ClearLayer(int layerID){
            StateMachineLayer layer = null;
            for (int i = 0; i < stateMachineLayers.Count; i++){
                if (stateMachineLayers[i].layerNameHash == layerID){
                    layer = stateMachineLayers[i];
                }
            }
            
            if (layer == null) return;

            if (!layer.waitAnimation){
                ClearLayer(layer);
            }
        }
        
        // Clears an animation layer without waiting
        private void ClearLayerNWA(int layerID){
            StateMachineLayer layer = null;
            for (int i = 0; i < stateMachineLayers.Count; i++){
                if (stateMachineLayers[i].layerNameHash == layerID){
                    layer = stateMachineLayers[i];
                }
            }
            
            if (layer == null) return;
            ClearLayer(layer);
        }
        
        // Initializes state machine layers if they haven't been created
        private void CreateStateMachineLayers(){
            if (stateMachineLayers.Count > 0) return;
            
            // Create animation layers
            for (int i = 0; i < this.layerList.layers.Length; i++){
                AnimationLayer layer = this.layerList.layers[i];
                stateMachineLayers[i] = new StateMachineLayer(i, layer.name);
            }
        }

        // Sets the animation status by name
        public void SetStatus(string statusName, bool playSameAnimation){
            bool found = false;
            for (int i = 0; i < this.statusList.status.Length; i++){
                AnimationStatus status = this.statusList.status[i];
                if (status.ID == Animator.StringToHash(statusName)){
                    this.status = i;
                    found = true;
                    break;
                }
            }

            if (!found){
                Debug.LogWarning($"Status '{statusName}' not found.");
                return;
            }

            // Optionally replay the current animation
            if (playSameAnimation){
                for (int i = 0; i < this.layerList.layers.Length; i++){
                    StateMachineLayer layer = stateMachineLayers[i];

                    if (layer.animation != null){
                        if (layer.motionPos.magnitude > 0) 
                            PlayAnimation(layer.animation.nameID, layer.motionPos.x, layer.motionPos.y);
                        else 
                            PlayAnimation(layer.animation.nameID, null, null);
                    }
                }
            }
        }

        // Finds the appropriate motion tree entry for blending
        private MotionTree GetAnimationMotion(AnimationData animationData, float blendPosX, float blendPosY){
            foreach (MotionTree motionTree in animationData.motionTree){
                if (motionTree.pos.x == blendPosX && motionTree.pos.y == blendPosY){
                    return motionTree;
                }
            }
            return null;
        }


        public AnimationData GetAnimation(string layerName){
            return GetAnimation(Animator.StringToHash(layerName));
        }

        public AnimationData GetAnimation(int layerHash){
            for (int i = 0; i < this.layerList.layers.Length; i++){
                StateMachineLayer layer = stateMachineLayers[i];
                if (layer.layerNameHash == layerHash){
                    return layer.animation;
                }
            }
            
            return null;
        }
        
        public AnimationData GetAnimationByLayerIndex(int layerIndex){
            if (layerIndex >= stateMachineLayers.Count) return null;
            
            StateMachineLayer layer = stateMachineLayers[layerIndex];
            return layer.animation;
        }

        // Called when the script starts
        void Start(){
            animator = GetComponent<Animator>();
            overrideController = new AnimatorOverrideController(this.layerList.animatorController);
            animator.runtimeAnimatorController = overrideController;
        }
        
        // Gets the directory path of the SimpleAnimatorLink script
        public static string getDirectory() {
            string[] guids = AssetDatabase.FindAssets("SimpleAnimator t:Script");

            if (guids.Length == 0){
                Debug.LogError("SimpleAnimator script not found.");
                return "";
            }
            
            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            string currentDir = Path.GetDirectoryName(scriptPath);
            string parentDir = Path.GetDirectoryName(currentDir);
            
            return parentDir.Replace("\\", "/");
        }
    }
 }
