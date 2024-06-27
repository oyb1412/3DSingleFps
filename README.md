### 

# Combat Quarter


---

## Description

---


- 🔊프로젝트 소개

  -AI의 행동을 구현해 싱글플레이가 가능하게 만든 FPS 게임입니다.

  -두 종류의 맵과 킬 피드, 상태 보드, 헤드샷 및 각종 무기 등 FPS게임에 필요한 각종 요소들을 충실히   구현했습니다. 

  -또한 단순히 플레이어를 따라오는 AI가 아닌, 순찰 - 발견 - 공격 으로 이어지는
자연스러운 행동을 구현했습니다.

       

- 개발 기간 : 2024.04.15 - 2024.05.01

- 🛠️사용 기술

   -c#

   -Unity Engine


- 💻구동 화면

![스크린샷(1)](https://github.com/oyb1412/3DSingleFps/assets/154235801/175e8729-0154-48f8-9316-7dd99d0f64c0)

## 목차

---

- 기획 의도
- 발생 및 해결한 주요 문제점
- 핵심 로직


### 기획 의도

---

- 멀티플레이 FPS는 제작 경험이 있었기 때문에, AI를 도입한 싱글플레이 FPS게임또한 제작해보고자 시작했습니다.

- 다양한 맵과 무기, 자연스러운 적 AI로 실제 플레이어와 전투중인 느낌을 구현하기 위해 노력했습니다.



### 발생 및 해결한 주요 문제점

---

- (발생)적 객체의 과도한 콜라이더 사용으로 인한 프레임 드랍

   -적 객체가 타겟을 서치하기 위해, OverlapSphere및 Raycast 두 종류의 충돌 기능을 동시에 사용했습니다.

   ![1](https://github.com/oyb1412/3DSingleFps/assets/154235801/58df63b1-2e83-4528-86b8-350afcacf94d)

   -이로 인해 물리 엔진을 사용하는 객체가 늘어나면, 안정적으로 프레임 유지가 안되는 상황이 발생했습니다.

  ![2](https://github.com/oyb1412/3DSingleFps/assets/154235801/024d622b-a9a4-4fd4-8b0d-58a6f4c0d24a)

- (해결)물리엔진 사용을 최소화해 프레임 복구
   -적 객체의 타겟 서치 시도 로직을, OverlapSphere가 아닌 적의 시야 범위를 이용한 내적 계산으로 변경했습니다.

  ![3](https://github.com/oyb1412/3DSingleFps/assets/154235801/d1057fcd-a36b-41e0-989b-8b00f53f6287)


   -또한 프로젝트 설정에서 물리 엔진 검사 시간의 주기를 조금 늦춰, 게임의 밸런스와 최적화 두마리 토끼를 모두 잡았습니다.

  ![4](https://github.com/oyb1412/3DSingleFps/assets/154235801/7af43bac-2d86-44a7-afbe-f14a08b78f2a)


  -그 결과, 객체가 아무리 많아져도 안정적으로 프레임이 유지되는 결과를 확인했습니다.

  ![5](https://github.com/oyb1412/3DSingleFps/assets/154235801/baebf161-4d74-460f-a0e9-51f0f147fdd4)


### 핵심 로직

---
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・FSM(유한 상태 머신) 시스템

AI의 복잡한 행동들을 모두 AI쪽에서 일관성없이 제어해, 조건문이 길어지고 행동이 추가될 때 마다 조건문이 추가되는 등 가시성, 유지보수 모두 최악인 구조를 탈피하기 위해 사용했습니다.

![그림1](https://github.com/oyb1412/3DSingleFps/assets/154235801/5d7760f3-7124-4ccb-9411-f97d918534ec)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)


### ・옵저버 패턴을 이용한 이벤트 위주 로직

데이터의 변경이 없음에도 주기적으로 데이터를 동기화해, 필요 없는 작업이 지속적으로 반복되어 결과적으로 퍼포먼스가 하락되었기 때문에, 최적화를 위해 사용했습니다.

![그림2](https://github.com/oyb1412/3DSingleFps/assets/154235801/44f44d85-5c93-4b09-a19b-2d25eb924e05)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・컴포넌트 패턴과 전략 패턴을 이용한 무기 관리 시스템

무기의 종류가 많아지고 각 무기를 각각 구현해 중복된 코드를 작성하는 일이 잦아지고, 무기의 종류가 늘어나거나 줄어들 때 마다 직접적으로 플레이어 쪽 코드의 수정이 필요하게 되어 유지보수가 굉장히 힘든 문제를 해결하기 위해 사용했습니다.

![그림3](https://github.com/oyb1412/3DSingleFps/assets/154235801/e05c7170-8e32-44d1-a1c1-1f3d46584098)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・애니메이터와 파라미터를 이용한 유닛 애니메이션 시스템

Play() 등 단순한 애니메이션 호출 메서드로 원할 때 애니메이션을 호출할 수는 있었지만, 애니메이션이 자연스럽게 이어지는 것이 아닌 뚝뚝 끊기는 연출이 반복되는 문제를 해결하기 위해 사용했습니다.

![그림4](https://github.com/oyb1412/3DSingleFps/assets/154235801/c22bb44e-cbe0-42c0-bb27-ec4537bed895)

![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・씬 전환 페이드 시스템

씬 전환시 아무 연출없이 즉각적으로 화면이 전환되어 화면이 갈아끼워지는듯한 느낌을 받는다는 피드백을 받아, 보다 극적인 연출을 위해 사용하였습니다.

![13](https://github.com/oyb1412/3DSingleFps/assets/154235801/146e3220-24e7-4a00-818b-4a0bd11e3908)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・BGM, Personal SFX, 3D SFX 사운드 시스템

모든 사운드가 리스너와의 거리에 관계없이 어디에서든 동일하게 들려, 사운드에서 오는 게임의 정보를 획득하는것이 불가능함과 동시다발적으로 무분별하게 들리는 사운드로 게임에 몰입할 수 없는 문제가 발생했기에 이를 타개하고자 사용했습니다.

![그림5](https://github.com/oyb1412/3DSingleFps/assets/154235801/284b44bb-9699-463b-86d8-f9e504f70b06)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・풀링 오브젝트 시스템

각종 오브젝트를 필요할 때 마다 생성, 필요가 없어지면 제거해 짧은 시간 내에 다량의 객체를 생성하고 제거하는 상황이 반복되 퍼포먼스가 크게 하락하였기에 사용했습니다.

![31](https://github.com/oyb1412/3DSingleFps/assets/154235801/add0e28c-90cb-4777-a79d-729dbe3749e3)
