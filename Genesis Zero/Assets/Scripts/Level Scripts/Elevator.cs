using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class Elevator : MonoBehaviour
{
    [Tooltip("List of control buttons")]
    public List<GameObject> buttons;
    public float mTime;
    public float mDistance;
    public float activationDistance;
    public float movePlayerDistance = 2.5f;
    public bool biDirectional = false;
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

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            foreach (var button in buttons)
            {
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
            yield return new WaitForFixedUpdate();
        }
        state = direction;
        if (movePlayer)
            GameInputManager.instance.EnablePlayerControls();
    }
}
