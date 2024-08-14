using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;

public class PlayerController : MonoBehaviour
{   
    // 公开类，外部可以访问
    public float runSpeed;
    public float jumpSpeed;
    public int maxJumpNum;
    public float attack1CD;

    // 私有类，外部不能访问
    private Rigidbody2D playerCharacterRigidbody;
    private Animator playerAnimator;
    private BoxCollider2D playerContact;

    private int currentJumpNum;

    private bool isRun;
    private bool isGround;

    private bool lastGround;

    // Start is called before the first frame update
    void Start()
    {
        playerCharacterRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerContact = GetComponent<BoxCollider2D>();
 
        PlayerInitial();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        Run();
        Jump();
        Attack();
    }

    void LateUpdate()
    {
        SpeedOutput();
        SendJumpNum();
        RecordState();
    }

    void PlayerInitial()
    {
        currentJumpNum = 0;
        isRun = false;
        isGround = false;
        lastGround = false;
        CDMgr.Instance.AddCD("Attack1", attack1CD);
    }

    void Run()
    {
        float moveDir = Input.GetAxis("Horizontal"); // 获取方向键左右，推测左返回-1， 右返回1
        Vector2 runVel = new(moveDir * runSpeed, playerCharacterRigidbody.velocity.y);
        // 速度是一个二维矢量，runSpeed 是一个外部定义的量，×±1之后赋予x轴，y轴速度不变
        playerCharacterRigidbody.velocity = runVel; // 将上面那个二维向量赋予人物

        if (playerCharacterRigidbody.velocity.x > Mathf.Epsilon)
        {
            isRun = true;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (playerCharacterRigidbody.velocity.x < -Mathf.Epsilon)
        {
            isRun = true;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else { isRun = false; }
        playerAnimator.SetBool("Run", isRun);
    }

    void Jump()
    {
        if (isGround & (!lastGround)) { currentJumpNum = 0; }
        if ((!isGround) & lastGround) { currentJumpNum = 1; }
        if(Input.GetButtonDown("Jump"))
        {
            if (currentJumpNum < maxJumpNum)
            {   
                Vector2 jumpVel = new(0.0f, jumpSpeed);
                playerCharacterRigidbody.velocity = Vector2.up * jumpVel;
                if (currentJumpNum == 0) { playerAnimator.SetTrigger("Jump"); }
                else { playerAnimator.SetTrigger("MultiJump"); }
                currentJumpNum = currentJumpNum + 1;
            }
        }
    }

    void Attack()
    {
        if(Input.GetButtonDown("Attack1"))
        {
            if (CDMgr.Instance.IsReady("Attack1"))
            {
                playerAnimator.SetTrigger("Attack1");
                CDMgr.Instance.StartCool("Attack1");
            }
        }
    }

    void CheckGrounded()
    {
        isGround = playerContact.IsTouchingLayers(LayerMask.GetMask("Ground"));
        playerAnimator.SetBool("Grounded", isGround);
    }

    void SpeedOutput()
    {
        playerAnimator.SetFloat("SpeedX", playerCharacterRigidbody.velocity.x);
        playerAnimator.SetFloat("SpeedY", playerCharacterRigidbody.velocity.y);
    }

    void SendJumpNum()
    {
        playerAnimator.SetInteger("NowJumpNum", currentJumpNum);
    }

    void RecordState()
    {
        lastGround = isGround;
    }
}
