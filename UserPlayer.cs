using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : PlayerBase            //PlayerBase를 상속받음.
{
   
    private void OnMouseDown() //상대 오브젝트를 누르면 공격.
    {
        if(PlayerManager.pm.MyTurn)
        {
            if(isMine)
            {
                if(act == ACT.IDLE && PlayerManager.pm.CanMove == true)
                {

                    PlayerManager.pm.MyBase = this;
                    MapManager.mm.ResetMapColor();
                    MapManager.mm.HighLightMoveRanage(CurHex, MoveRange);
                }
            }
            else
            {
                if(PlayerManager.pm.MyBase != null)
                {
                    if((PlayerManager.pm.MyBase.act == ACT.IDLE) || (PlayerManager.pm.MyBase.act == ACT.ATTACK))
                    {
                        if (MapManager.mm.IsReachAble(PlayerManager.pm.MyBase.CurHex, CurHex, PlayerManager.pm.MyBase.AttackRange) != null)
                        {
                            PlayerManager.pm.MyBase.GetComponent<Shoot>().Fire_RPC(pv.viewID);
                            MapManager.mm.ResetMapColor();
                            PlayerManager.pm.TurnOver();
                        }
                    }
                }
            }
        }
    }
    
}
