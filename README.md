# Beyond The Axis

## 조작법!

WASD + 스페이스바 : 이동  
Alt 키: 커서 락 풀기/세팅  
마우스 좌클릭: 공격  
F키: 상호작용



**축을 넘어서** — 시점이 바뀌는 멀티 퍼스펙티브 플랫포머

Unity 2022.3 기반 팀 프로젝트 게임입니다.  
1인칭, 사이드뷰, 쿼터뷰, 보스전까지 한 편의 스토리로 이어지는 단일 플레이어 경험을 목표로 제작되었습니다.

---

## 🎮 게임 개요

플레이어는 여러 **시점(축)**을 넘나들며 스테이지를 진행합니다.

| 순서 | 스테이지 | 시점 | 목표 |
| - | - | - | - |
| 1 | **Start** | — | 게임 시작 |
| 2 | **FirstPersonScene** | 1인칭(FPS) | 지정된 오브젝트 4개 찾기 → 포탈 활성화 |
| 3 | **SideViewScene** | 2D 사이드뷰 | 플랫포머로 골 지점 도달 |
| 4 | **SideViewEndScene** | 2D 사이드뷰 | 다음 스테이지로 이동 |
| 5 | **QuarterViewScene** | 쿼터뷰(탑다운) | 열쇠 6개 수집 → 상자에서 나침반 획득 |
| 6 | **BossStage** | 쿼터뷰 / 보스 전용 카메라 | 보스 격파 후 타이틀로 복귀 |

---

## ✨ 주요 기능

### 시점별 플레이

- **1인칭**: FPS 카메라, 마우스 룩, 특정 오브젝트 탐색 및 포탈 이동
- **사이드뷰**: 2D 플랫포머 이동/점프, 스프링, 추가 점프 아이템, 리스폰, 골 트리거
- **쿼터뷰**: 3D 필드, NavMesh AI 적, 열쇠/상자/나침반, 상호작용(Outline 등)
- **보스전**: Cinemachine 다중 카메라, 보스 패턴(기본 투사체, 수평/수직 레이저), VFX 풀링, 플레이어 체력/공격

### 공통 시스템

- **체력/공격**: `HealthSystem`, `AttackSystem`, `IDamageable`, 체력바 UI
- **상호작용**: `IInteract`, `PlayerInteractor`, 열쇠/상자/오브젝트
- **씬 전환**: `LevelLoader` 싱글톤 + 로딩 애니메이션
- **UI**: TextMeshPro, 미션/타이틀 패널(`TextUIPanel`)

---

## 🛠 기술 스택

- **엔진**: Unity 2022.3.62f2
- **패키지**: Cinemachine, AI Navigation, TextMeshPro, Post Processing, Shader Graph, UniTask, PrimeTween 등
- **구조**: C# 스크립트, 씬별 매니저 + 공통 시스템 분리

---

## 📁 프로젝트 구조

| 폴더 | 설명 |
|------|------|
| **Scenes/** | StartScene, FirstPersonScene, SideViewScene, QuarterViewScene, BossStage 등 |
| **Scripts/** | Boss, Side, KSJ(FPS), KTS(Quarter), LEES(Player/Combat) 등 |
| **Prefabs/** | KSJ, KTS, LEES, LYC, SHY 팀별 프리팹 |
| **Animations/** | 애니메이션 클립/컨트롤러 |
| **Resources/** | SceneLoader, VFX 등 |
| **Settings/** | 라이팅 설정 |
