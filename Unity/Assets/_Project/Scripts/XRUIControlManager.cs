using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.UI;

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
        [Header("Canvases")]

        [SerializeField]
        RectTransform mainCanvas;

        [SerializeField]
        List<GameObject> hideableCanvases;

        [Space]
        [Header("Controller Activate References")]

        [SerializeField]
        InputActionReference leftActivateReference;
        [SerializeField]
        InputActionReference rightActivateReference;

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

        void ToggleUI()
        {
            foreach (GameObject canvas in hideableCanvases)
            {
                bool toggle = !canvas.GetComponent<Canvas>().enabled;
                canvas.GetComponent<Canvas>().enabled = toggle;
                canvas.GetComponent<GraphicRaycaster>().enabled = toggle;
                canvas.GetComponent<TrackedDeviceGraphicRaycaster>().enabled = toggle;
            }
        }

        void onStartActivation(InputAction.CallbackContext context)
        {
            // Ignore if UI already active
            if (leftUIActivated || rightUIActivated)
                return;

            string action = context.action.ToString();
            bool leftActive = action.Contains("XRI Left");
            bool rightActive = action.Contains("XRI Right");

            // Ignore if none or both of the controller triggers are pressed
            if (leftActive == rightActive)
                return;

            if (leftActive)
            {
                mainCanvas.SetParent(leftController);
                leftUIActivated = true;
            }
            else if (rightActive)
            {
                mainCanvas.SetParent(rightController);
                rightUIActivated = true;
            }
            mainCanvas.SetLocalPositionAndRotation(canvasDefaultPosition, canvasDefaultRotation);
            ToggleUI();
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
                ToggleUI();
            }
        }

        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }

        void OnDestroy()
        {
            var leftActivation = GetInputAction(leftActivateReference);
            if (leftActivation != null)
            {
                leftActivation.started -= onStartActivation;
                leftActivation.canceled -= onStopActivation;
            }

            var rightActivation = GetInputAction(rightActivateReference);
            if (rightActivation != null)
            {
                rightActivation.started -= onStartActivation;
                rightActivation.canceled -= onStopActivation;
            }
        }

        void Update()
        {

        }
    }
}
