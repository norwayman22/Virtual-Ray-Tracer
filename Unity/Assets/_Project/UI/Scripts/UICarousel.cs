using UnityEngine;
using UnityEngine.InputSystem;
using _Project.Scripts;
using System;

namespace _Project.UI.Scripts
{
	public class UICarousel : MonoBehaviour
	{
		private enum RotateDirection : int
		{
			Left = 1,
			None = 0,
			Right = -1
		}

		[SerializeField]
		private XRUIControlManager xrUiControlManager;

		[SerializeField]
		public double animationDurationSeconds;

		[Space]
		[Header("Controller Activate References")]

		[SerializeField]
		InputActionReference leftMoveReference;
		[SerializeField]
		InputActionReference rightMoveReference;

		private double animationRemainingSeconds;
		private double currentRotation;
		private RotateDirection rotateDirection;

		private RectTransform canvasRectTransform;

		private void resetRotationAnimation()
		{
			animationRemainingSeconds = animationDurationSeconds;
			currentRotation = 0;
			rotateDirection = RotateDirection.None;
		}

		private void Start()
		{
			canvasRectTransform = this.GetComponent<RectTransform>();
			resetRotationAnimation();

			var leftMoving = GetInputAction(leftMoveReference);
			if (leftMoving != null)
				leftMoving.started += onStartMoving;

			var rightMoving = GetInputAction(rightMoveReference);
			if (rightMoving != null)
				rightMoving.started += onStartMoving;
		}

		void onStartMoving(InputAction.CallbackContext context)
		{
			if (!canvasRectTransform.gameObject.activeInHierarchy
				|| rotateDirection != RotateDirection.None
				|| (!xrUiControlManager.leftUIActivated && !xrUiControlManager.rightUIActivated))
				return;

			Vector2 moveDirection = context.ReadValue<Vector2>();
			if (Math.Abs(moveDirection.x) < Math.Abs(moveDirection.y))
				return;

			rotateDirection = moveDirection.x > 0 ? RotateDirection.Left : RotateDirection.Right;
		}

		private void handleRotation()
		{
			if (rotateDirection == RotateDirection.None)
				return;

			double remainingRotation = 30.0f - currentRotation;
			double rotateAmount = 30.0f / (animationRemainingSeconds / Time.deltaTime);

			if (rotateAmount < remainingRotation)
				currentRotation += rotateAmount;
			else
			{
				rotateAmount = remainingRotation;
				resetRotationAnimation();
			}
			canvasRectTransform.Rotate(0.0f, (float)(rotateAmount * (int)rotateDirection), 0.0f);
		}

		static InputAction GetInputAction(InputActionReference actionReference)
		{
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
			return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
		}

		void Update() => handleRotation();
	}
}