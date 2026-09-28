using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace Platformer
{
    
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2,bool> Look = delegate { };

        public event UnityAction<bool> Jump = delegate { };

        InputSystem_Actions playerInputActions;

        public Vector3 Direction => playerInputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (playerInputActions == null)
            {
                playerInputActions = new InputSystem_Actions();
                playerInputActions.Player.SetCallbacks(this);
            }

            playerInputActions.Enable();
        }
        void OnDisable()
        {
            playerInputActions.Disable();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }
        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.displayName == "Mouse";

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }
        public event UnityAction<bool> Attack = delegate { };

        public void OnAttack(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Attack.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Attack.Invoke(false);
                    break;
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            //noop
        }
    }
}

