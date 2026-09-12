# iPhone 회전 중심 정렬 — Flutter 비교 (2026-09-13)

## 현재 동작: 중심 정렬·원래 크기 유지

사용자가 아래 `ScaleToFill` 수정으로 중심은 맞지만 스트레칭이 어색하다고 확인했다.
이에 iOS의 생성자와 layout을 `ContentMode.Center`·`GravityCenter`로 변경했다.
회전 중에는 원래 크기의 내용을 중심에 놓고 bounds 밖의 내용은 clip한다.
새 크기의 레이아웃은 drawable에 다시 그린다. 이미지 전체를 가로·세로로 늘리지 않는다.
이는 사용자의 비율·크기 보존 요구를 반영한 정책이며 Flutter 일반 화면의 기본
`resize` gravity를 그대로 복제하는 정책은 아니다. UIKit의 회전 transform·anchor는
변경하지 않는다. 이전 transaction 표시 및 비동기 GPU 회수는 유지한다.

기존 중심 검사에 20×20 중심 표식의 가로·세로 픽셀 크기를 함께 검사하도록 보강했다.
세로·중간·가로 bounds 6사례에서 중심과 크기를 유지해야 하고, 과거 top-left의 중심
밀림과 scale-to-fill의 표식 늘어남을 각각 negative control로 재현해야 통과한다.
Core Animation bitmap 합성 검사로, 실제 OS 회전 애니메이션의 화면 캡처는 아니다.
이번 증거 디렉터리는 `Doroti/artifacts/ios-rotation-unscaled/`다.

iPhone 12의 수정 후 합성 6사례는 PASS다. 중심 오차와 20×20 표식의 크기 오차는
모두 0 output pixels이며, resize negative control의 최대 크기 오차는 20 output pixels,
top-left negative control의 최대 중심 오차는 60 output pixels다.
양방향 drawable 크기 변경·transaction 표시·기존 GPU 수명 검사도 통과했다.
원본은 `Doroti/artifacts/ios-rotation-unscaled/lifecycle/runtime.json`에 있다.
실제 OS 회전 애니메이션의 자연스러움은 사용자 재확인이 필요하다.

수정된 Release NativeAOT 게시도 경고·오류 0으로 통과했다. Testbed를 다시 설치해
Graphite 표시·동일 프로세스의 background/resume을 확인했고 실행 상태로 두었다.
게시 및 제품 실행 증거는 같은 디렉터리의 `publish.result.json`과 `product/result.json`이다.

## 이전 변경과 비교 근거

사용자가 이전 transaction 표시 수정 후에도 회전 중심이 어긋난다고 보고했다.
기존 크기 변경 검사는 drawable 크기와 표시 순서만 확인했으며, 중간 레이어 크기에서
이미지 중심이 유지되는지는 검사하지 않았다.

### ScaleToFill 적용 당시 코드 비교와 수정

| 항목 | 저장소의 Flutter 참조 구현 | Doroti iOS 변경 |
| --- | --- | --- |
| 화면 내용의 정렬 | `FlutterView.mm`의 일반 view 초기화·layout은 UIKit의 기본 content gravity를 유지한다. `FlutterMetalLayer.mm`도 이를 덮어쓰지 않는다. | 생성자와 `LayoutSubviews`의 `GravityTopLeft`를 `GravityResize`로 변경하고 `ContentMode.ScaleToFill`을 사용한다. |
| 크기 전환 시점 | `FlutterViewController.mm:873`은 회전 시간의 절반 동안 viewport metrics 전달을 보류한다. 주석에는 종횡비 왜곡 완화 목적과 남는 왜곡이 설명되어 있다. | 기존 layout 내 동기 paint와 transaction 표시를 유지한다. 이번 수정은 내용의 중심 정렬이며 Flutter의 midpoint 지연을 추가하지 않는다. |
| Metal 표시 | Impeller `surface_mtl.mm:292`은 transaction/main-thread 표시에서 commit → waitUntilScheduled → drawable present를 사용한다. | 이전 수정의 iOS layout 표시 경로가 같은 순서를 사용한다. 일반 프레임과 GPU 자원 회수는 비동기를 유지한다. |

`FlutterViewController.mm:1424`의 `contentMode = Center`는 별도 auto-resize 설정
경로다. 일반 전체 화면 회전의 기본값으로 해석하지 않았다.
UIKit/CALayer의 기본 gravity는
[`resize`](https://developer.apple.com/documentation/quartzcore/calayer/contentsgravity)다.
이 설정에서는 이미지의 정규화 중심이 레이어 중심에 대응한다. 기존 top-left 설정은
회전 애니메이션 중 이미지와 레이어의 크기가 다를 때 중심에 오프셋을 만든다.
UIView의 anchor point나 position을 강제로 보정하지 않으며, UIKit이 회전 기하를 소유한다.
Mac Catalyst의 기존 데스크톱 크기 변경 설정은 유지한다.

참조 소스는 `reference/flutter-master/engine/src/flutter/` 아래 파일이다.
이 경로는 별도 git checkout이 아니므로 상위 저장소 HEAD를 Flutter revision으로
기록하지 않았다. 비교 시 파일 SHA-256:

- `shell/platform/darwin/ios/framework/Source/FlutterView.mm`: `3066214d4a5625777d16a331ef1a362c3c0e74ee08ada76873c8e156abd6bd4c`
- `shell/platform/darwin/ios/framework/Source/FlutterViewController.mm`: `df966aadbe8a4f3f29b2d4f6a46fd66ec7892991a9da8c3e007dc1d3ed6e2686`
- `impeller/renderer/backend/metal/surface_mtl.mm`: `cc4f5307da61a6f2c333a08c3a0abdc1913f0eb6964e7731d5e51e03e9aa0278`

### ScaleToFill 적용 당시 검증 범위

수명 검사에 Core Animation bitmap 합성 검사를 추가했다. 실제 production view의
gravity·contentsScale을 사용하는 CALayer에 중심 표식을 그린 세로/가로 이미지를 넣고,
세로·중간·가로 bounds에서 합성한 결과의 픽셀 중심을 측정한다. 일반 UIView의 기본
설정과 비교하고, 이전 top-left 설정에서 오프셋이 재현되어야 통과한다.
실제 Metal 출력이나 OS 회전 애니메이션의 화면 캡처 검사는 아니다.

최초 검사 빌드에서 `ContentsGravity`의 C# 타입을 `NSString`으로 지정한 오류가
발생해 `string`으로 수정했다. 최초 실패는 `Doroti/artifacts/ios-rotation-center/lifecycle/`에
보존하며, 수정 후 검사는 `lifecycle-final/`에 기록한다.

수정 후 iPhone 12 결과: **PASS**. 6개 합성 사례의 최대 중심 오차는 **0 output pixel**,
이전 top-left negative control은 **60 output pixels**였다. output renderer의 scale은 1이며,
기기 화면에서 실측한 물리 픽셀 오차로 해석하지 않는다. 양방향 크기 교환 2회,
transaction paint 2회, 비활성화·복귀, 세 프레임 제한, 취소, 지연 GPU 회수도 통과했다.
원본: `Doroti/artifacts/ios-rotation-center/lifecycle-final/runtime.json`.

Release NativeAOT 게시도 경고·오류 0으로 통과했다. 수정한 Testbed를 아이폰에 다시
설치해 Graphite 표시와 동일 프로세스의 background/resume을 확인했고, 실행 상태로
두었다. 두 표시 검사에서 실패 프레임은 0이다.
빌드 증거는 `Doroti/artifacts/ios-rotation-center/publish.result.json`,
제품 실행 증거는 `Doroti/artifacts/ios-rotation-center/product/result.json`에 있다.

실제 기기 회전의 체감 개선 여부는 사용자 재확인이 필요하다.
