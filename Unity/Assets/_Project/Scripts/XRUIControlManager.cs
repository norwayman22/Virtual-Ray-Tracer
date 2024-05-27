using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts
{
    public class XRUIControlManager : MonoBehaviour
    {
        [Space]
        [Header("Controllers")]

        [SerializeField]
        Transform leftController;
        [SerializeField]
        Transform rightController;

        [Space]
        [Header("Main Canvas")]

        [SerializeField]
        RectTransform mainCanvas;

        [Space]
        [Header("Controller Activate References")]

        [SerializeField]
        InputActionReference leftActivateReference;
        [SerializeField]
        InputActionReference rightActivateReference;

        [Space]
        [Header("Locomotion Game Objects")]

        [SerializeField]
        GameObject turn;
        [SerializeField]
        GameObject move;

        private Vector3 canvasDefaultPosition;
        private Quaternion canvasDefaultRotation;

        public bool leftUIActivated { get; private set; }
        public bool rightUIActivated { get; private set; }

        void Start()
        {
            canvasDefaultPosition = mainCanvas.localPosition;
            canvasDefaultRotation = mainCanvas.localRotation;

            var leftActivation = GetInputAction(leftActivateReference);
            if (leftActivation != null)
            {
                leftActivation.started += onStartActivation;
                leftActivation.canceled += onStopActivation;
            }

            var rightActivation = GetInputAction(rightActivateReference);
            if (rightActivation != null)
            {
                rightActivation.started += onStartActivation;
                rightActivation.canceled += onStopActivation;
            }
        }

        void onStartActivation(InputAction.CallbackContext context)
        {
            if (leftUIActivated || rightUIActivated)
                return;

            string action = context.action.ToString();
            bool leftActive = action.Contains("XRI Left");
            bool rightActive = action.Contains("XRI Right");

            // Return if none or both of the controller triggers are pressed
            if (leftActive == rightActive)
                return;

            if (leftActive)
            {
                mainCanvas.SetParent(leftController);
                leftUIActivated = true;
                move.SetActive(false);
            }
            else if (rightActive)
            {
                mainCanvas.SetParent(rightController);
                rightUIActivated = true;
                turn.SetActive(false);
            }
            mainCanvas.SetLocalPositionAndRotation(canvasDefaultPosition, canvasDefaultRotation);
            mainCanvas.gameObject.SetActive(true);
        }

        void onStopActivation(InputAction.CallbackContext context)
        {
            // Ignore if no UI active
            if (!leftUIActivated && !rightUIActivated)
                return;

            string action = context.action.ToString();
            if ((leftUIActivated && action.Contains("XRI Left")) ||
                (rightUIActivated && action.Contains("XRI Right")))
            {
                leftUIActivated = false;
                rightUIActivated = false;
                mainCanvas.gameObject.SetActive(false);

                turn.SetActive(true);
                move.SetActive(true);
            }
        }

        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }

        void Update()
        {

        }
    }
}
