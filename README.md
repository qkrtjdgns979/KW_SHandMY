# KW_SHandMY
ReadMe

목표
	Network를 기반으로 한 Unity Tool을 이용한 게임을 구현한다. 
  본 프로젝트	의 목적은 Network 기반 게임개발에 우선순위를 두고 있기 때문에 Unity에	서 지원하는 Photon Unity Network의 Server를 이용하여 게임에   사용될 Network Client를 구축하고 Delay의 발생여부와 개선된 사항을 확인할 수 있는 턴제 대전 게임을 구축하는데 그 목적이 있다. 
  Network의 Data Latency를 줄이기 위해 고안한 방법은 현재의 위치 정보를 상대방에게 주기적으로 송수신하는 방법이 아닌 Object가 이동할 최종 도착점 
  정보를 수신자 측에 전송하여 자신이 이동할 경로에 대한 위치 정보를 생략하는 방법을 사용하여 발생하는 Delay를 줄이는 방법을 사용한다.
  
개발 내용
	연구의 필요성에 따라 프로젝트를 진행하기 위해 Unity Tool을 사용한 Network 기반 턴제 대전 게임을 개발한다. 
  게임 내 Object의 움직임이 Delay를 구분하는데 가장 핵심이 되는 포인트이기 때문에 Hexagon Map을 구성하여 Turn방식을 
  사용하는 방향으로 개발을 진행한다. 
  캐릭터가 움직이	는 것에 있어서 일정한 주기가 있어야 확인을 할 수 있기 때문에 최단 경로 알고리즘 중 게임개발에 
  가장 많이 사용되는 A* Algorithm을 사용하여 구현한다. 
  같은 움직임에 대한 Delay를 측정하는 것이 가장 정확하기 때문에 일정한 형식을 가지고 움직일 수 있는 타일 기반의
  Hexagon Map을 제작	한다. 
  최단경로를 사용하여 움직이기 때문에 Object별 움직임의 범위를 설정하여 송신자가 수신자에게 자신의 도착점 정보만을 
  넘겨주는 방식을 사용	할 때와 자신의 위치정보를 상대방에게 지속적으로 전송할 때 움직이는 방	식의 차이를 비교하여 분석한다.
  
  
주의사항 
 포톤유니티 네트워크를 이용하였기 때문에 네트워크가 연결되어있어야 한다.
 게임이 진행되기 위해서는 2명의 Client가 필요하다.

Credit 
 전민영 , 박성훈
 
 Reference
a* algorithm : http://homepages.abdn.ac.uk/f.guerin/pages/teaching/CS1013/practicals/aStarTutorial.htm	
Hexagon map : https://www.redblobgames.com/grids/hexagons/
Photon Unity Network & Unity : 
절대강좌! 유니티 5 이재현 저, 위키북스
https://www.photonengine.com/ko-KR/Photon
