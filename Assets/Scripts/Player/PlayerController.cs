using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private InputAction moveAction;
        private InputAction lookAction;

        private Vector2 moveInput;
        private Vector2 lookInput;

        private CharacterController body;

        [SerializeField, ReadOnlyAttribute, Header("Speed")] private float PlayerSpeed;
        [SerializeField, Space] private float BaseSpeed;
        [SerializeField] private float RunSpeed, SlowSpeed;

        [SerializeField, Header("Camera Settings")] private float Sensitivity;
        private Quaternion PlayerRotation;

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
            body = GetComponent<CharacterController>();
            PlayerRotation = transform.rotation;

            if (moveAction == null || lookAction == null)
            {
                ErrorHandler.LoadErrorScene("Couldn't resolve Move or Look input action // PlayerController // Start");
                return;
            }
        }

        private void Update()
        {
            moveInput = moveAction.ReadValue<Vector2>();
            lookInput = lookAction.ReadValue<Vector2>();
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
            if (!moveInput.Equals(Vector2.zero))
            {
                Move(moveInput);
            }
        }

        private void Look(Vector2 direction)
        {
            if (!direction.Equals(Vector2.zero))
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, Sensitivity * Time.timeScale * direction.x, 0f);
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
    }
}
