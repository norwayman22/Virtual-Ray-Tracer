using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		public double animationDurationSeconds;
		private double animationRemainingSeconds;
		private double currentRotation;
		private RotateDirection rotateDirection;

		private RectTransform rectTransform;

		private void resetRotationAnimation()
		{
			animationRemainingSeconds = animationDurationSeconds;
			currentRotation = 0;
			rotateDirection = RotateDirection.None;
		}

		private void Start()
		{
			rectTransform = this.GetComponent<RectTransform>();
			resetRotationAnimation();
		}

		public void RotateLeft() => rotateDirection = RotateDirection.Left;

		public void RotateRight() => rotateDirection = RotateDirection.Right;

		private void handleRotation()
		{
			if (rotateDirection == RotateDirection.None)
				return;

			double remainingRotation = 90.0f - currentRotation;
			double rotateAmount = 90.0f / (animationRemainingSeconds / Time.deltaTime);

			if (rotateAmount < remainingRotation)
				currentRotation += rotateAmount;
			else
			{
				rotateAmount = remainingRotation;
				resetRotationAnimation();
			}
			rectTransform.Rotate(0.0f, (float)(rotateAmount * (int)rotateDirection), 0.0f);
		}

		void Update() => handleRotation();
	}
}