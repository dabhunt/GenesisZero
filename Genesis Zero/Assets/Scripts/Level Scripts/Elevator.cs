using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class Elevator : MonoBehaviour
{
	[Tooltip("List of control buttons")]
	public List<GameObject> buttons;
	[Tooltip("Amount of time it takes to move mDistance")]
	public float mTime;
	[Tooltip("Amount of distance to move")]
	public float mDistance;
	[Tooltip("How far away from button player can activate")]
	public float activationDistance;
	[Tooltip("Distance from platform to check for player")]
	public float movePlayerDistance = 2.5f;
	[Tooltip("True if elevator can move up/down")]
	public bool biDirectional = false;
	[Tooltip("Initial position -1 if it's down, 1 if it's up")]
	[Range(-1, 1)]

	public int iniState;
	private int state; //0: moving, -1: down, 1: up
	private GameObject player;
	private bool canMove = false;
	private int direction = 0;
	private bool movePlayer = false;
	private Animator animator;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		state = iniState;
		animator = player.GetComponent<Animator>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{

			TriggerEvevator();
		}
	}

	public void Interact(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			TriggerEvevator();
		}
	}

	private void TriggerEvevator()
	{
		foreach (var button in buttons)
		{
			if (button.GetComponent<InteractPopup>())
			{
				Destroy(button.GetComponent<InteractPopup>());
			}
			if (Vector3.Distance(player.transform.position, button.transform.position) <= activationDistance)
			{
				canMove = true;
				break;
			}
			else
			{
				canMove = false;
			}
		}
		if (canMove)
		{
			if (Vector3.Distance(player.transform.position, transform.position) <= movePlayerDistance)
				movePlayer = true;
			else
				movePlayer = false;
			Move();
		}
	}

	public void Move()
	{
		// If elevator is moving dont do anyhting
		if (state == 0)
			return;

		// If elevator is down then move up
		if (state == -1)
		{
			if (state != iniState && !biDirectional)
				return;
			state = 0;
			direction = 1;
			StartCoroutine(moveCoroutine());
		}
		// If elevator is up then move down
		else if (state == 1)
		{
			if (state != iniState && !biDirectional)
				return;
			state = 0;
			direction = -1;
			StartCoroutine(moveCoroutine());
		}
	}

	private IEnumerator moveCoroutine()
	{
		float t = 0;
		float speed = mDistance / mTime;
		if (movePlayer)
			GameInputManager.instance.DisablePlayerControls();
		while (t <= mTime)
		{
			animator.SetBool("isGrounded", true);
			transform.position += Vector3.up * direction * speed * Time.fixedDeltaTime;
			if (movePlayer)
				player.transform.position += Vector3.up * direction * speed * Time.fixedDeltaTime;
			t += Time.fixedDeltaTime;
			player.GetComponent<PlayerController>().movementInput = Vector2.zero;
			yield return new WaitForFixedUpdate();
		}
		state = direction;
		if (movePlayer)
			GameInputManager.instance.EnablePlayerControls();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		if (GetComponent<BoxCollider>())
		{
			foreach (GameObject b in buttons)
			{
				Gizmos.DrawWireSphere(b.transform.position, activationDistance);
			}

			Gizmos.DrawWireCube(transform.position + new Vector3(0, mDistance, 0), new Vector3(GetComponent<BoxCollider>().size.x * transform.localScale.x, GetComponent<BoxCollider>().size.y * transform.localScale.y, GetComponent<BoxCollider>().size.z * transform.localScale.z));
		}
	}
}
