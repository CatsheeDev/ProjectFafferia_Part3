using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller
{
    [RequireComponent (typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private InputAction moveAction; 
        private CharacterController body; 
        
        [SerializeField, Header("Speed")] private float PlayerSpeed; //cant combine all 4 cuz unity serialization sukcs
        [SerializeField] private float BaseSpeed, RunSpeed, SlowSpeed; 
        
        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move"); 
            body = GetComponent<CharacterController>();
            
            if (moveAction == null)
            {
                ErrorHandler.LoadErrorScene("Couldn't resolve Move input action // PlayerController // Start");
                return; 
            }
        }
    
        // Update is called once per frame
        private void Update()
        {
            Vector2 moveInfo = moveAction.ReadValue<Vector2>();
    
            body.Move(moveInfo * PlayerSpeed * Time.DeltaTime); //NOT good. CHANGE THIS!!!!!!!!!!! -love, me :)
        }
    }
}
