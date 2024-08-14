using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;

public class PlayerController : MonoBehaviour
{   
    // �����࣬�ⲿ���Է���
    public float runSpeed;
    public float jumpSpeed;
    public int maxJumpNum;
    public float attack1CD;

    // ˽���࣬�ⲿ���ܷ���
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
        float moveDir = Input.GetAxis("Horizontal"); // ��ȡ��������ң��Ʋ��󷵻�-1�� �ҷ���1
        Vector2 runVel = new(moveDir * runSpeed, playerCharacterRigidbody.velocity.y);
        // �ٶ���һ����άʸ����runSpeed ��һ���ⲿ�������������1֮����x�ᣬy���ٶȲ���
        playerCharacterRigidbody.velocity = runVel; // �������Ǹ���ά������������

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
