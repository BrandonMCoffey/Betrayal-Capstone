﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	[SerializeField] private float _walkingSpeed = 7.5f;
	[SerializeField] private float _runningSpeed = 11.5f;
	[SerializeField] private bool _canJump = true;
	[SerializeField, ShowIf("canJump")] private float _jumpSpeed = 8.0f;
	[SerializeField] private float _gravity = 20.0f;
	[SerializeField] private Transform _cameraParent;
	[SerializeField] private float _lookSpeed = 2.0f;
	[SerializeField] private float _lookXLimit = 45.0f;
	
	[SerializeField] private CharacterController _controller;
	[SerializeField, ReadOnly] private bool _canMove = true;
	
	[SerializeField, ReadOnly] private Vector3 _moveDirection = Vector3.zero;
	private float rotationX = 0;
	
	private float MoveSpeed => PlayerInputManager.Sprint ? _runningSpeed : _walkingSpeed;
	private bool CanJump => _canJump && _controller.isGrounded;

	public Transform CameraParent => _cameraParent;
	public void SetCanMove(bool canMove) => _canMove = canMove;

	private void OnValidate()
	{
		if (!_controller) _controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void ProcessMovement()
	{
		if (!_canMove) return;
		float movementDirectionY = _moveDirection.y;
		
		var moveDirInput = PlayerInputManager.MoveDir;
		_moveDirection = transform.forward * moveDirInput.y + transform.right * moveDirInput.x;
		_moveDirection *= MoveSpeed;

		_moveDirection.y = CanJump && PlayerInputManager.Jump ? _jumpSpeed : movementDirectionY;

		if (!_controller.isGrounded) _moveDirection.y -= _gravity * Time.deltaTime;

		_controller.Move(_moveDirection * Time.deltaTime);

		if (_canMove)
		{
			var lookDirInput = PlayerInputManager.LookDir;
			rotationX += -lookDirInput.y * _lookSpeed;
			rotationX = Mathf.Clamp(rotationX, -_lookXLimit, _lookXLimit);
			_cameraParent.localRotation = Quaternion.Euler(rotationX, 0, 0);
			transform.rotation *= Quaternion.Euler(0, lookDirInput.x * _lookSpeed, 0);
		}
		SendTransformToNetwork();
	}
	
	private void SendTransformToNetwork()
	{
		if (LocalUser.Instance) LocalUser.Instance.SetTransform(transform.position, transform.eulerAngles);
	}
}
