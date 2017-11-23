using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ACT     //player의 상태를 표현하기 위해 사용.
{
    IDLE,
    MOVING,
    ATTACK
}
//player가 현재 위치하고있는 hex의 위치정보를 저장할 스크립트
public class PlayerBase : MonoBehaviour
{
    public int MoveRange = 3;       //플레이어가 움직일수 있는 범위.
    public int AttackRange = 3;

    public float rotSpeed = 10.0f;
    public float moveSpeed = 7.0f;
    public int ShellDmg = 20;
    public Hex CurHex;              //현재 플레이어가 위치하고 있는 Hex

    [HideInInspector]
    public List<Hex> MoveHexes;
    
    public ACT act;                  //현재 플레이어의 행동 상태.
    

    public bool isMine = false;     //나의 object인지 여부.
    
    protected PhotonView pv;
    protected Animator animator;
    protected void Awake()
    {
        act = ACT.IDLE;             //플레이어의 초기 설정.
        PlayerManager.pm.Players.Add(this);
        pv = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        
        //내꺼면 ismine check.
        if (pv.isMine)
        {
            CameraControl.cc.m_Targets.Add(transform);
            isMine = true;

        }
    }

    float startTime = 0.0f;
    private void Update()
    {

        /*
         * 자기 자신의 tank라면 움직임과 상태, 턴종료를 처리
         * 아니라면 단순히 움직임만 처리.
         * 이렇게 한 이유는 데이터전송의 딜레이로 인한 오브젝트의 부자연스러운 움직임을 극복하기 위함.
         */
        if (pv.isMine)
        {
            if (act == ACT.MOVING)   //player의 상태가 Moving이라면 거리를 계산하고, 스무스하게 이동.
            {
                
                Hex nextHex = MoveHexes[0];
                animator.SetBool("IsRun", true);

                float distance = Vector3.Distance(transform.position, nextHex.transform.position);
                if (distance >= 0.1f)
                {
                    transform.position = Vector3.Lerp(transform.position, nextHex.transform.position, Time.deltaTime * moveSpeed);
                    Quaternion look_dir = Quaternion.LookRotation(nextHex.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look_dir, Time.deltaTime * rotSpeed);
                }
                else    //다음목표 hex에 도착함.
                {
                    transform.position = nextHex.transform.position;   //이동 완료시키고 턴을 종료.
                    MoveHexes.RemoveAt(0);

                    if (MoveHexes.Count == 0)
                    {   //최종목적지에 도착
                        CurHex = nextHex;
                        animator.SetBool("IsRun", false);

                        //이동을 마친후, 공격사거리 안에 적이 있는지 확인,
                        //있다면 act = Attack.
                        //없다면 턴종료.

                        bool AtkCheck = MapManager.mm.HighLightAttackRanage(CurHex, AttackRange);

                        if (AtkCheck)
                            act = ACT.ATTACK;
                        else
                        {
                            PlayerManager.pm.TurnOver();
                            
                        }

                    }
                }
            }   
        }
        else
        {
            if (act == ACT.MOVING)   //player의 상태가 Moving이라면 거리를 계산하고, 스무스하게 이동.
            {
                if (startTime == 0)
                {
                    startTime = Time.realtimeSinceStartup;
                }
                Hex nextHex = MoveHexes[0];
                animator.SetBool("IsRun", true);

                float distance = Vector3.Distance(transform.position, nextHex.transform.position);
                if (distance >= 0.1f)
                {
                    transform.position = Vector3.Lerp(transform.position, nextHex.transform.position, Time.deltaTime * moveSpeed);
                    Quaternion look_dir = Quaternion.LookRotation(nextHex.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look_dir, Time.deltaTime * rotSpeed);
                }
                else    //다음목표 hex에 도착함.
                {
                    transform.position = nextHex.transform.position;   //이동 완료시키고 턴을 종료.
                    MoveHexes.RemoveAt(0);

                    if (MoveHexes.Count == 0)       //단순히 이동만 한후, act를 IDLE로.
                    {   //최종목적지에 도착
                        CurHex = nextHex;
                        act = ACT.IDLE;
                        animator.SetBool("IsRun", false);

                        float endTime = Time.realtimeSinceStartup;
                        Debug.Log("Total time : " + (endTime - startTime + 0.15f));
                        startTime = 0.0f;

                    }
                }
            }
        }
    }
    
}
