using Assets.Scripts;
using Assets.Scripts.CombatLogic;
using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;

#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;
		public bool slide;
        public bool reloading;
		public bool skill1;
		public bool skill2;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = false;
		public bool cursorInputForLook = false;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAim(InputValue value)
        {
            aim = value.isPressed;
        }

        public void OnShoot(InputValue value)
        {
            shoot = value.isPressed;
            if (shoot && !aim) shoot = false;
        }
        public void OnSlide(InputValue value)
        {
			slide = value.isPressed;
        }
		public void OnReloading(InputValue value)
        {
            reloading = value.isPressed;
        }

		public void OnSkill1(InputValue value)
		{
			skill1 = value.isPressed;
		}
        public void OnSkill2(InputValue value)
        {
            skill2 = value.isPressed;
        }
        public void OnChangeTeammateStatu(InputValue value)
		{
			CombatContextManager.Instance.ChangeTeammateStatu();
        }

		public void OnChangeView(InputValue value)
		{
			ViewChangeManager.Instance.ChangeVew();
		}



#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			//SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}