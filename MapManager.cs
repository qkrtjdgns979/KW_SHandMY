using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shortest_Path
{
    public Shortest_Path Parent;            //부모(이전) Shortest_Path
    public Hex CurHex;             //그 위치의 hex
    public int F;                  //비용,F = H+G;
    public int G;                  //시작점부터 현재까지의 거리값.
    public int H;                //현재부터 도착점까지의 거리값.

    public Shortest_Path(Shortest_Path parent, Hex hex, int g, int h)
    {
        Parent = parent;
        CurHex = hex;
        G = g;
        H = h;
        F = H + G;

    }
}

//여기서 사용하는 x,y,z는 Hex의 3차원상의 좌표가 아닌, Hex의 순서 index이다.
public class MapManager : MonoBehaviour
{
    public static MapManager mm;    //싱글턴
    public Point[] Dirs;    //헥사주변을 편리하게 접근하기 위한 포인트 배열.
    public GameObject Obj_Hex;      //Hex 타일 오브젝트. 
    public GameObject Water_Hex;
    public GameObject Background;
    //Hex타일의 renderer크기.
    private float HexW;
    private float HexH;

    //Map의 x,y,z크기.
    public int MapSizeX;
    public int MapSizeY;
    public int MapSizeZ;

    public Color HighlightColor;
    public Color HighlightAttackColor;
    public Color HighlightMineColor;
    
    public GameObject Stage;        //생성한 Hex를 자식으로 묶어버릴 오브젝트.
    private Hex[][][] Map;          //x,y,z값에 따른 Hex 타일 배열. (생성된 Hex를 모두 저장하는 배열)


    private void Awake()
    {
        if (mm == null)
            mm = this;

        SetHexSize();       //시작하면 (Awake에서), Map을 만드는데 필요한 HexW,H를 계산한다., 그후 Start에서 HexManager에서 Map생성.
        initDirs();         //인접 헥스들에 쉽게 접근하기 위한 배열(Dir)을 초기화.
    }
    void SetHexSize()   //Hex타일의 renderer의 크기를 측정하여 변수에 저장하는 함수.
    {   
        //renderer의 x크기,z크기 (뼈대의 크기라고 생각하면 될듯, model의 크기)
        HexW = Obj_Hex.GetComponentInChildren<Renderer>().bounds.size.x;
        HexH = Obj_Hex.GetComponentInChildren<Renderer>().bounds.size.z;
        
    }
    public void initDirs()          //주변 hex에 간편하게 접근하기 위하여 초기 설정.
    {
        Dirs = new Point[6];

        Dirs[0] = new Point(1, -1, 0);      //3시
        Dirs[1] = new Point(1, 0, -1);      //1시
        Dirs[2] = new Point(0, 1, -1);      //11시
        Dirs[3] = new Point(-1, 1, 0);      //9시
        Dirs[4] = new Point(-1, 0, 1);      //7시
        Dirs[5] = new Point(0, -1, 1);      //5시

    }
    public Vector3 GetWorldPos(int x , int y , int z)   //Hex의 x,y,z값을 가져오면 3차원 공간상의 좌표를 반환해주는 함수.
    {
        float X, Z;
        X = x * HexW + (z * HexW * 0.5f);
        Z = (-z) * HexH * 0.75f;
        return new Vector3(X, 0, Z);


    }
    public void CreateMap()         //MapSize에 따라 Map을 생성하는 함수.
    {
        //x축 값 설정, ex)3이면 -3 ~ +3 
        //이므로 -부터 시작. 그렇기에 크기가 x2 
        //0이 존재하기에 +1
        //배열의 index는 음수가 없으므로, +Mapsize
        Instantiate(Background);

        Map = new Hex[MapSizeX * 2 + 1][][];

        for(int x = -MapSizeX; x <= MapSizeX; x++)
        {

            Map[x+MapSizeX] = new Hex[MapSizeY * 2 + 1][];  

            for(int y = -MapSizeY; y <= MapSizeY; y++)
            {
                Map[x+MapSizeX][y+MapSizeY] = new Hex[MapSizeY * 2 + 1];
                for(int z = -MapSizeZ; z <= MapSizeZ; z++)
                {
                    Map[x+MapSizeX][y+MapSizeY][z+MapSizeZ] = new Hex();
                    
                    if(x+y+z == 0)          //헥사곤 맵의 특징이 Hex의 x,y,z 좌표값을 모두 더하였을 때 0이여야 한다.
                    {
                        if(Mathf.Abs(y) <= 1)
                        {
                            GameObject map = Instantiate(Water_Hex);
                            map.transform.parent = Stage.transform;
                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ] = map.GetComponent<Hex>(); //x,y,z값에 맞춰서 hex 오브젝트를 생성하고, 생성한것들의 hex스크립트 컴포넌트를 배열로 해서 저장해놓음.


                            Vector3 pos = GetWorldPos(x, y, z);     //x,y,z값에 해당하는 3차원 좌표를 얻어와서, Hex를 가진 오브젝트를 이동시키고, Hex.Point에 좌표정보를 저장.
                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].transform.position = pos;
                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].SetMapPos(x, y, z);
                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].Passable = false;
                        }
                        else
                        {
                            GameObject map = Instantiate(Obj_Hex);
                            map.transform.parent = Stage.transform;
                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ] = map.GetComponent<Hex>(); //x,y,z값에 맞춰서 hex 오브젝트를 생성하고, 생성한것들의 hex스크립트 컴포넌트를 배열로 해서 저장해놓음.


                            Vector3 pos = GetWorldPos(x, y, z);     //x,y,z값에 해당하는 3차원 좌표를 얻어와서, Hex를 가진 오브젝트를 이동시키고, Hex.Point에 좌표정보를 저장.
                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].transform.position = pos;
                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].SetMapPos(x, y, z);
                        }
                        
                    }
                }
            }
        }

        SetPass(2, 0, -2);
        SetPass(3, -1, -2);
        SetPass(1, 1, -2);

        SetPass(4, -1, -3); 
        SetPass(2,1,-3);

        SetPass(5,-1,-4);
        //0, -8, 8
        SetNotPass(0, -8, 8);

        //1,-8,7
        //0,-7,7
        //-1,-6,7
        SetNotPass(1,-8,7);
        SetNotPass(0,-7,7);
        SetNotPass(-1,-6,7);



        //2,-8,6
        //1,-7,6
        //0,-6,6
        //-1,-5,6
        SetNotPass(2, -8, 6);
        SetNotPass(1, -7, 6);
        SetNotPass(0, -6, 6);
        SetNotPass(-1, -5, 6);




        //2,-7,5    
        //1,-6,5
        SetNotPass(2, -7, 5);
        SetNotPass(1, -6, 5);
        SetNotPass(6, -4, -2);



    }
    void SetNotPass(int x, int y, int z)        //해당 위치의 hex의 passable을 설정하는 함수,
    {
        Hex notPassObj = GetPlayerHex(x, y, z);
        notPassObj.Passable = false;

        notPassObj = GetPlayerHex(-x, -y, -z);
        notPassObj.Passable = false;
    }
    //해당 위치의 hex의 passable을 설정하는 함수,
    void SetPass(int x, int y, int z)
    {
        Hex PassObj = GetPlayerHex(x, y, z);
        PassObj.Passable = true;
        PassObj.GetComponent<MeshCollider>().enabled = true;

        PassObj = GetPlayerHex(-x, -y, -z);
        PassObj.Passable = true;
        PassObj.GetComponent<MeshCollider>().enabled = true;
    }
    public Hex GetPlayerHex(int x, int y, int z)    //x,y,z에 해당하는 Map을 반환시켜주는 함수.
    {
        return Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ];
    }
    public void HighLightMoveRanage(Hex start, int range)   //시작점(start)에서 range이내의 Hex 오브젝트의 색을 변경하는 함수.
    {
        start.GetComponentInChildren<Renderer>().material.color = Color.gray;
        for (int x = -MapSizeX; x <= MapSizeX; x++)
        {

            for (int y = -MapSizeY; y <= MapSizeY; y++)
            {

                for (int z = -MapSizeZ; z <= MapSizeZ; z++)
                {

                    if (x + y + z == 0 && Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].Passable == true)   //해당 hex가 x+y+z조건이 적합하고, 지나갈수 있다면.
                    {
                        //시작점(start)에서 현재 index의 hex와의 거리(실제 거리가 아닌 칸수)를 계산.
                        int distance = GetDistance(start, Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ]); //시작점과 해당 hex의 거리를 계산

                        if (distance <= range && distance != 0)     //distance(칸수)가 range 이내에 있고, 0이 아니라면(현재위치가 아니라면)
                        {
                            if (IsReachAble(start, Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ], range) != null)
                            {

                                Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].GetComponentInChildren<Renderer>().material.color = HighlightColor;   //해당 Hex 오브젝트의 색을 변경.

                                foreach (PlayerBase pb in PlayerManager.pm.Players)
                                {

                                    if (Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ] == pb.CurHex && pb.enabled == true)
                                    {
                                        if (pb.isMine)
                                        {
                                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].GetComponentInChildren<Renderer>().material.color = HighlightMineColor;

                                        }
                                        else
                                        {
                                            Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].GetComponentInChildren<Renderer>().material.color = HighlightAttackColor;   //해당 Hex 오브젝트의 색을 변경.

                                        }

                                    }

                                }
                                
                            }

                        }
                        

                    }


                }

            }

        }
    }
    public bool HighLightAttackRanage(Hex start, int range)   //시작점(start)에서 range이내의 Hex 오브젝트의 색을 변경하는 함수.
    {
        int enemyNum = 0;
        for (int x = -MapSizeX; x <= MapSizeX; x++)
        {
            for (int y = -MapSizeY; y <= MapSizeY; y++)
            {

                for (int z = -MapSizeZ; z <= MapSizeZ; z++)
                 {

                    if (x + y + z == 0 && Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].Passable == true)
                    {
                        int distance = GetDistance(start, Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ]); //시작점(start)에서 현재 index의 hex와의 거리(실제 거리가 아닌 칸수)를 계산.

                        if (IsReachAble(start, Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ], range) != null && distance != 0)     //distance(칸수)가 range 이내에 있고, 0이 아니라면(현재위치가 아니라면)
                        {
                            bool isExist = false;
                            foreach(PlayerBase pb in PlayerManager.pm.EnemyUnit)
                            {
                                if (pb.CurHex.MapPos == Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].MapPos && pb.enabled == true)
                                {
                                    enemyNum++;
                                    isExist = true;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].GetComponentInChildren<Renderer>().material.color = HighlightAttackColor;   //해당 Hex 오브젝트의 색을 변경.
                            }
                        }

                    }


                }

            }

        }

        if (enemyNum > 0)
            return true;
        else
            return false;
    }
    public void ResetMapColor()         //모든 Hex 오브젝트의 색을 복구시키는 함수.
    {
        //모든 배열에 접근하여 Hex 오브젝트의 색을 원상태로 만들어줌.
        for (int x = -MapSizeX; x <= MapSizeX; x++)
        {

            for (int y = -MapSizeY; y <= MapSizeY; y++)
            {

                for (int z = -MapSizeZ; z <= MapSizeZ; z++)
                {

                    if (x + y + z == 0)
                        Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].GetComponentInChildren<Renderer>().material.color = Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ].OriColor;

                }

            }

        }
    }
    public int GetDistance(Hex h1, Hex h2)    //두 Hex사이의 distance(칸수)를 구하여 반환해주는 함수.
    {
        Point pos1 = h1.MapPos; 
        Point pos2 = h2.MapPos;

        int distance; //헥사곤 맵에서 거리구하는 공식.
        distance = (Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) + Mathf.Abs(pos1.z - pos2.z)) / 2;

        return distance;
    }
    public List<Hex> GetNeightbors(Hex pos) //해당 hex에서 주위 hex를 list로 반환해주는 함수.
    {
        List<Hex> rtn = new List<Hex>();    //일단 list를 생성하고,
        Point cur = pos.MapPos;             //현 hex위치를 기준으로

        if (!pos.Passable)
            return rtn;
        PlayerManager pm = PlayerManager.pm;
        foreach (PlayerBase pb in pm.Players)
        {
            if (pb == pm.MyBase)
            {
                if (pb.CurHex == pos)
                    break;
            }

            else if (pb.CurHex.MapPos == pos.MapPos)
                return rtn;


        }
        
        //배열의 index에 접근할때 0보다 작은것은 접근하면 안된다.
        foreach (Point p in Dirs)           //근처 hex를 모두 검색
        {
            Point temp = p + cur;
            
            if (temp.x + temp.y + temp.z == 0)      //헥사기준에 맞는것을 선택하여 추가
            {
                if (temp.x >= -MapSizeX && temp.x <= MapSizeX && temp.y >= -MapSizeY && temp.y <= MapSizeY && temp.z >= -MapSizeZ && temp.z <= MapSizeZ)
                {
                    Hex hex = GetHex(temp.x, temp.y, temp.z);

                    rtn.Add(hex);
                }

            }

        }

        return rtn;     //list 반환.
    }
    public Hex GetHex(int x, int y, int z)  //Point(x,y,z)에 위치한 Hex를 Map배열에서 찾아서 반환해주는 함수.
    {
        return Map[x + MapSizeX][y + MapSizeY][z + MapSizeZ];
    }
    public List<Hex> IsReachAble(Hex start, Hex dest, int moveRange)
    {
        List<Hex> path = GetPath(start, dest);
        if (path.Count == 0 || path.Count > moveRange)
            return null;

        return path;
    }
    List<Shortest_Path> OpenList;                    //a*알고리즘을 사용하기 위한 리스트
    List<Shortest_Path> ClosedList;
    public List<Hex> GetPath(Hex start, Hex dest)
    {
        OpenList = new List<Shortest_Path>();
        ClosedList = new List<Shortest_Path>();
        List<Hex> rtnVal = new List<Hex>();


        int H = GetDistance(start, dest);
        Shortest_Path p = new Shortest_Path(null,start, 0, H);

        ClosedList.Add(p);

        Shortest_Path result =  Recursive_FindPath(p, dest);     //p는 시작점.

        if(result == null)
        {
            Debug.Break();
        }
        //if(result == null)
        //{
        //    return rtnVal;
        //}
        while(result.Parent != null)
        {
            rtnVal.Insert(0, result.CurHex);    //역순으로 들어가는것이기에, 0번인덱스에 추가하는것으로.
                                                //rtnVal.Add(result.CurHex);   

            result = result.Parent;

        }
        return rtnVal;

    }
    public Shortest_Path Recursive_FindPath(Shortest_Path parent, Hex dest)
    {
        if(parent.CurHex.MapPos == dest.MapPos)
        {
            return parent;  //목적지를 찾은경우.
        }


        List<Hex> neighbors = GetNeightbors(parent.CurHex);

        foreach (Hex h in neighbors)
        {
            Shortest_Path newP = new Shortest_Path(parent,h, parent.G + 1, GetDistance(h, dest));
            AddToOpenList(newP);

        }

        Shortest_Path bestP;
        if(OpenList.Count == 0)
        {
            return null; //목적지까지 가는길이 없는경우
        }
        bestP = OpenList[0];
        foreach(Shortest_Path p in OpenList)
        {
            if (p.F < bestP.F)
                bestP = p;
        }

        OpenList.Remove(bestP);
        ClosedList.Add(bestP);
        return Recursive_FindPath(bestP,dest);

    }
    public void AddToOpenList(Shortest_Path p)
    {
        foreach(Shortest_Path inP2 in ClosedList)
        {
            if (p.CurHex.MapPos == inP2.CurHex.MapPos)
                return;
        }

        foreach(Shortest_Path inP in OpenList)
        {
            if(p.CurHex.MapPos == inP.CurHex.MapPos)
            {
                if(p.F < inP.F)
                {
                    OpenList.Remove(inP);
                    OpenList.Add(p);
                    return;
                }
            }
        }
        OpenList.Add(p);
    }
}
