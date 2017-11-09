# KW_SHandMY
ReadMe
1.Hex.cs
Hex타일의 위치정보를 저장하는 클래스.
Hex타일을 클릭시, OnMouseDown 콜백함수가 실행되어, Object가 해당 Hex로 이동.
Data Latency문제를 해결하기위해, RPC를 사용하여 Client들에게 움직이고자 하는 Hex타일의 정보 송신.

2.HexManager.cs
해당 Scene에서 모든 Manager Class를 관리하는 클래스.
Master Client에서만 실행하는 클래스.
게임을 시작하는데 필요한 기본적인 셋팅을 해주는 함수를 실행.
턴을 관리하는 Coroutine 함수를 실행.

3.MapManager.cs
Hexagon Map에 대한 정보를 관리하는 클래스.
Hexagon Map을 생성하는 함수가 있으며, Hexagon Map생성시 Hex Class에 해당 Hexagon 정보 입력.
Object 클릭시, Object의 이동반경을 표현해주는 함수 구현.
A*알고리즘을 사용하여 최단경로를 구하는 함수 구현.

3.PlayerManager.cs
Player와 Player의 Turn을 관리하는 클래스.
Player를 생성하는 함수 구현.
MapManager Class의 최단경로를 구하는 함수를 이용하여 Player를 Move하는 함수 구현.
Player에게 Turn을 주고, Player가 움직임을 마치면 Turn Over하는 함수 구현.
RPC를 사용하여 해당 Turn의 Client에게 Turn을 제공하는 함수 구현.

4.PhotonInit.cs
Lobby Scene에서 Photon Network에 접속하는 클래스.
Room Make, Room Join, Room Search 를 구현.

5.RoomManager.cs
Room Scene을 관리하는 클래스.
RPC를 사용하여 채팅창 구현.
게임시작 구현.

6.DataTransfer.cs
Client Object의 Data를 송수신하는 클래스.
Player가 현재 위치한 Hex 타일의 정보를 송수신.

7.PlayerBase.cs
Player의 상태를 관리하는 클래스.
PlayerManager의 Player Move하는 함수를 이용하여, Map에서 Player를 움직이게 하는 함수 구현.
Hex클래스에서 송신한 Hex타일의 정보를 수신하여, Player의 Move 구현.

8.Shoot.cs
Object의 발사를 관리하는 클래스.
RPC를 사용하여 모든 Client에서 총알이 발사되도록 구현.

9.UserPlayer.cs
PlayerBase를 상속받은 클래스.
본인 Object를 클릭시, MapManager클래스를 이용하여 활동반경 표현.
상대방 Object를 클릭시, Shoot클래스에 총알을 발사하도록 명령.
