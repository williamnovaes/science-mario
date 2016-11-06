﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (SpriteRenderer))]
public class SMBPlayer : MonoBehaviour {

	private float _jumpTimer;
	private bool  _isOnGround;

	public float xSpeed = 1f;
	public float ySpeed = 5f;
	public float longJumpTime = 1f;
	public float longJumpWeight = 0.1f;

	// Custom components
	private Animator       _animator;
	private Rigidbody2D    _rigidbody;
	private BoxCollider2D  _collider;
	private SpriteRenderer _renderer;

	void Awake() {

		_animator = GetComponent<Animator> ();
		_rigidbody = GetComponent<Rigidbody2D> ();
		_collider = GetComponent<BoxCollider2D> ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {

		_isOnGround = IsOnGround ();
		_animator.SetBool ("isJumping", !_isOnGround);

		Jump ();

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Move ((float)SMBConstants.MoveDirection.Backward);
			_animator.SetBool ("isMoving", true);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {

			Move ((float)SMBConstants.MoveDirection.Forward);
			_animator.SetBool ("isMoving", true);
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.RightArrow)) {

			Vector2 currentVelocity = _rigidbody.velocity;
			currentVelocity.x = 0f;
			_rigidbody.velocity = currentVelocity;

			_animator.SetBool ("isMoving", false);
		}
	}

	void Jump() {

		if (_isOnGround && Input.GetKeyDown(KeyCode.Z)){

			_jumpTimer = longJumpTime;
			_rigidbody.velocity = Vector2.up * ySpeed * Time.fixedDeltaTime;
		}

		if (_jumpTimer > 0f) {

			if (Input.GetKeyUp(KeyCode.Z)) {

				_jumpTimer = 0f;

			}
			else if(Input.GetKey(KeyCode.Z)) {

				_jumpTimer -= Time.fixedDeltaTime;
				if (_jumpTimer <= longJumpTime/2f)
					_rigidbody.velocity += Vector2.up * ySpeed * longJumpWeight * Time.fixedDeltaTime;
			}
		}

	}

	void Move(float side) {

		Vector2 currentVelocity = _rigidbody.velocity;
		currentVelocity.x = (xSpeed * side) * Time.fixedDeltaTime;
		_rigidbody.velocity = currentVelocity;

		if (side == (float)SMBConstants.MoveDirection.Forward)
			_renderer.flipX = false;

		if (side == (float)SMBConstants.MoveDirection.Backward)
			_renderer.flipX = true;

		// Lock player x position
		Vector3 playerPos = transform.position;
		playerPos.x = Mathf.Clamp (playerPos.x, SMBGameWorld.Instance.LockLeftX - SMBGameWorld.Instance.TileMap.size, 
			SMBGameWorld.Instance.LockRightX);
		transform.position = playerPos;
	}

	bool IsOnGround() {

		Vector2 rayOrigin = _collider.bounds.min;

		for (int i = 0; i < 3; i++) {

			RaycastHit2D ray = Physics2D.Raycast(rayOrigin, -Vector2.up, 0.01f);
			if (ray.collider && ray.collider.tag == "Platform") {

				if(Mathf.Abs(rayOrigin.y - ray.collider.bounds.max.y) < 0.01f)
					return true;
			}

			rayOrigin.x += _collider.bounds.size.x / 2f;
		}

		return false;
	}
}
