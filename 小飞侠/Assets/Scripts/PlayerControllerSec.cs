using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DragonBones;

public class PlayerControllerSec : MonoBehaviour
{

	// 水平移动的速度
	public float _speed = 0.1f;

	// 向上跳跃的加速度
	public float accel = 300f;

	// 获取 碰撞体用于 是否在地面检测 中挂在射灯
	private Rigidbody2D rb;
	private Vector2 input;

	// 发射 检测 人物是否在地面 的射线的长度
	private float rayCastLengthCheck = 0.1f;

	// 玩家所处的状态
	private PlayerState _curState = PlayerState.IDEL;

	private const string NORMAL_ANIMATION_GROUP = "normal";
	private const string RUN_ANIMATION_GROUP = "run";
	//idel & jump & run
	private const string JUMP_ANIMATION_GROUP = "jump";
	private const string ATTACK_ANIMATION_GROUP = "attack";


	private UnityArmatureComponent _armatureComponent;
	private DragonBones.AnimationState _idelState = null;
	private DragonBones.AnimationState _runState = null;
	private DragonBones.AnimationState _jumpState = null;
	// 跳跃的 动画状态
	private DragonBones.AnimationState _attackState = null;

	// 2. 音频


	void Awake ()
	{
		rb = GetComponent<Rigidbody2D> ();
		_armatureComponent = GetComponent<UnityArmatureComponent> ();

	}

	void Start ()
	{
		// 这一步很重要, 如果没有将 初始动画消除的话, 会叠加到 Update方法 的动画里面
		_armatureComponent.animation.Reset ();
	}

	// Update is called once per frame
	void Update ()
	{

		input.x = Input.GetAxis ("Horizontal");
		input.y = Input.GetAxis ("Jump");

		// 移动
		if (input.x > 0) {
			_move (1);
		} else if (input.x < 0) {
			_move (-1);
		} else {
			_move (0);
		}

		// 跳跃
		if (input.y >= 1) {
			_jump ();
		}

		if (Input.GetKeyDown (KeyCode.K)) {
			_fight ();
		}

	}

	/**
	 * 移动方法
	 * dir = 1 表示向右移动, dir = -1 表示向左
	 */ 
	private void _move (int dir)
	{

		if (dir > 0) {
			_curState = PlayerState.RUN;

			//			rb.velocity = new Vector2 (input.x * _speed.x, 0);
			gameObject.transform.Translate (input.x * _speed, 0, 0);
			_armatureComponent.armature.flipX = false;

		} else if (dir < 0) {
			_curState = PlayerState.RUN;

			//			rb.velocity = new Vector2 (input.x * _speed.x, 0);
			gameObject.transform.Translate (input.x * _speed, 0, 0);
			_armatureComponent.armature.flipX = true;

		} else {
			_curState = PlayerState.IDEL;
		}

		_updateAnimation ();
	}

	/**
 * 跳跃
 */ 
	private void _jump ()
	{

		// 判断是否在地面上
		if (PlayerIsOnGround ()) {

			// 1. 改变状态
			_curState = PlayerState.JUMP;

			// 2. 淡入跳跃动画
			_jumpState = _armatureComponent.animation.FadeIn ("jump", -1.0f, 1, 0, NORMAL_ANIMATION_GROUP, AnimationFadeOutMode.SameGroup);
			_jumpState.timeScale = 2.0f;  // 控制 某个动画状态 的 播放速度
			_jumpState.autoFadeOutTime = 0.1f;  // 对于限定播放次数的动画, 会自动淡出

			// 3. 添加位移
			rb.AddForce (new Vector2 (0, accel));

			// 4. 将 Jump 同组的其他动画停掉, 并设置成 null
			// 将idel 状态停掉, 并设置成null
			if (_idelState != null) {
				_idelState.FadeOut (0.1f);
				_idelState = null;
			}

			// 将run 状态停掉, 并设置成null
			if (_runState != null) {
				_runState.FadeOut (0.1f);
				_runState = null;
			}
		}

	}

	/**
	 * 攻击
	 */ 
	private void _fight ()
	{

		_curState = PlayerState.ATTACK;

		_attackState = _armatureComponent.animation.FadeIn ("attack1", -1, 1, 0, ATTACK_ANIMATION_GROUP);
		_attackState.autoFadeOutTime = 0.1f;
	}

	/**
	 * 更新动画
	 */ 
	private void _updateAnimation ()
	{
		if (_curState == PlayerState.IDEL) {
			if (_idelState == null) {
				if ((_jumpState == null) || (_jumpState != null && !_jumpState.isPlaying)) {
					_idelState = _armatureComponent.animation.FadeIn ("steady", -1.0f, -1, 0, NORMAL_ANIMATION_GROUP, AnimationFadeOutMode.SameGroup);

				}
			}

			if (_runState != null) {
				//				_runState.Stop ();     //停止的话, 动作还是保持跑步的状态
				_runState.FadeOut (0.1f);              //淡出动画
				_runState = null;
			}

			if (_jumpState != null && !_jumpState.isPlaying) {
				_jumpState.FadeOut (0.1f);
				_jumpState = null;
			}

		} else if (_curState == PlayerState.RUN) {

			if (_runState == null) {
				if ((_jumpState == null) || (_jumpState != null && !_jumpState.isPlaying)) {
					_runState = _armatureComponent.animation.FadeIn ("walk", -1.0f, -1, 0, NORMAL_ANIMATION_GROUP, AnimationFadeOutMode.SameGroup);
					_runState.timeScale = 1.2f;
				}
			}

			if (_idelState != null) {
				_idelState.FadeOut (0.1f);
				_idelState = null;
			}

			if (_jumpState != null && !_jumpState.isPlaying) {
				_jumpState.FadeOut (0.1f);
				_jumpState = null;
			}

		} else {
		}

	}

	/**
	 * 判断是否在地面上 true 表示 在地面
	 */ 
	private bool PlayerIsOnGround ()
	{
		float del = 2.58f;

		float width = GetComponent<Collider2D> ().bounds.size.x * 0.5f;
		float height = GetComponent<Collider2D> ().bounds.size.y * 0.5f - del;

//		// 测试时使用这个 (可以看到射线)
//		Vector2 orig1_test = new Vector2 (transform.position.x, transform.position.y - height);
//		RaycastHit2D hit1_test = Physics2D.Raycast (
//			                         orig1_test,
//			                         -Vector2.up, 
//			                         rayCastLengthCheck);
//		Debug.DrawLine (orig1_test, hit1_test.point);

		// 射灯1 的起点
		Vector2 orig1 = new Vector2 (transform.position.x, transform.position.y - height);
		bool hit1 = Physics2D.Raycast (
			            orig1,
			            -Vector2.up, 
			            rayCastLengthCheck);

		Vector2 orig2 = new Vector2 (transform.position.x + (width - 0.2f), transform.position.y - height);
		bool hit2 = Physics2D.Raycast (
			            orig2, 
			            -Vector2.up,
			            rayCastLengthCheck);

		Vector2 orig3 = new Vector2 (transform.position.x - (width - 0.2f), transform.position.y - height);
		bool hit3 = Physics2D.Raycast (
			            orig3, 
			            -Vector2.up,
			            rayCastLengthCheck);

		if (hit1 || hit2 || hit3) {
			return true;
		}

		return false;
	}

	void OnCollisionEnter2D (Collision2D coll)
	{

		//		Debug.Log (coll.otherCollider.name);
		//		Debug.Log (coll.collider.name);
	}

	//	void OnTriggerEnter2D(Collider2D other) {
	//
	//		Debug.Log ("Tir = " + other.name);
	//	}
}
