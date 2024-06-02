using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.UI.Scripts.Tutorial
{
    public class XrInputTutorialEventHandler : MonoBehaviour
    {
        [Space]
        [Header("Input References")]

        [SerializeField]
        InputActionReference turnReference;
        [SerializeField]
        InputActionReference moveReference;
        [SerializeField]
        InputActionReference leftGrabReference;
        [SerializeField]
        InputActionReference rightGrabReference;

        [Serializable]
        public class Event : UnityEvent { }

        [Space]
        [Header("Events")]
        public Event OnTurn;
        public Event OnMove;
        public Event OnDoubleGrab;

        private bool leftGrabbing, rightGrabbing;
        private bool doubleGrabDone = false;
        void Start()
        {
            var turn = GetInputAction(turnReference);
            if (turn != null)
                turn.started += ctx => OnTurn?.Invoke();

            var move = GetInputAction(moveReference);
            if (move != null)
                move.started += ctx => OnMove?.Invoke();

            var leftGrab = GetInputAction(leftGrabReference);
            if (leftGrab != null)
            {
                leftGrab.started += ctx => leftGrabbing = true;
                leftGrab.canceled += ctx => leftGrabbing = false;
            }

            var rightGrab = GetInputAction(rightGrabReference);
            if (rightGrab != null)
            {
                rightGrab.started += ctx => rightGrabbing = true;
                rightGrab.canceled += ctx => rightGrabbing = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!doubleGrabDone && leftGrabbing && rightGrabbing)
            {
                doubleGrabDone = true;
                OnDoubleGrab?.Invoke();
            }
        }

        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}
