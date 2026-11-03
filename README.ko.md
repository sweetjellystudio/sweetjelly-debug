#   SWeetJelly Debug

Unity 6.3 LTS용 동기 진단 로깅 패키지다. severity/filter·formatter·출력 대상 계약과 SWJDebug/Trace facade, Unity Console·uGUI Text 출력을 제공한다. 일반 Release Player에서도 동작하며 Enabled/MinimumLevel로 제어한다.

호출·등록·설정 변경은 하나의 문맥에서 순차 수행하고 Unity 출력은 main thread에서 호출한다. 전달 시작 시 출력 목록을 고정하며 callback 예외는 즉시 전파한다. 같은 service의 로그 재진입은 오류다. bootstrap은 자신의 runtime 등록과 facade만 해제하며 설정 asset과 runtime owner를 구분한다.

설치·사용법은 [README][readme], 이전 변경과 상세 계약은 [이전 안내][migration]·[아키텍처][architecture]에 있다. 샘플은 Console을 유지하면서 자신이 생성한 UI 출력만 추가/해제한다.

[readme]: README.md
[migration]: Documentation~/migration-2.0.md
[architecture]: ARCHITECTURE.md
