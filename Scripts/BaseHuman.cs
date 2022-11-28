using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    //是否移动
    private bool isMoving = false;
    //移动目标点
    private Vector3 targetPosition;
    //移动速度
    public float speed = 1.2f;
    //动画组件
    private Animator animator;
    //是否在攻击
    internal bool isAttacking = false;
    internal float attackingTime = float.MinValue;
    //自身描述
    public string desc = "";

    //移动到某处
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);

    }
    //移动Update
    public void MoveUpdate()
    {
        if (isMoving == false)
        {
            return;
        }
        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);//形成不断跑过去的动作

        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }
    //攻击动作
    public void Attack()
    {
        isAttacking = true;
        attackingTime = Time.time;
        animator.SetBool("isAttacking",true);
    }
    //攻击循环
    public void AttackUpdate()
    {
        if (!isAttacking) return;
        if (Time.time - attackingTime < 1f) return;
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
    //Update
    protected void Start()
    {
        animator = GetComponent<Animator>();

    }
    //每帧改地址
    protected void Update()
    {
        MoveUpdate();
        AttackUpdate();
    }

}
