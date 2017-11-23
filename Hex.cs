using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Point          //Hex좌표를 저장할 클래스.
{
    public int x;
    public int y;
    public int z;

    public Point(int X, int Y, int Z)
    {
        x = X;
        y = Y;
        z = Z;

    }
    //오퍼레이터 연산자 정의.
    public static Point operator+(Point p1, Point p2)
    {
        return new Point(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }   
    public static bool operator ==(Point p1, Point p2)
    {
        return (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z);
    }
    public static bool operator !=(Point p1, Point p2)
    {
        return (p1.x != p2.x || p1.y != p2.y || p1.z != p2.z);
    }
}

/*
 * Hex오브젝트에 달려있는 스크립트
 * Hex스크립트는 해당 Hex오브젝트의 헥스좌표에 대한 정보와 마우스핸들러함수를 가지고있음.
 */
public class Hex : MonoBehaviour
{
    
    public Point MapPos;        //해당 Hex의 좌표를 저장하는 변수.
    public bool Passable = true;
    public Color OriColor = Color.white;

    

    public void SetMapPos(Point pos)           //오버라이딩. Hex의 좌표를 저장하는 함수.
    {
        MapPos = pos;
    }
    public void SetMapPos(int X, int Y, int Z)      //오버라이딩. Hex의 좌표를 저장하는 함수.
    {
        Point pos = new Point(X, Y, Z);
        
        MapPos = pos;
    }
    bool GetCanClick()  //이 Hex와 Players의 hex를 비교하여 같은게 있으면, 이 Hex위에 Player가 있다는 의미이므로 클릭불가능.
    {
        foreach(PlayerBase pb in PlayerManager.pm.Players)
        {
            if (pb.CurHex.MapPos == this.MapPos)
                return false;
        }
        return true;
    }
    private void OnMouseDown()      //해당 Hex를 선택하면 실행되는 콜백함수.
    {
        if(!GetCanClick() || Passable != true)  //클릭불가.
            return;

        if(PlayerManager.pm.MyTurn && PlayerManager.pm.MyBase != null && PlayerManager.pm.CanMove == true)
        {
            if(MapManager.mm.IsReachAble(PlayerManager.pm.MyBase.CurHex, this, PlayerManager.pm.MyBase.MoveRange) != null)
            {

                PlayerManager.pm.MovePlayer(PlayerManager.pm.MyBase.CurHex, this);
                PlayerManager.pm.SendMoveMappos_RPC(MapPos.x, MapPos.y, MapPos.z, PlayerManager.pm.MyBase.GetComponent<PhotonView>().viewID);
                PlayerManager.pm.CanMove = false;
            }
           
        }
    }
    
}
