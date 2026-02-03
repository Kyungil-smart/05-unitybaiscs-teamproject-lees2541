using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace ExampleNamespace
{
	public class CameraChangeExample : MonoBehaviour
	{
		[Serializable]
		public enum CameraState
		{
			QuarterWorld,
			QuarterCamera,
			TpsCamera,
			FpsCamera,
			SideviewWorld
		}

		public CameraState State
		{
			get => state;
			set
			{
				state = value;
				ChangeState(value);
			}
		}

		private CameraState state;
		public CinemachineVirtualCameraBase FpsCamera;
		public CinemachineVirtualCameraBase TpsCamera;
		public CinemachineVirtualCameraBase QuarterViewCamera;
		public CinemachineVirtualCameraBase SideViewCamera;
		public UnityChan.PlayerController Player;

		private Camera mainCamera;
		private List<CinemachineVirtualCameraBase> cameras = new();
		private string[] stateNames = Enum.GetNames(typeof(CameraState));

		private void Awake()
		{
			mainCamera = GetComponent<Camera>();

			cameras.Add(FpsCamera);
			cameras.Add(TpsCamera);
			cameras.Add(QuarterViewCamera);
			cameras.Add(SideViewCamera);
		}

		private void Start()
		{
			ChangeState(CameraState.QuarterCamera);
		}

		void ChangeState(CameraState state)
		{
			foreach (var cameraBase in cameras)
			{
				cameraBase.Priority = -1;
			}

			if (state == CameraState.FpsCamera)
			{
				mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
			}
			else
			{
				mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("Player"));
			}

			switch (state)
			{
				case CameraState.QuarterWorld:

					Player.PerspectiveState = UnityChan.PlayerController.MovementSpace.World;
					QuarterViewCamera.Priority = 10;
					break;
				case CameraState.QuarterCamera:
					Player.PerspectiveState = UnityChan.PlayerController.MovementSpace.Camera;
					QuarterViewCamera.Priority = 10;
					break;
				case CameraState.TpsCamera:
					Player.PerspectiveState = UnityChan.PlayerController.MovementSpace.Camera;
					TpsCamera.Priority = 10;
					break;
				case CameraState.FpsCamera:
					Player.PerspectiveState = UnityChan.PlayerController.MovementSpace.Camera;
					FpsCamera.Priority = 10;
					break;
				case CameraState.SideviewWorld:
					Player.PerspectiveState = UnityChan.PlayerController.MovementSpace.World;
					SideViewCamera.Priority = 10;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		private void OnGUI()
		{
			foreach (var s in stateNames)
			{
				if (GUILayout.Button(s))
				{
					ChangeState((CameraState)Enum.Parse(typeof(CameraState), s));
				}
			}
		}
	}
}