using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerManager))]
    public class PlayerController : MonoBehaviour
    {
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction sprintAction; 

        private Vector2 moveInput;
        private Vector2 lookInput;
        private Quaternion PlayerRotation;
        private Vector3 lastPosition; 

        private CharacterController body;
        private PlayerManager manager;

        [SerializeField, ReadOnlyAttribute, Header("Speed")] private float PlayerSpeed;
        [SerializeField, Space] private float BaseSpeed;
        [SerializeField] private float RunSpeed, SlowSpeed;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            PlayerSpeed = BaseSpeed;
        }
    #endif 

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            lookAction = InputSystem.actions.FindAction("Look");
            sprintAction = InputSystem.actions.FindAction("Sprint"); 

            body = GetComponent<CharacterController>();
            manager = GetComponent<PlayerManager>();

            PlayerRotation = transform.rotation;

            if (moveAction == null || lookAction == null || sprintAction == null)
            {
                ErrorHandler.LoadErrorScene("Couldn't resolve Move, Look or Speint input action // PlayerController // Start");
                return;
            }

            sprintAction.started += OnSprintStarted;
            sprintAction.canceled += OnSprintCanceled;
        }

        private void Update()
        {
            moveInput = moveAction.ReadValue<Vector2>();
            lookInput = lookAction.ReadValue<Vector2>();

            Sprint(); 
        }

        private void LateUpdate()
        {
            if (!lookInput.Equals(Vector2.zero))
            {
                Look(lookInput);    
            }
        }

        private void FixedUpdate()
        {
            lastPosition = transform.position;
            if (!moveInput.Equals(Vector2.zero))
            {
                Move(moveInput);
            }
        }

        private void Look(Vector2 direction)
        {
            if (!direction.Equals(Vector2.zero))
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, manager.CamSensitivity * Time.timeScale * direction.x, 0f);
                PlayerRotation *= deltaRotation;

                transform.rotation = PlayerRotation;
            }
        }


        public void Move(Vector2 direction)
        {
            Vector3 localInput = transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
            Vector3 projectedVector = new Vector3(localInput.x, 0, localInput.z);

            body.Move(PlayerSpeed * Time.deltaTime * projectedVector.normalized);
        }

        public bool Moving()
        {
            return (Vector3.Distance(transform.position, lastPosition) > 0.01f);
        }

        ///SPRITING
        private void OnSprintStarted(InputAction.CallbackContext context)
        {
            if (manager.CanSprint && manager.Stamina > 0)
            {
                manager.IsSprinting = true;
                PlayerSpeed = RunSpeed; 
            }
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            manager.IsSprinting = false;
            PlayerSpeed = BaseSpeed; 
        }

        private void Sprint()
        {
            if (!manager.CanSprint)
            {
                manager.IsSprinting = false;
                return;
            }

            bool shouldStopSprinting = false;

            if (Moving() && manager.IsSprinting)
            {
                manager.Stamina -= Time.deltaTime * manager.StaminaDepleteRate;

                if (manager.Stamina <= 0)
                {
                    PlayerSpeed = SlowSpeed;
                    manager.Stamina = -1f;
                    manager.UIManager.ToggleObject("RestIndicator", true, false);
                    shouldStopSprinting = true;
                }
            }
            else if (!Moving() && !manager.IsSprinting)
            {
                manager.Stamina += Time.deltaTime * manager.StaminaDepleteRate;

                if (manager.Stamina < 10 && manager.Stamina > 1)
                {
                    PlayerSpeed = BaseSpeed;
                    manager.UIManager.ToggleObject("RestIndicator", false, false);
                }
            }

            manager.Stamina = Mathf.Clamp(manager.Stamina, -1f, manager.MaxStamina);
            manager.UIManager.ModifySlider("StaminaMeter", manager.Stamina, Operation.Set);

            if (shouldStopSprinting)
            {
                manager.IsSprinting = false;
            }
        }

    }
}
