using UnityEditor.Animations;
using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
	
	[System.Serializable]
	public class AnimationLayer{
		public string name;
		[Range(0f,1f)]
		public float weight = 1f;
		public AvatarMask avatarMask;
		public AnimatorLayerBlendingMode blending;
		public bool sync;
		[ConditionalHide("sync")]
		[DinamicDropdown("layers", "name")]
		public int syncLayer;
		[ConditionalHide("sync")]
		public bool timing;
		public bool iKPass;
	}
}