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
    [Range(-1, 1)]
    public int iniState;
    private int state; //0: moving, -1: down, 1: up
    private GameObject player;
    private bool canMove = false;
    private int direction = 0;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        state = iniState;
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
            state = 0;
            direction = 1;
            StartCoroutine(moveCoroutine());
        }
        // If elevator is up then move down
        else if (state == 1)
        {
            state = 0;
            direction = -1;
            StartCoroutine(moveCoroutine());
        }
    }

    private IEnumerator moveCoroutine()
    {
        float t = 0;
        float speed = mDistance / mTime;
        Debug.Log(t);
        Debug.Log(speed);
        while (t <= mTime)
        {
            transform.position += Vector3.up * direction * speed * Time.fixedDeltaTime;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        state = direction;
    }
}
