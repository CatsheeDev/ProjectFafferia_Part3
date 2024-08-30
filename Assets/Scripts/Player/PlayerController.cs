using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerManager))]
    public class PlayerController : MonoBehaviour
    {
        public bool Moved
        {
            get
            {
                return MoveVec.magnitude > 0f;
            }
        }
        
        private InputAction MoveAction;
        private InputAction LookAction;
        private InputAction SprintAction; 

        private Vector2 MoveInput;
        private Vector2 LookInput;
        private Quaternion PlayerRotation;
        private Vector3 MoveVec; 

        private CharacterController Body;
        private PlayerManager Manager;

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
            MoveAction = InputSystem.actions.FindAction("Move");
            LookAction = InputSystem.actions.FindAction("Look");
            SprintAction = InputSystem.actions.FindAction("Sprint"); 

            Body = GetComponent<CharacterController>();
            Manager = GetComponent<PlayerManager>();

            PlayerRotation = transform.rotation;

            if (MoveAction == null || LookAction == null || SprintAction == null)
            {
                ErrorHandler.LoadErrorScene("Couldn't resolve Move, Look or Speint input action // PlayerController // Start");
                return;
            }

            SprintAction.started += OnSprintStarted;
            SprintAction.canceled += OnSprintCanceled;
        }

        private void Update()
        {
            MoveInput = MoveAction.ReadValue<Vector2>();
            LookInput = LookAction.ReadValue<Vector2>();

            Sprint(); 
        }

        private void LateUpdate()
        {
            if (!LookInput.Equals(Vector2.zero))
            {
                Look(LookInput);    
            }
        }

        private void FixedUpdate()
        {
            if (!MoveInput.Equals(Vector2.zero))
            {
                Move(MoveInput);
            }
        }

        private void Look(Vector2 direction)
        {
            if (!direction.Equals(Vector2.zero))
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, Manager.CamSensitivity * Time.timeScale * direction.x, 0f);
                PlayerRotation *= deltaRotation;

                transform.rotation = PlayerRotation;
            }
        }


        public void Move(Vector2 direction)
        {
            Vector3 localInput = transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
            Vector3 projectedVector = new Vector3(localInput.x, 0, localInput.z);
            MoveVec = PlayerSpeed * Time.deltaTime * projectedVector.normalized;

            Body.Move(MoveVec);
        }

        ///SPRITING
        private void OnSprintStarted(InputAction.CallbackContext context)
        {
            if (Manager.CanSprint && Manager.Stamina > 0)
            {
                Manager.IsSprinting = true;
                PlayerSpeed = RunSpeed; 
            }
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            Manager.IsSprinting = false;
            PlayerSpeed = BaseSpeed; 
        }

        private void Sprint()
        {
            if (!Manager.CanSprint)
            {
                Manager.IsSprinting = false;
                return;
            }

            bool ShouldStopSprinting = false;

            if (Moved && Manager.IsSprinting)
            {
                Manager.Stamina -= Time.deltaTime * Manager.StaminaDepleteRate;

                if (Manager.Stamina <= 0)
                {
                    PlayerSpeed = SlowSpeed;
                    Manager.Stamina = -1f;
                    Manager.UIManager.ToggleObject("RestIndicator", true, false);
                    ShouldStopSprinting = true;
                }
            }
            else if (!Moved && !Manager.IsSprinting)
            {
                Manager.Stamina += Time.deltaTime * Manager.StaminaDepleteRate;

                if (Manager.Stamina < 10 && Manager.Stamina > 1)
                {
                    PlayerSpeed = BaseSpeed;
                    Manager.UIManager.ToggleObject("RestIndicator", false, false);
                }
            }

            Manager.Stamina = Mathf.Clamp(Manager.Stamina, -1f, Manager.MaxStamina);
            Manager.UIManager.ModifySlider("StaminaMeter", Manager.Stamina, Operation.Set);

            if (ShouldStopSprinting)
            {
                Manager.IsSprinting = false;
            }
        }

    }
}
