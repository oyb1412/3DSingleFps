### 

# Combat Quarter


---

## Description

---


- 🔊프로젝트 소개

  Combat Quarter는 AI의 행동을 구현하여 싱글플레이가 가능하게 만든 FPS 게임입니다. 두 종류의 맵과 킬 피드, 상태 보드, 헤드샷 및 다양한 무기를 구현하여 FPS 게임의 다양한 요소를 충실히 재현했습니다. AI는 단순히 플레이어를 따라오는 것이 아니라, 순찰 - 발견 - 공격의 자연스러운 행동을 보입니다.

       

- 개발 기간 : 2024.04.15 - 2024.05.01

- 🛠️사용 기술

   -언어 : C#

   -엔진 : Unity Engine

   -데이터베이스 : 로컬

   -개발 환경: Windows 10, Unity 2021.3.10f1



- 💻구동 화면

![스크린샷(1)](https://github.com/oyb1412/3DSingleFps/assets/154235801/175e8729-0154-48f8-9316-7dd99d0f64c0)

## 목차

---

- 기획 의도
- 발생 및 해결한 주요 문제점
- 핵심 로직


### 기획 의도

---

- 멀티플레이 FPS는 제작 경험을 바탕으로, AI를 도입한 싱글플레이 게임 개발

- 다양한 맵과 무기, 자연스러운 적 AI로 실제 플레이어와 전투하는 느낌을 구현



### 발생 및 해결한 주요 문제점

---

- (발생)적 객체의 과도한 콜라이더 사용으로 인한 프레임 드랍

   -적 객체가 타겟을 서치하기 위해, OverlapSphere및 Raycast 두 종류의 충돌 기능을 동시에 사용.

   ![1](https://github.com/oyb1412/3DSingleFps/assets/154235801/58df63b1-2e83-4528-86b8-350afcacf94d)

   -이로 인해 물리 엔진을 사용하는 객체가 늘어나면, 안정적으로 프레임 유지가 안되는 상황이 발생.

  ![2](https://github.com/oyb1412/3DSingleFps/assets/154235801/024d622b-a9a4-4fd4-8b0d-58a6f4c0d24a)

- (해결)물리엔진 사용을 최소화해 프레임 복구
   -적 객체의 타겟 서치 시도 로직을, OverlapSphere가 아닌 적의 시야 범위를 이용한 내적 계산으로 변경.

  ![3](https://github.com/oyb1412/3DSingleFps/assets/154235801/d1057fcd-a36b-41e0-989b-8b00f53f6287)


   -또한 프로젝트 설정에서 물리 엔진 검사 시간의 주기를 조정하여 최적화.

  ![4](https://github.com/oyb1412/3DSingleFps/assets/154235801/7af43bac-2d86-44a7-afbe-f14a08b78f2a)


  -그 결과, 객체가 아무리 많아져도 안정적으로 프레임 유지 성공.

  ![5](https://github.com/oyb1412/3DSingleFps/assets/154235801/baebf161-4d74-460f-a0e9-51f0f147fdd4)


### 핵심 로직

---
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・FSM(유한 상태 머신) 시스템

AI의 복잡한 행동을 제어하여 조건문이 길어지는 문제를 해결하고 가시성과 유지보수를 개선.

![그림1](https://github.com/oyb1412/3DSingleFps/assets/154235801/5d7760f3-7124-4ccb-9411-f97d918534ec)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)


### ・옵저버 패턴을 이용한 이벤트 위주 로직

데이터 변경이 없을 때도 주기적으로 데이터를 동기화하는 문제를 해결하여 퍼포먼스 최적화.

![그림2](https://github.com/oyb1412/3DSingleFps/assets/154235801/44f44d85-5c93-4b09-a19b-2d25eb924e05)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・컴포넌트 패턴과 전략 패턴을 이용한 무기 관리 시스템

무기 종류가 많아지면서 중복된 코드 작성 문제와 유지보수 어려움을 해결.

![그림3](https://github.com/oyb1412/3DSingleFps/assets/154235801/e05c7170-8e32-44d1-a1c1-1f3d46584098)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・애니메이터와 파라미터를 이용한 유닛 애니메이션 시스템

애니메이션이 자연스럽게 이어지지 않는 문제를 해결.

![그림4](https://github.com/oyb1412/3DSingleFps/assets/154235801/c22bb44e-cbe0-42c0-bb27-ec4537bed895)

![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・씬 전환 페이드 시스템

씬 전환 시 극적인 연출을 위해 페이드 전환 효과를 사용.

![13](https://github.com/oyb1412/3DSingleFps/assets/154235801/146e3220-24e7-4a00-818b-4a0bd11e3908)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・BGM, Personal SFX, 3D SFX 사운드 시스템

사운드가 리스너와의 거리와 관계없이 동일하게 들리는 문제를 해결하여 게임 몰입도 향상.

![그림5](https://github.com/oyb1412/3DSingleFps/assets/154235801/284b44bb-9699-463b-86d8-f9e504f70b06)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・풀링 오브젝트 시스템

객체 생성 및 제거로 인한 퍼포먼스 저하 문제를 해결하기 위해 풀링 시스템 사용.

![31](https://github.com/oyb1412/3DSingleFps/assets/154235801/add0e28c-90cb-4777-a79d-729dbe3749e3)
