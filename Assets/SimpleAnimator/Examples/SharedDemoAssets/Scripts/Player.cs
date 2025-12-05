using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    public class Player : MonoBehaviour{
        private SimpleAnimator simpleAnimator;
        private Vector2 input;
        
        // Start is called before the first frame update
        void Start(){
            simpleAnimator = GetComponent<SimpleAnimator>();
        }

        // Update is called once per frame
        void Update(){
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");  
            
            //mov
            if (input.magnitude > 0) {
                if (input.x > 0) input.x = 1;
                else if (input.x < 0) input.x = -1;
                
                if (input.y > 0) input.y = 1;
                else if (input.y < 0) input.y = -1;
                
                if (Input.GetKey(KeyCode.LeftShift)) simpleAnimator.PlayAnimation("Run", input.x, input.y);
                else simpleAnimator.PlayAnimation("Walk", input.x, input.y);
                
            } else {
                simpleAnimator.PlayAnimation("Idle");
            }

            if (Input.GetMouseButton(1)){
                simpleAnimator.PlayAnimation("Aim");
            } else simpleAnimator.ClearLayer("Sup");

            if (Input.GetKey(KeyCode.Keypad1)){
                simpleAnimator.SetStatus("Normal", true);
            } else if (Input.GetKey(KeyCode.Keypad2)){
                simpleAnimator.SetStatus("Injure", true);
            }
            
            
            //Camera rotation
            float horizontal = Input.GetAxis("Mouse X") * 10;
            Camera.main.transform.RotateAround(this.transform.position, Vector3.up, horizontal);
        }
    }
}
