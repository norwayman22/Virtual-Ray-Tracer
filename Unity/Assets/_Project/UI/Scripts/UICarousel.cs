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
		private double animationRotatedDegrees = 0;
		private RotateDirection rotateDirection = RotateDirection.None;

		private RectTransform rectTransform;

		private void Start()
		{
			rectTransform = this.GetComponent<RectTransform>();
			animationRemainingSeconds = animationDurationSeconds;
		}

		public void RotateLeft() => rotateDirection = RotateDirection.Left;

		public void RotateRight() => rotateDirection = RotateDirection.Right;

		private void handleRotation()
		{
			if (rotateDirection == RotateDirection.None)
			{
				return;
			}
			double frameDeltaTime = Time.deltaTime;
			double remainingRotation = 90.0f - animationRotatedDegrees;
			double rotateAmount = 90.0f / (animationRemainingSeconds / frameDeltaTime);

			if (rotateAmount >= remainingRotation)
			{
				rectTransform.Rotate(0.0f, (float)(remainingRotation * (int)rotateDirection), 0.0f);

				// Reset rotation animation
				rotateDirection = RotateDirection.None;
				animationRemainingSeconds = animationDurationSeconds;
				animationRotatedDegrees = 0;
			}
			else
			{
				rectTransform.Rotate(0.0f, (float)(rotateAmount * (int)rotateDirection), 0.0f);
				animationRotatedDegrees += rotateAmount;
			}

			// if (animationRemainingSeconds / frameDeltaTime <= 1)
			// {
			// 	rotateAmount /= (animationRemainingSeconds / frameDeltaTime);
			// }

			// rectTransform.Rotate(0.0f, rotateAmount * (int)rotateDirection, 0.0f);
			// animationRemainingSeconds -= frameDeltaTime;
			/*
			if (rotateAmount >= remainingRotation)
			{
				rotateAmount = remainingRotation;

				rotateDirection = RotateDirection.None;
				animationRemainingSeconds = animationDurationSeconds;
				animationRotatedDegrees = 0;
			}
			else
			{
				animationRotatedDegrees += rotateAmount;
			}
			rectTransform.Rotate(0.0f, rotateAmount * (int)rotateDirection, 0.0f);
			*/
			// Vector3 rotateAmount = new Vector3(0.0f, 90.0f * (int)direction, 0.0f);
			// rectTransform.Rotate(rotateAmount);
		}

		void Update() => handleRotation();
	}
}