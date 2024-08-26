using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private InputAction moveAction; 

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Moave"); 

        if (moveAction == null)
        {
            ErrorHandler.LoadErrorScene("Couldn't resolve Move input action");
            return; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
