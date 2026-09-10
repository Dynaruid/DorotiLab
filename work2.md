# Doroti Mono 제거·NativeAOT 전환 작업계획

작성일: 2026-09-10. 상태: **계획 작성 완료, NativeAOT 구현·publish·실기기 검증 미착수**.

재검토 기준 커밋: `0538597fafe52954b898b008adde110a79bd1ca0`. 최초 계획의 기준은 `3ff6f464d51a67ef412d9c16f68c4d63ced9f1dc`였다. 이번에는 `ref1.md`, 현재 제품 소스·빌드 설정·기존 검증 보고서와 공식 문서를 대조하여 이 문서만 수정했다. 빌드·publish·기기 연결은 실행하지 않았다. 기존 Mono 트리밍 실험의 성공을 NativeAOT 성공으로 취급하지 않는다.

## 1. 목표와 완료의 의미

**최우선 목표는 배포 앱에서 Mono 런타임·인터프리터 의존을 제거하는 것이다.** 기존 계획의 첫 전환 타깃인 iOS arm64에서 DorotiTestbedApp의 전체 기능을 유지하는 NativeAOT 앱을 publish·서명·설치하고, iPhone에서 정상 동작하도록 만든다. 현재 Doroti 제품 코드를 직접 정비하여 이후 새 앱과 새로운 위젯도 같은 경로로 배포할 수 있게 한다. 번역기 수정은 별도의 제한된 개선 작업으로 둔다.

Mono full trimming 개선, Android `RunAOTCompilation=false`, iOS 해석 실행 확대는 이 목표의 대안이나 선행 최적화 단계가 아니다. 기존 Mono 산출물은 동작·크기 비교와 전환 중 복구에만 사용한다. NativeAOT에도 GC 등 자체 .NET 런타임 지원은 남으므로, “Mono 제거”를 “모든 .NET 런타임 코드 제거”로 정의하지 않는다. iOS 완료를 Android·Web 등 저장소 전체의 Mono 제거 완료로 발표하지 않는다.

확정 범위: **현재 프레임워크 C# 코드가 제품의 기준이다.** 이미 Flutter와 의도적인 차이가 생겼으므로, 번역기가 이 코드를 동일하게 재현하거나 Flutter 소스로부터 프레임워크 전체를 재생성하는 것은 목표가 아니다. 제품 수정 내용을 번역기에 모두 역반영해야 한다는 조건도 두지 않는다.

성공을 세 단계로 구분한다.

| 단계               | 완료 조건                                                                            | 이것만으로 전체 완료인가                |
| ------------------ | ------------------------------------------------------------------------------------ | --------------------------------------- |
| G1: 기반 호환성    | 현재 MAUI·SkiaSharp·Metal·네이티브 바인딩으로 작은 NativeAOT 앱을 iPhone에서 실행    | 아니오. 외부 의존성의 조기 검증         |
| G2: 실제 앱 성공   | 전체 Testbed를 NativeAOT로 publish하고 화면·입력·비동기·네이티브 연동·수명 검증 통과 | 앱 전환 성공. 제품 SDK 배포 검증은 별도 |
| G3: 제품 경로 완성 | iOS 정식 배포 기본 경로를 NativeAOT로 전환하고 Runner/CLI·NuGet 소비·새 템플릿·CI 검증 | **NativeAOT 전환의 전체 완료 조건**     |

G2/G3의 공통 조건:

- `dotnet publish`가 실제 NativeAOT 컴파일러와 네이티브 링크를 수행한다. 일반 `dotnet build`, Mono AOT, 시뮬레이터 실행만으로 대신하지 않는다.
- 최종 배포 번들 및 링크 결과에 Mono 런타임·인터프리터·Mono AOT 이미지가 없고, JIT·DLR 런타임 코드 생성에 의존하지 않는다. §4.4의 정적 링크·번들·실행 증거를 모두 확보한다. 기존 `MtouchInterpreter=-all`은 NativeAOT 해결책이 아니다.
- 제품과 배포 의존성의 미해결 AOT/트리밍 경고가 없다. 전체 경고 숨김, `ILLinkTreatWarningsAsErrors=false`, 광범위한 멤버 보존으로 성공 판정을 만들지 않는다.
- 샘플을 단순화하거나 실패하는 위젯·지역화·셰이더·진단 화면을 빼서 크기와 실행 성공을 얻지 않는다.
- 현재 지원하는 외부 앱의 사용자 정의 Widget/State/Route/Action/LocalizationsDelegate 확장이 가능하다. Testbed 타입만 하드코딩한 구현은 불합격이다.
- 기능 검증 결과, 실제 산출물, 빌드 로그, 크기·성능 비교, 사용한 커밋·도구 버전을 저장소의 재현 가능한 보고서로 남긴다.

크기 절감은 전환 효과를 평가하는 별도 지표다. 동일 커밋·기능·리소스의 Mono 기준과 비교하되 절감률을 Mono 제거의 성공 조건으로 삼지 않는다. **Mono 제거·기능 호환성·크기 변화·성능 판정을 각각 기록한다.** 크기가 기대보다 크면 최적화 backlog로 남기며 Mono 유지로 목표를 대체하지 않는다. 입력·시작·프레임 성능의 허용 범위를 넘는 회귀는 제품 승격을 막지만 Mono 제거 여부와 구분한다. 아직 측정하지 않은 “50MB 이하” 같은 수치를 확약하지 않는다.

## 2. 범위

| 구분                | 이번 계획의 책임                                                                                                     |
| ------------------- | -------------------------------------------------------------------------------------------------------------------- |
| 우선 배포 타깃      | `net10.0-ios` / `ios-arm64`, 과거 검증에 사용한 iPhone 12를 우선 후보로 삼되 Mac·서명·기기 가용성은 N0에서 확인       |
| 공용 제품 코드      | Runtime, Ui, Hosting, Framework 전체 계층, Skia 렌더링·runtime effects, 앱 시작·등록 계약                            |
| iOS 통합            | MAUI host, UIKit/Metal/Graphite 및 기존 지원 렌더 경로, native binding, Runner SDK, Testbed                          |
| 제품 생성·배포      | bootstrap·등록 코드, 템플릿·패키지·CLI·검증 도구                                                                     |
| 번역기 보조 개선    | 선택한 타입 매핑·호출 lowering의 품질 개선. 현재 프레임워크 동일 재현·전체 재생성·모든 제품 수정의 역반영은 제외     |
| 타 플랫폼           | 공유 코드 변경의 Android·Web·Windows·AppKit·Mac Catalyst·Linux 회귀 방지. 실제 테스트 불가 환경은 미검증으로 남김    |
| 후속 NativeAOT 확장 | Mac Catalyst 등은 타깃별 지원성과 publish 검증 후 별도 승격. iOS 성공을 전체 플랫폼 NativeAOT 지원으로 발표하지 않음 |

Android는 현재 Mono AOT 기본 동작을 보존하는 공유 코드 회귀 범위다. Android에서 Mono를 제거하는 후속 작업은 해당 SDK의 NativeAOT 지원·JNI/native binding·Vulkan host를 별도로 검증해야 한다. 단순 AOT-off나 CoreCLR 교체를 이번 iOS NativeAOT 완료로 계산하지 않는다. Web의 Worker/.NET WASM 실행 구조도 별도 전환 범위다.

번역기·Roslyn·빌드 도구 자체의 NativeAOT화는 요구하지 않는다. 이들은 **앱 실행 시 의존성**과 분리하여 분석한다. 일반 앱이 아닌 빌드 도구에서 사용하는 리플렉션까지 무조건 제거하지 않는다.

Flutter 소스와 테스트는 의미를 이해하고 비교하는 참고 자료로 사용한다. 현재 Doroti의 의도적인 차이는 제품 계약과 회귀 테스트로 명시하여 유지한다. NativeAOT 전환을 계기로 Flutter와 다른 부분을 일괄 원복하거나 번역 결과로 제품 소스를 덮어쓰지 않는다. bootstrap·JSON context·native binding 등 빌드에 필요한 코드 생성은 이 번역기 범위 축소와 별개로 계속 검증한다.

## 3. 검토 결과와 기준선

### 3.1 이미 확보한 자산

[iPhone 트리밍 보고서](Doroti/docs/validation/ios-trimming-2026-09-10.md)에 다음 결과가 있다.

| 기존 실험                        | 로컬 `.app` bytes | 실행 파일 bytes | 의미 |
| -------------------------------- | ----------------: | --------------: | ---- |
| 부분 트리밍 + Mono AOT           | 136,038,224 | 88,303,792 | 호환성 수정 전 과거 기준선 |
| 전체 트리밍, 동적 멤버 보존 없음 | 96,677,312 | 58,291,376 | 지역화·첫 화면 준비에서 `NoSuchMember` 발생 |
| 전체 트리밍 + 동적 멤버 보존     | 105,013,528 | 63,839,616 | iPhone에서 샘플·진단·MediaQuery 렌더링 성공, 1,691개 빌드 경고 잔존 |

105MB 빌드는 NativeAOT가 아니다. 해당 검증의 초기 스냅샷은 pointer event가 0이므로 전체 상호작용 검증도 아니다. 값은 서명된 `.app`의 일반 파일 길이 합이며 decimal MB를 사용한다. IPA 압축 크기·App Store 전송량·기기 설치 저장 공간과 다르다. 31,024,696 bytes 절감에는 JSON/호환성 수정도 포함되어 있으므로 “31MB 전부가 trim 속성 하나의 효과”라고 단정하지 않는다. N0/N10에서 통제된 비교를 만든다.

이미 도입된 개선은 재사용한다.

- `IRenderObjectWithChild`, `IContainerRenderObject`, `RouteBase`, `IIntentAction` 등 정적 계약과 focused dynamic-dispatch 검증.
- `DorotiApplicationFactory.Create<TStartup>` 및 생성된 bootstrap/plugin 등록. 처음부터 문자열 기반 자동 발견 시스템으로 다시 만들지 않는다.
- application manifest, native platform bridge, MAUI evidence, Skia font-fallback key, MediaQuery의 생성형 JSON context.
- Dart JSON 채널 값의 명시적 writer와 `validation/json-codec`의 실제 full-trim 실행 검증.
- LayoutBuilder의 자식 연결을 리플렉션 대신 인터페이스로 처리한 수정.

### 3.2 현재 소스에서 확인한 장애물

아래는 소스 조사 결과다. NativeAOT publish 경고 전체 목록을 이미 확보했다는 뜻은 아니다.

| 위치                                                     | 확인한 사실                                                                  | 필요한 작업                                                                             |
| -------------------------------------------------------- | ---------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- |
| `Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets`           | iOS/Catalyst에서 `MtouchInterpreter=-all` 기본 주입                          | Mono/NativeAOT 프로필 분리, NativeAOT에 Mono 전용 설정 유입 방지                        |
| `DorotiTestbedApp/ios/TrimmerRoots.xml`                  | 여러 assembly에서 살아남은 타입의 모든 멤버 보존                             | NativeAOT 성공 조건에서 제외하고 동적 호출 자체를 정적 계약으로 전환                    |
| `Doroti.Framework.Widgets/localizations.cs`              | heterogeneous delegate의 `type/isSupported/load/shouldReload`를 dynamic 호출 | 비제네릭 공통 계약 + 제네릭 구현의 typed bridge                                         |
| `Doroti.Framework.Widgets/view.cs`                       | `renderObject.prepareInitialFrame`, 자식 접근에 dynamic 사용                 | RenderView 및 기존 render-child 계약으로 연결                                           |
| `Doroti.Framework.Widgets/layout_builder.cs`             | 자식 연결은 개선됐지만 layoutInfo 등 다른 dynamic 경로가 남음                | box/sliver 공통 layout callback·제약 전달 계약 정비                                     |
| Widgets/Material/Cupertino 전반                          | 상태·callback·컬렉션·렌더 객체·연산자에 dynamic 사용                         | 용도별 분류 후 계층별 전환; 단순 문자열 치환 금지                                       |
| `Doroti.Runtime/DartAsync.cs`, `DartCoreAdapters.cs`     | error handler/predicate의 `Delegate.DynamicInvoke`, `Method.GetParameters()` | 명시적 callback 형태와 반환값 처리, 비동기 의미 보존                                    |
| `Doroti.Runtime/FoundationRuntimePorts.cs`               | `EnumIndex`가 `GetProperty("index"/"value")` 사용                            | enum/FontWeight/indexable 값의 명시적 계약                                              |
| `Doroti.Ui/FrameworkShaderAssets.cs`                     | assembly 열거 후 이름으로 `Assembly.Load` fallback                           | 생성·명시 등록된 resource owner와 stream factory 사용                                   |
| `Doroti.Host.Maui/DorotiGraphiteView.cs`                 | `SKGLView`를 입력/속성 계약으로 상속하며 Graphite handler는 별도 GPU surface를 사용 | 상속·등록에서 도달하는 MAUI control 경계 검사. 상속만으로 GL 렌더러 사용이나 AOT 불가를 단정하지 않음 |
| `DorotiMauiApplication.cs`                               | `UseSkiaSharp`, generic handler·서비스 등록, Graphite/기존 view 등록         | 실제 root graph와 생성된 registrar, DI·callback 경고 검증                               |
| `DorotiTestbedApp/ios/binding/ApiDefinition.cs`          | ObjC export와 `Action<string>` completion bridge                             | NativeAOT binding/registrar·역방향 callback·수명 실기기 검증                            |
| `tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/` | Types/Expressions 등에서 `dynamic`과 `((dynamic)this.renderObject)`를 출력   | 보조 개선 후보. 선택한 패턴의 출력 품질을 개선하되 제품 프레임워크 재현을 요구하지 않음 |
| 제품 프로젝트 설정                                       | 공통 `IsAotCompatible`/AOT 분석 정책이 아직 없음                             | 계층별 분석 도입과 앱 publish 검증 병행                                                 |

최초 계획에 기록된 단순 텍스트 집계는 다음과 같다. 이번 재검토에서 전체 재집계하지 않았으며 N0에서 갱신한다. `bin/obj`를 제외한 C# 파일에서 `\bdynamic\b`를 세었으며 주석·선언·문자열도 포함할 수 있다. **이 수치는 실제 DLR call-site 수나 수정 건수가 아니다.**

| Framework assembly | 포함 파일 수 | dynamic 토큰 수 | 명시적 `(dynamic)` 수 |
| ------------------ | -----------: | --------------: | --------------------: |
| Widgets            |           80 |             966 |                   586 |
| Material           |           58 |             499 |                   277 |
| Cupertino          |           25 |             263 |                   128 |
| Painting           |            5 |              17 |                     2 |
| Rendering          |            1 |               2 |                     2 |
| 합계               |          169 |           1,747 |                   995 |

N0에서 Roslyn 의미 분석과 생성 IL 검사로 실제 호출·도달성을 다시 집계한다. `DartMap<Type, dynamic>` 같은 데이터 선언, `JsonElement.GetProperty` 같은 JSON API, Windows 전용 조건부 코드를 iOS 런타임 리플렉션 장애물로 잘못 세지 않는다. `DynamicInvoke` 역시 발견만으로 모든 형태가 불가능하다고 단정하지 않고 호출 형태와 실제 publish 진단을 함께 평가한다.

### 3.3 고정해서 조사할 외부 의존성

현재 `Doroti/global.json`, `Directory.Packages.props`, Runner props 기준:

- .NET SDK `10.0.400`, 이전 실험의 .NET/ILLink `10.0.11`, Microsoft.iOS SDK `26.5.10315`.
- `Microsoft.Maui.Controls` `10.0.90`.
- `SkiaSharp`, `SkiaSharp.Views.Maui.Controls` 계열 `4.154.0-preview.1.26454.9`와 실제 iOS native framework.
- `MaterialColorUtilities` `0.3.0`; 그 외는 restore graph에서 전이 의존성까지 조사.
- native Xcode framework, 생성된 binding assembly, 실제 Xcode/SDK/배포 최소 OS 버전.

이 버전들이 NativeAOT 호환이라고 아직 판정하지 않는다. 해결에 버전 변경이 필요하면 해당 패키지·워크로드 단위로 재현 가능한 변경을 만들고, 관련 없는 플랫폼 패키지까지 일괄 업그레이드하지 않는다.

### 3.4 `ref1.md` 적용 판단

참고 문서의 **DLR 실행 의존을 정리하여 NativeAOT로 전환한다**는 방향은 현재 구성에 맞는다. 다만 Mono 최적화부터 진행하는 로드맵은 사용자의 Mono 제거 목표에 맞지 않으므로 채택하지 않는다.

| 참고 문서의 주장·제안 | 현재 근거와 판단 | 계획 반영 |
| --- | --- | --- |
| iOS 136 → 105MB, 실행 파일 88.3 → 63.8MB | §3.1 보고서 수치와 일치. 수정 전후 비교이며 원인별 기여도는 미분해 | 과거 참고값만 유지, 절감 확약 금지 |
| 큰 Mach-O에 Mono AOT·Skia·MAUI 등이 함께 들어감 | 링크된 코드의 구성 후보로 타당. 실행 파일 전체를 Skia 또는 Mono의 크기로 귀속할 수 없음 | native link map과 section 자료로 분해, 귀속 불가분은 unknown |
| `MtouchInterpreter=-all` 유지 | 현재 Runner 설정과 일치. 모든 assembly를 AOT하면서 동적 생성 메서드에 interpreter를 허용하는 뜻이며 `all`과 다름 | 기존 Mono 비교에만 유지. NativeAOT에선 효과가 없으며 제거 대상 ([공식 설명](https://learn.microsoft.com/en-us/dotnet/maui/macios/interpreter?view=net-maui-10.0)) |
| full trim + descriptor로 dynamic 해결 | 현재 descriptor는 `required="false"`로 살아남은 타입의 멤버를 넓게 보존. DLR 코드 생성 의존은 남음 | Mono 호환성 우회로만 기록, NativeAOT 계약으로 채택하지 않음 |
| Android x64 APK 49,818,774 bytes / AOT 128개 | [시작 ANR 수정 보고서](Doroti/docs/validation/android-aot-startup-2026-09-10.md)와 일치. 모듈 개수만으로 AOT 총 bytes는 알 수 없음 | Android 회귀 기준. iOS 용량 근거로 전용하지 않음 |
| ABI 두 개이므로 약 100MB | RID-neutral Host가 두 ABI를 포함하는 것은 확인. 실제 runner는 선택 RID별 target을 참조하며 최종 AAB의 두 ABI 포함·100MB 원인은 미확인 | 실제 archive entry 없이 합산 추정하지 않음. AAB와 기기별 split 배포량 구분 |
| Android AOT-off를 먼저 비교 | Mono는 남고 JIT 부담이 증가. 위 x64 보고서에서 AOT-off 시작 ANR 재현 | Mono 제거 경로에서 제외, 현재 Release AOT 정책 보존 |
| Graphite/Skottie/codec이 용량을 차지 | `Doroti/native/graphite/build-android.py`에서 빌드 옵션 확인. Linux 12,941,848 bytes는 Linux 자료로만 유효 | iOS/Android native 산출물별로 측정. Graphite·shader·codec 삭제를 기본 해법으로 삼지 않음 |
| `DorotiDynamic.InvokeMember` / generated dispatch로 대체 | 이름만 바꾼 reflection/DLR wrapper면 문제가 그대로 남음 | §4.1의 정적 계약 우선. 잔여 호출에만 빌드 시 확정된 typed thunk 사용 |

MAUI 기본 앱의 NativeAOT 용량 예시는 가능성을 보여주는 외부 사례이며 Doroti 예상 크기가 아니다. APK/AAB 압축 크기, ABI별 비압축 `.so`, iOS `.app` raw 합계는 직접 비교하지 않는다. Google Play는 기기에 맞는 APK를 생성하므로 AAB 업로드 용량이 기기 전송량과 같지 않다. ([Android App Bundles](https://developer.android.com/guide/app-bundle))

## 4. 전환 설계 원칙

### 4.1 현재 Doroti 계약을 유지하는 정적 호출

1. 타입이 이미 알려진 수신자는 구체 타입·기존 인터페이스로 호출한다. 상속/override, 명시적 interface 구현, 접근성을 먼저 확인한다.
2. 여러 `T`를 한 컨테이너에 보관해야 하면 비제네릭 동작 계약을 두고 `T`는 구현 내부에서 유지한다. Route/Action의 기존 접근을 Localizations, State, render/layout, callback 경계에 적용한다.
3. callback은 `Action`/`Func` 또는 명시적 adapter로 호출한다. parameter 개수·타입을 런타임 리플렉션으로 추측하지 않는다.
4. 현재 Doroti에서 동적 동작이 필요한 지점은 제품 코드의 명시적 adapter·연산 계약으로 바꾼다. 반환 타입, named/optional 인수, null, numeric promotion, operator, library-private 접근 및 현재 지원하는 `noSuchMethod` 의미를 보존한다. 필요한 등록 코드는 앱 빌드에서 생성할 수 있지만 Dart 번역기 개선을 전제로 하지 않는다.
5. 열린 사용자 확장은 공개 계약 또는 최종 앱 빌드에서 생성하는 등록 경로로 지원한다. 새 타입마다 저장소 본체에 case를 추가해야 하는 Testbed 전용 whitelist를 만들지 않는다.
6. 제품 코드에서 처리하지 못하는 호출은 계약과 오류 동작을 명확히 정한다. `null/default`, 호출 무시, reflection/Expression.Compile/DLR fallback으로 숨기지 않는다. 번역기에서 개선 대상으로 선택한 lowering도 지원하지 못하면 명시적 compiler 진단을 내도록 한다.

모든 타입을 하나의 거대한 switch로 바꾸는 것도 목표가 아니다. 정적 dispatch가 제네릭 인스턴스 수·boxing·코드 크기를 과도하게 늘리는지 N10에서 측정한다.

계약별 구현 선택을 다음처럼 고정한다. 아래 이름은 설계 후보이며 아직 추가된 API가 아니다.

| 호출군 | 우선 구현과 확인할 의미 | 담당 단계 |
| --- | --- | --- |
| RenderView/child/layout | 구체 `RenderView`, 기존 `IRenderObjectWithChild`/container 계약, layout callback 계약. attach/detach·box/sliver 제약·override 유지 | N4 |
| `LocalizationsDelegate<T>` | 비제네릭 `ILocalizationsDelegate` 후보에 type/support/load/reload bridge 제공. `T`와 `Future<T>`는 구현 내부에서 유지하고 동기 완료·locale 변경 중 늦은 완료·서로 다른 delegate 타입의 reload를 검증 | N4 |
| State/Route/Action | 기존 generic 구현 + 비제네릭 동작 계약. 외부 assembly의 새 타입도 override와 결과형 보존 | N4~N6 |
| callback/연산 | typed adapter와 현재 숫자·null·오류 계약. 지원 함수 형태를 API 경계에서 명시하고 등록 후 `DynamicInvoke`로 재진입하지 않음 | N2/N5 |
| 위 계약으로 표현할 수 없는 실제 동적 멤버 호출 | 필요한 경우 호출 지점·receiver 계약·멤버·인수 형태별 generated typed thunk. 읽기/쓰기/호출/연산을 구분하고 닫힌 generic 대상은 빌드 시 생성 | N2 설계, N5/N6 적용 |

generated dispatch를 도입한다면 N2에서 작은 제품 fixture로 필요성을 먼저 입증한다. 등록은 앱 composition root/generated bootstrap에서 하고 NuGet 외부 타입 확장과 충돌·누락 진단을 포함한다. final app의 알려진 타입은 빌드 진단으로 검증하며, 실제 런타임의 지원 밖 receiver/멤버는 현재 제품 오류 계약을 따른다. 런타임 assembly 검색, `MakeGenericType`로 임의 닫힌 타입 생성, `Expression.Compile`/DLR fallback은 대체 구현이 아니다. 호출 캐시는 타입 수·수명·threading이 제한되어야 한다. 번역기 전체 개편 없이 현재 C# 제품 코드에서 적용한다.

### 4.2 리플렉션·등록·리소스

- bounded reflection은 분석 가능한 타입 흐름과 필요한 범위의 annotation으로 지원할 수 있다. 리플렉션 API라는 이유만으로 전면 금지하지 않는다.
- `DynamicallyAccessedMembers`/`DynamicDependency`는 실제 계약에 필요한 좁은 범위에만 사용한다. 보존 annotation은 런타임 코드 생성을 지원하게 만드는 기능이 아니다.
- shader/resource/plugin owner는 `typeof(Owner).Assembly`, 직접 factory 또는 generated registry로 연결한다. 공유 Ui가 상위 Material assembly를 역참조하여 순환 의존성을 만들지 않도록 composition root에서 주입한다.
- embedded resource 이름·해시·실패 처리·비동기 로딩 의미를 유지한다. assembly 파일 경로가 존재한다는 가정도 제거한다.
- **SkSL/runtime shader의 GPU 컴파일은 .NET 런타임 코드 생성과 별개다.** NativeAOT를 이유로 FragmentProgram/runtime effects를 제거하지 않는다.

### 4.3 빌드·실험 경로

- 공용 라이브러리는 분석 프로필로 전체 코드의 경고를 먼저 수집하고, 정비된 계층부터 `IsAotCompatible=true`를 적용한다. 선언 자체를 호환성 증거로 삼지 않는다.
- NativeAOT는 지원 TFM/배포 프로필로 선택하고 Debug/Release 사이에 feature switch 의미가 달라지지 않도록 한다. publish 검증은 Release로 수행한다.
- NativeAOT 프로필에서는 `PublishAot=true`를 사용하며 `TrimMode`를 별도로 강제하지 않는다. NativeAOT의 full trimming을 사용한다. [공식 MAUI trimming 문서](https://learn.microsoft.com/en-us/dotnet/maui/deployment/trimming?view=net-maui-10.0)
- Mono 프로필과 NativeAOT 프로필의 obj/bin, restore graph, 생성 bootstrap, native binding 산출물, launch 재사용 키를 구분한다. 기존 `ArtifactsPath`/RID별 경로와 맞물리는 부분까지 검증한다.
- 첫 실패 분석에서는 경고를 수집할 수 있으나, 이 설정을 정식 완료 판정·템플릿·기본 빌드에 남기지 않는다. 최종 native publish는 분석을 켜고 경고를 오류로 처리한다.

### 4.4 Mono 제거를 증명하는 계약

G1의 probe, G2의 Testbed, G3의 템플릿/NuGet 소비 앱에 같은 검사기를 사용한다.

1. **빌드 선택:** 평가된 TFM/RID/`PublishAot`·feature switches·compiler/link 실행 로그를 저장한다. `dotnet publish`가 실제 ILC와 native link를 실행해야 한다. 속성 출력만으로 성공 처리하지 않는다.
2. **링크 입력과 포함 코드:** SDK runtime pack, native linker response/input, 가능한 link map에서 Mono runtime/interpreter/Mono AOT 모듈의 링크·포함 여부를 확인한다. iOS에서는 Mono가 정적 링크될 수 있어 `.dylib` 부재나 `otool -L`만으로 제거를 증명할 수 없다. SDK 버전별 입력 이름·역할을 기준으로 검사하고 이름이 불명확하면 미판정으로 남긴다.
3. **최종 번들:** 서명·배포할 `.app` 및 IPA payload의 파일 목록·크기·SHA-256, executable의 load command와 가능한 symbol/section 자료를 연결한다. stripping된 바이너리에서 `mono` 문자열 검색 결과가 0인 것은 보조 자료일 뿐이다. NativeAOT의 자체 GC/runtime 지원을 Mono로 오분류하지 않는다.
4. **실제 실행:** 검사한 bundle의 설치·launch identity를 기록하고 기능·native callback·수명을 검증한다. runtime feature 값은 보조 증거다. 자동 Mono fallback, 다른 Mono 빌드 설치, stale artifact 재사용이 있으면 불합격이다.

결과에는 `monoAbsent=pass|fail|notVerified`, `nativeAotPublish`, `functional`, `performance`, `sizeDeltaBytes`를 별도 필드로 남긴다. `monoAbsent=pass`에는 1~3의 일치하는 증거가 필요하고 G2/G3 완료에는 4도 필요하다. 외부 도구나 앱 빌드에 쓰는 Roslyn의 런타임은 배포 앱의 Mono 제거 판정 대상이 아니다.

### 4.5 크기 보고서와 비교 규칙

N0에서 `Doroti/validation/native-aot/size-report.py`를 후보로 삼아 Mac/Windows에서 읽을 수 있는 JSON 보고서를 설계한다. N10에서 최종 비교에 사용하며 이번 계획 수정에서는 도구를 구현하지 않는다.

- 입력: bundle/archive 경로, 플랫폼·RID·mode·커밋·도구 버전·native provenance, 선택적 linker map. 산출물 파일을 읽고 원본을 변경하지 않는다.
- `.app`: 일반 파일의 bytes 합계, executable, frameworks/dylibs, fonts/images/shaders, 기타 리소스를 중복 없이 집계한다. symlink는 별도 표시하여 중복 합산하지 않는다. dSYM은 배포 bundle과 별도 기록한다.
- `.apk`/`.aab` 분석이 필요하면 ABI별 `libaot-*`, Skia, Mono/플랫폼 glue, managed assembly blob, dex/resources, 기타를 분리한다. ZIP entry의 비압축/압축 bytes와 archive 자체 bytes를 모두 보존하고 header·alignment·서명 차액도 표시한다. packed blob을 이름만 보고 Doroti/MAUI/기본 라이브러리별로 임의 분배하지 않는다.
- iOS의 정적 링크된 코드 기여도는 map에서 확인 가능한 부분만 분류한다. 최종 section 합계와 다른 map 추정치를 같은 정확도로 표시하지 않고 unknown을 허용한다.
- MB는 `bytes / 1,000,000`, MiB는 `bytes / 1,048,576`로 명시한다. `.app` raw·IPA compressed·설치 공간·스토어 전송량은 각각 독립 필드다. device-specific Android 전송 추정이 필요할 때만 `bundletool`을 사용한다. ([bundletool 크기 측정](https://developer.android.com/tools/bundletool))
- Mono 비교는 같은 소스·renderer·리소스·RID·서명 및 진단 설정으로 만든다. 보존 descriptor가 필요한 Mono와 불필요한 NativeAOT의 차이는 의도된 차이로 기록한다. 별도 Mono 최적화나 native 기능 삭제를 섞지 않는다.

## 5. 단계별 작업

필수 단계는 **구현 → 해당 계약 검증 → 가능한 실제 publish → 증거 기록**까지 수행한다. 빌드가 한 번 통과했다는 이유로 후속 단계를 생략하지 않는다. **N3은 별도 보조 개선이며 N0~N2, N4~N12의 착수·완료나 G3 판정을 막지 않는다.** 아래 체크박스는 이번 계획 작성으로 완료되지 않는다.

### N0. 기준선·전체 장애물 목록·실험 격리

- [ ] 현재 커밋, SDK/workload/Xcode/native 라이브러리 해시, 잠금 파일, signing 및 지원 OS를 기록한다.
- [ ] Mac/Xcode/workload·서명·iPhone의 실제 가용성을 확인하고 Windows에서 가능한 소스/계약 검증과 분리한다. 장비가 없으면 native/device 항목을 `notVerified`로 남기되 독립적인 계약 정비는 진행할 수 있다.
- [ ] 기존 Mono 배포 프로필 하나를 주 비교군으로 정하고 기능·크기·startup/frame/memory 기준선을 확보한다. 기본 partial trim을 우선 사용하며, 기존 guarded full trim은 보조 비교로만 둔다. 새로운 Mono 최적화 작업을 추가하지 않는다.
- [ ] §4.5 보고서 형식·집계 도구와 §4.4 제거 판정의 수집 항목을 만든다. 기존 임시 Mac 증거의 존재·해시를 확인하고 사라진 원자료는 재확보 전까지 과거 보고서 값으로 표시한다.
- [ ] 제품 프로젝트·패키지 그래프와 실제 앱 reachability를 만든다. `id, assembly, source/member, operation, receiver/type-flow, Debug/Release reachability, warning/IL evidence, replacement, fixture, status`로 장애물을 관리한다.
- [ ] C# dynamic 호출, IL의 Binder/CallSite, reflection/assembly loading, delegate invocation, serializer, generic reflection, native callback을 구분해 조사한다.
- [ ] 분석 전용 profile과 별도 산출물 경로를 만들고 최초 iOS NativeAOT publish 실패 로그/binlog를 확보한다. 실패가 예상되는 진단 실행이며 배포 성공으로 기록하지 않는다.
- [ ] 현재 제품 소스와 빌드에 필수인 bootstrap·등록·binding 생성의 소유권을 확인한다. 번역기의 과거 manifest/재생성 체계 복구를 NativeAOT 선행 작업으로 넣지 않는다.
- [ ] N1/N4가 사용할 최소 opt-in NativeAOT profile을 먼저 만든다. N8은 이를 정식 Runner/CLI/패키지로 완성하는 단계이며 최초 publish를 N8까지 미루지 않는다.
- [ ] 같은 기기에서 cold start 5회, 대표 조작 시나리오 3회 등 제한된 baseline으로 측정 변동과 허용 회귀율을 문서화한다. ANR/crash/입력 누락은 0건을 요구하고 startup·frame p95·메모리의 수치 기준은 후보 결과를 보기 전에 확정한다.

**완료 조건:** 재현 가능한 초기 실패, 소유자가 정해진 장애물 목록, 크기 측정 정의와 성능 합격 기준, 기존 앱과 충돌하지 않는 실험 디렉터리가 있다.

### N1. 외부 의존성·네이티브 호스트 조기 검증 — G1

- [ ] 작은 iOS NativeAOT probe를 만들어 MAUI 창 → 현재 Graphite/Metal view → Skia 도형/텍스트/이미지 → runtime shader 순서로 확장한다.
- [ ] 현재 SKGLView/UseSkiaSharp 경로를 먼저 검증한다. string binding·implicit conversion·generated registrar가 막히면 최소 재현을 분리한다.
- [ ] 해결 순서는 지원 버전/공식 수정 확인 → 최소 패치 → 필요한 경우 SKGLView 상속과 광범위 handler 등록을 제거한 Doroti 소유 View/handler로 한정한다. 렌더러 자체 교체를 기본 해법으로 삼지 않는다.
- [ ] iOS native framework의 `platformInfo`, echo, main-thread completion을 실제 호출한다. ObjC export, 생성 binding/registrar, callback 수명·GC·예외 전달을 검증한다.
- [ ] MaterialColorUtilities는 representative 색상 계산을 포함한 NativeAOT probe로 별도 확인한다.

**완료 조건:** 실제 iPhone에서 렌더링과 native round-trip이 되는 작은 NativeAOT 산출물이 있다. dependency blocker가 있다면 해결 PR/버전/교체 작업까지 추적한다. 빈 MAUI 창만 떴다고 G1을 닫지 않는다.

§4.4의 Mono 제거 증거도 probe에 남긴다. 이 단계의 dependency blocker가 미해결이면 N4~N6의 대규모 전환에 앞서 원인·지원 버전·최소 패치 가능성을 확정한다. N2의 독립적인 계약 조사/fixture는 진행 가능하나 외부 기반이 검증됐다고 가정하지 않는다.

### N2. 런타임의 명시적 호출·값 계약

- [ ] `DartAsync.cs`의 error callback을 인수 1개/2개, void/값/Future/Task 반환 등 실제 지원 형태로 구분하고 typed adapter를 제공한다.
- [ ] `DartCoreAdapters.cs`의 predicate 호출 및 `EnumIndex`의 enum/FontWeight/indexable 처리를 명시적 계약으로 전환한다.
- [ ] 기존 공개 API와 현재 제품 코드의 call-site를 함께 이전한다. 사용자 callback을 조용히 무시하거나 기본값으로 바꾸지 않는다. 번역기 반영이 유용한 항목은 N3 후보로 따로 기록한다.
- [ ] §4.1의 호출군별로 직접 호출/비제네릭 bridge/typed callback/필요 시 generated thunk 선택과 API 변경 영향을 기록한다. 새 registry가 필요하면 외부 assembly 확장·누락 진단·오류 의미·호출 수명을 포함한 작은 NativeAOT fixture부터 통과시킨다.
- [ ] FutureOr 처리, 오류 회복·재전파, stack 보존, SynchronousFuture, microtask 순서, cancellation, timeout을 검증한다.
- [ ] `runtime-async-contract`, `json-codec`를 재사용하고 필요한 focused fixture를 실제 NativeAOT console publish로 실행한다. 기존 JSON 검증은 full trim이었으므로 NativeAOT 검증을 추가한다.

**완료 조건:** 지원하는 Runtime public 계약을 호출하는 NativeAOT 소비 fixture가 통과하고 해당 계약에 unresolved IL2xxx/IL3xxx가 없다.

### N3. 번역기의 제한된 보조 개선 — NativeAOT 필수 경로 밖

- [ ] 제품 전환 중 발견한 패턴에서 번역기 자체에도 유용한 개선만 선정한다. 예: 알려진 receiver 타입 유지, 불필요한 dynamic cast 감소, callback 타입 추론, 분명한 호출의 정적 출력.
- [ ] `FrameworkCSharpLowerer.Types.cs`, `Expressions.cs` 등 선정한 항목의 범위에서 IR/타입 매핑/lowering을 개선한다. 프레임워크 전체의 재현을 위한 IR·port 체계 전면 개편은 포함하지 않는다.
- [ ] 개선한 패턴에 작은 Dart→C# fixture를 두고 컴파일·동작을 검증한다. NativeAOT 호환 출력을 표방하는 해당 fixture에만 실제 NativeAOT publish/실행 검증을 추가한다.
- [ ] 실패할 수밖에 없는 입력은 명시적 진단으로 남긴다. 도구의 지원 범위를 넘는 입력까지 NativeAOT 호환이라고 선언하지 않는다.
- [ ] 제품 코드와 번역기 출력의 차이를 허용하고, 현재 프레임워크 전체와의 동일성 비교·golden 파일 동기화·재생성 덮어쓰기를 하지 않는다.
- [ ] 번역기 README의 `Doroti/migration/...` 예시 경로가 현재 checkout에 없는 문제는 선정한 도구 개선의 실행에 필요할 때만 정리한다.

**이 보조 작업의 완료 조건:** 선정한 개선 항목과 그 fixture가 검증되고 도구의 지원 범위가 명확하다. 프레임워크 전체 재생성, 제품 코드와의 일치, 모든 제품 변경의 역반영은 완료 조건이 아니다. 선택하지 않은 개선은 별도 backlog로 남겨도 G3 완료에 영향을 주지 않는다.

### N4. 부팅·렌더·지역화 경로 정적화

- [ ] `view.cs`의 RenderView 초기 프레임·자식 연결을 typed contract로 전환한다.
- [ ] `localizations.cs`에 비제네릭 delegate 계약을 연결하고 `LocalizationsDelegate<T>`의 typed 구현·reload·동기/비동기 load를 보존한다.
- [ ] `framework.cs`, `layout_builder.cs`, `sliver_layout_builder.cs`의 State/Element/render/layout 경계를 정비한다.
- [ ] nullable 수신자, attach/detach, child 교체·이동·제거, relayout, 비동기 지역화 완료 후 rebuild를 검증한다.
- [ ] 외부 fixture assembly에 정의한 State/LocalizationsDelegate도 동일 경로로 동작하게 한다.
- [ ] 정비한 계약의 descriptor 보존 의존을 제거하면서 Mono 회귀와 작은 iOS NativeAOT publish를 모두 확인한다. 아직 DLR이 남은 다른 계약의 Mono 보존을 한꺼번에 삭제하지 않는다.

**완료 조건:** 작은 실제 Doroti 위젯 트리를 NativeAOT로 부팅·갱신·해제할 수 있고 최초 `NoSuchMember` 경로가 멤버 보존 없이 통과한다. 정적 첫 프레임은 G2 전체 완료가 아니다.

### N5. Widgets 전체 공개 동작 계약

- [ ] Navigator/Route/Overlay, Actions/Shortcuts/Focus, Scroll/Sliver, ImageProvider, EditableText/Selection 등 기능군별로 남은 호출을 조사한다.
- [ ] 기존 `RouteBase`, `IIntentAction`, render-child/container 계약을 확장·재사용한다. 런타임 타입별 중복 dispatch 체계를 만들지 않는다.
- [ ] heterogeneous generic 결과·callback·컬렉션의 타입/수명/오류 정책을 유지한다. reflection으로 닫힌 제네릭을 런타임에 만드는 경로는 생성된 factory로 바꾼다.
- [ ] Navigation 결과, focus 이동, gesture 취소, scroll momentum, 이미지 로드 실패/재시도, 편집·선택·clipboard의 cold path를 검증한다.
- [ ] `dynamic-dispatch`의 reflection 기반 IL/CallSite 검사 부분은 호스트 측 정적 검사와 분리하여, 실제 동작 fixture를 NativeAOT로 실행할 수 있게 한다.

**완료 조건:** 지원 Widgets와 사용자 확장 경로에 DLR 실행 의존이 없고 Debug/Release의 의미가 일치한다. Release에서 assert가 제거되어 호출이 사라지는 것만으로 수정 완료를 주장하지 않는다.

### N6. Material·Cupertino·Painting 마무리

- [ ] Theme/ColorScheme/Localizations, button/selection control, menu/dialog/bottom sheet, date/time picker, navigation, text toolbar/magnifier를 기능군별로 정비한다.
- [ ] Painting/Rendering에 남은 dynamic 연산과 callback도 끝낸다. 외부 자료형·연산자의 동등성·오류 의미를 유지한다.
- [ ] `fcr7-material-widget`의 기존 fixture를 재사용하고 외부 앱 타입으로 generic API 확장을 검증한다. Flutter provenance는 참고하되 의도적인 Doroti 차이는 현재 제품 계약으로 검증한다.
- [ ] 첫 화면뿐 아니라 탭 전환·다크모드·색상/이미지 테마 변경·picker·텍스트 메뉴를 열어 지연 초기화 경로까지 실행한다.

**완료 조건:** Testbed가 제공하는 Material/Cupertino 기능을 제거하지 않고 NativeAOT로 실행할 수 있으며, root descriptor로만 숨겨진 동적 호출이 남지 않는다.

### N7. 리소스·직렬화·네이티브 경계 통합 정비

- [ ] `FrameworkShaderAssets`의 assembly 탐색/loading을 resource-owner 등록으로 바꾸고 shader hash/ABI/오류 callback을 유지한다.
- [ ] generated JSON context와 명시적 Dart codec을 새 plugin·진단 기능에도 적용한다. 남은 reflection serializer, 문자열 type lookup, assembly-location 가정을 전체 iOS closure에서 검사한다.
- [ ] ObjC/Swift/C ABI와 Skia/native callback의 signature, 문자열/구조체 marshalling, delegate/handle 수명, threading, dispose를 검사한다.
- [ ] analyzer나 실제 linker가 요구하는 경계에만 source-generated interop 또는 명시적 thunk를 적용한다. 모든 `DllImport`/Marshal API가 NativeAOT 불가라는 전제로 일괄 변경하지 않는다.
- [ ] 배포된 번들에서 native framework·폰트·이미지·shader가 존재하고 실제 로드되는지 확인한다.

**완료 조건:** 네트워크·리소스·native callback을 포함한 지연 경로가 NativeAOT에서 동작하고, fallback으로 Mono 또는 런타임 assembly loading을 요구하지 않는다.

### N8. NativeAOT publish 경로와 패키지·템플릿

- [ ] Runner SDK와 iOS csproj에 명시적 NativeAOT 배포 프로필을 연결한다. Mono 전용 interpreter/AOT 옵션과 광범위 `TrimmerRoots.xml`을 NativeAOT 경로에서 제외한다.
- [ ] 현재 descriptor 조건이 `TrimMode=full`만 검사하는 점을 수정하여 NativeAOT와 Mono 보존형 실험이 명시적으로 분리되게 한다. NativeAOT에 빈 interpreter 값을 넘기는 것만으로 끝내지 않고 `Sdk.targets`의 기본값 재주입도 막는다. 최종 평가 속성·item과 compiler 입력으로 검증한다.
- [ ] 정비된 제품 라이브러리에 `IsAotCompatible`/trim 분석 정책을 적용한다. 앱에서 안 쓰는 메서드는 publish 분석만으로 놓칠 수 있으므로 라이브러리 API 소비 fixture도 유지한다.
- [ ] `Doroti/eng/doroti.ps1`, `launch-identity.ps1`, generated bootstrap과 restore/publish graph에 compilation mode를 전달하고 재사용 키에 반영한다.
- [ ] `launch-identity.ps1`의 현재 runner/configuration/RID 선택에 mode 및 trim/보존 정책·관련 native provenance를 포함한다. Mono → NativeAOT → Mono 및 역순에서 서로의 obj/bin·서명 bundle을 재사용하지 않는지 검사한다.
- [ ] 앱 프로젝트와 플랫폼 runner의 property 전파, RID 제거 규칙, native binding 빌드가 올바른 TFM/RID를 선택하는지 검증한다.
- [ ] NuGet/buildTransitive/템플릿에 static 등록·리소스 계약을 포함한다. repository ProjectReference가 없는 새 소비 앱에서도 확인한다.
- [ ] 전환 중 opt-in 검증 정책과 최소 SDK/패키지 요구를 문서화한다. N12에서 iOS 정식 배포 기본 경로로 승격하며 검증된 프로필을 숨겨진 개발자 명령에만 남기지 않는다.

**완료 조건:** 정식 명령으로 깨끗한 restore → NativeAOT publish → 서명된 설치 대상 생성이 재현되고, Mono 산출물을 잘못 재사용하는 계약 테스트도 통과한다.

### N9. iPhone 기능·수명 검증 — G2

- [ ] 실제 서명된 NativeAOT 산출물을 설치한다. compiler/link 로그, runtime feature 정보, 실행 파일·링크 의존성으로 NativeAOT 실행을 확인한다. `RuntimeFeature.IsDynamicCodeSupported=false` 하나만으로 Mono AOT와 구분하지 않는다.
- [ ] §4.4 검사 결과와 설치한 bundle identity를 연결하고 `monoAbsent=pass`를 확인한다. 정적 링크 자료나 배포 payload 확인이 빠지면 기능이 동작해도 Mono 제거 판정은 `notVerified`다.
- [ ] Material·진단·MediaQuery 각각의 cold start, 앱 아이콘 재실행, background/foreground, 반복 view 생성/해제, GC 이후 callback을 확인한다.
- [ ] 버튼·switch·slider·menu·dialog·route 결과, 스크롤/fling/취소, 키보드·한글/IME·선택/clipboard, 테마·지역화·날짜/시간, 이미지·shader·native plugin을 실제 입력으로 검증한다.
- [ ] 세로/가로, safe area, keyboard inset, text scale, 접근성 focus/semantics와 기존 렌더 fallback의 지원 계약을 확인한다.
- [ ] frame/evidence·managed/native 오류·화면 캡처를 함께 기록한다. 오류 위젯이 그려졌거나 pointer event가 0인 시작 스냅샷을 상호작용 통과로 기록하지 않는다.

**완료 조건:** §6의 필수 검증표가 실제 기기 증거로 채워지고 재현 가능한 runtime 오류가 없다. 사용할 수 없는 기기/OS 검증은 미검증으로 표시하고 해당 지원 범위를 승격하지 않는다.

### N10. 크기·시작·프레임 성능 및 제네릭 코드 크기

- [ ] 같은 커밋/기능/리소스/아키텍처/signing 조건의 Mono 후보와 비교한다. `.app` 파일 합계, 실행 파일/리소스 구성, 압축 IPA, 기기 설치 크기를 각각 구분한다.
- [ ] 첫 의미 있는 프레임까지의 시간, 반복 시작 분포, 입력→새 장면 지연, frame p50/p95, 긴 프레임 비율, 메모리·할당을 측정한다. 실제 디스플레이 지연과 내부 commit 측정치를 혼동하지 않는다.
- [ ] 대표 조작·기기 온도·refresh rate·측정 횟수·warm/cold 조건을 고정한다. N0에서 정한 수치 기준을 보고 통과를 판정한다.
- [ ] NativeAOT 크기가 예상보다 크면 generic virtual method/value-type 인스턴스, 과도한 adapter/registration, 보존 metadata, native asset을 size/map 자료로 조사한다.
- [ ] `IlcOptimizationPreference=Size` 등은 검증된 NativeAOT 경로에서 별도 비교한다. 인터프리터·기능 삭제·stack trace 전체 제거로 합격을 대체하지 않는다.

**완료 조건:** §4.5의 bytes 비교·주요 구성·측정 조건을 남기고 N0에서 정한 성능 합격 기준을 만족한다. 크기 절감이 없다는 이유만으로 Mono 제거를 실패 처리하지 않으며, 크기 개선 후보는 별도 backlog로 남긴다. NativeAOT의 일반적인 벤치마크 수치를 Doroti 실측처럼 인용하지 않는다.

### N11. 지속 검증·지원 범위별 회귀 방지

- [ ] PR에서 새 DLR call-site/무분별한 assembly loading/분석 경고가 추가되는 것을 감지한다. 텍스트 dynamic 토큰 0보다 실제 호출 IL/의미 분석 결과를 사용한다.
- [ ] Runtime/계약 fixture의 NativeAOT publish와 실행, 대표 외부 API 소비 앱, 제품 빌드에 필수인 bootstrap·등록 코드 생성을 자동 검증한다. N3에서 실제 개선한 번역 fixture의 검증은 도구용 검사로 별도 관리한다.
- [ ] macOS runner에서 iOS native publish를 수행하고 signing/device 검증은 가능한 보안·장비 환경의 별도 단계로 연결한다. 장비 부재를 PASS로 처리하지 않는다.
- [ ] Android, Web Worker, Windows 두 호스트, AppKit, Catalyst, Linux의 공유 계약·host 빌드·가능한 실제 입력 회귀를 수행한다.
- [ ] 빌드 모드 변경, 누락된 native dependency, stale cache, 패키지 소비/템플릿 누락을 회귀 fixture로 남긴다.
- [ ] §4.4 검사기로 최종 배포 경로에 Mono runtime/interpreter 또는 Mono AOT 산출물이 재유입되면 실패시킨다. dependency graph·링크 provenance 변경도 감지한다.

**완료 조건:** 누군가 새 dynamic fallback을 도입하거나 패키지 등록을 누락하면 자동 검증이 실패하며, 플랫폼별 실제 검증 범위가 보고서와 일치한다.

### N12. 배포 경로 승격과 최종 인계 — G3

- [ ] 정식 NativeAOT 사용 명령, 지원 버전/타깃, 사용자 위젯·plugin·serializer 확장 규칙, 진단·symbolication 방법을 정리한다.
- [ ] 과거 full-trim workaround와 NativeAOT 요구를 구분한다. PDB 관련 ILLink 내부 오류가 현재 도구에서도 재현되는지 확인하고, 과거 `DebugType=None` 우회를 영구 정책으로 복사하지 않는다.
- [ ] 새 템플릿 앱과 NuGet 소비 앱의 publish/설치 결과를 남기고, 사용자 정의 generic widget/localization/action이 포함된 앱으로 검증한다.
- [ ] 최종 커밋·산출물 해시·패키지 버전·테스트 결과·측정치·남은 플랫폼별 제한을 기록한다. 문제가 생기면 명시적인 Mono 프로필로 되돌릴 수 있게 한다.
- [ ] G2·소비 앱 검증·제품 성능 게이트를 통과하면 iOS Runner/CLI/템플릿의 정식 배포 기본값을 NativeAOT로 전환하고 그 기본 경로를 다시 검증하여 G3를 닫는다. Mono는 명시적인 전환 복구 프로필로만 남기고 자동 런타임 fallback은 만들지 않는다. 다른 타깃은 각각의 검증 전까지 승격하지 않는다.

**완료 조건:** 새 checkout에서 iOS 정식 배포 기본 경로가 NativeAOT로 재현되고, 배포 SDK/템플릿과 실제 Testbed가 같은 계약을 사용한다. 각 최종 앱에 `monoAbsent=pass`, 실제 NativeAOT publish·기능·성능 결과와 크기 변화가 기록되어 있다. opt-in probe만 완료한 상태는 G3가 아니다. iOS 밖에 남은 Mono 사용 범위도 명시한다.

## 6. 필수 검증표

| 영역                | 주요 시나리오                                                                                             | 기존 검증 자산 / 추가할 증거                                                                                                                                     |
| ------------------- | --------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 제품 호출 의미      | overload, variance, operator, callback, mixin/override, 오류 동작                                         | 현재 제품의 `dynamic-dispatch`/`virtual-dispatch` 및 NativeAOT 계약 fixture                                                                                      |
| Runtime 비동기      | 동기 완료/지연, error recovery, error handler의 인수·반환 형태, timeout/cancellation, microtask 순서      | `runtime-async-contract`, 작은 NativeAOT 소비 fixture                                                                                                            |
| 등록·JSON·native    | manifest, 지역화·shader owner, nested map/list, Unicode/숫자, main-thread completion·GC                   | `json-codec`, `app-bootstrap/descriptor-contract`, 실기기 왕복 로그                                                                                              |
| Widget·generic 확장 | attach/update/detach, LayoutBuilder/SliverLayoutBuilder, Route 결과, Action dispatch, 외부 State/delegate | `dynamic-dispatch`, `virtual-dispatch`, NuGet 소비 앱                                                                                                            |
| Material/Cupertino  | 컴포넌트, picker/menu/dialog, toolbar/magnifier, theme/locale/typography                                  | `fcr7-material-widget`와 샘플 실제 조작                                                                                                                          |
| 입력·metrics        | pointer/fling/cancel, 한글/IME/선택/clipboard, 회전·keyboard·SafeArea·text scale                          | `--scaffold-metrics`, `--ios-text-menu`, `--scroll-tap`, [MediaQuery/SafeArea 완료 기록](history/26-09-10/media-query-safe-area-summary.md)의 raw→framework 검증 |
| 렌더·리소스         | Graphite/Metal, font/image, shader filter, background 복귀·resource 재생성                                | `runtime-shader-contract`, `image-pipeline`, device evidence·캡처                                                                                                |
| 접근성·수명         | semantics focus/actions, 폰트 배율, view 교체, dispose 후 callback, native handle 수명                    | semantics fixture + iPhone 접근성/반복 수명 검증                                                                                                                 |
| 배포·성능           | 새 checkout, mode 변경, 서명 번들, 아이콘 재실행, size/startup/frame/memory                               | publish binlog·artifact hash·launch identity fixture·기기 측정                                                                                                   |
| Mono 제거           | static/dynamic native link·최종 bundle·설치 identity 일치, Mono 재유입·fallback 없음 | §4.4 probe/Testbed/새 소비 앱 공통 검사와 원자료 |

라이브러리의 NativeAOT 호환성 선언에는 실제 앱에서 도달하지 않는 public API의 검증도 필요하다. 필요한 경우 API를 root로 삼는 분석용 앱과 작은 기능별 실행 앱을 나누되, 분석용으로 모든 API를 보존한 앱의 크기를 제품 크기로 비교하지 않는다.

N3을 수행하면 선정한 번역 fixture의 의미·컴파일·실행 검증을 별도로 추가한다. 위 제품 필수 검증표에는 프레임워크 전체 번역·동일 재현 게이트를 두지 않는다.

## 7. 실행 순서·산출물·완료 기록

의존 순서는 다음과 같다.

```text
N0 → N1(외부 기반 조기 확인)
N0 → N2(런타임 계약)
N1 통과 + N2 → N4 → N5 → N6(현재 프레임워크를 계약 단위로 직접 전환)
N1 + N2 + N4~N6 → N7 → N8 → N9 → N10 → N12
N11은 N0부터 회귀 검사를 추가하고 N12 전에 전체 게이트를 완성
N3은 별도 보조 개선: 제품 전환에서 얻은 패턴을 선택적으로 반영
```

N0/N1의 최소 publish 프로필로 N2/N4~N7의 계약 변경마다 가능한 NativeAOT publish를 수행한다. N8은 최초 실행을 위한 단계가 아니라 제품 배포 통합 단계다. 각 변경은 **정적 계약 fixture → 기존 플랫폼 회귀 → NativeAOT fixture/publish → 보존 의존 축소 → 증거 기록** 순으로 진행한다.

모든 테스트/외부 검증 프로세스는 `.github/copilot-instructions.md`에 따라 **20분(1,200초) timeout**을 적용한다. 시작·성능 측정 반복은 기본 10회 이내, 필요 시 이유를 기록하고 최대 20회로 제한하며 warm-up과 retry도 센다. timeout이나 장비 부재는 통과로 바꾸지 않는다.

실제 일정 추정은 N0/N1에서 blocker를 수집한 뒤 계약군별로 한다. 현재 작업량을 `dynamic` 텍스트 건수로 나누어 확정 일정을 만들지 않는다. 제품 변경은 원인·제품 코드·테스트·증거가 함께 검토되는 크기로 나누고, 번역기 개선은 선정한 lowering·도구 fixture 단위로 따로 진행한다.

향후 산출물의 권장 위치(아직 생성하지 않음):

- `Doroti/docs/validation/nativeaot-<date>.md`: 버전·최초 실패·호환성 판단·실기기·성능 최종 보고서.
- `Doroti/validation/native-aot/`: 호스트 probe, API 소비·동작 fixture, 분석·publish 검증 진입점.
- 위 디렉터리의 `size-report.py`, Mono 제거 검사 진입점, `blockers.json`, `gate-results.json`: 구현 시 확정할 도구·형식 후보. 보고서는 커밋/RID/mode/산출물 해시와 원자료 경로를 포함한다.
- `tools/Doroti.DartToCSharp/validation/native-aot/`: N3에서 선택한 개선의 fixture가 필요할 때만 추가. 프레임워크 전체 재생성 검증용이 아님.
- `Doroti/artifacts/native-aot/...`: 모드/RID/커밋별 로그·binlog·산출물·측정 원자료. 저장소의 기존 artifact/local-storage 정책에 맞춰 실제 위치를 N0에서 확정.

진행 시 이 문서에 각 N 단계의 상태, 변경 커밋, 실행한 명령, 결과·증거 경로, 미검증 사항과 다음 장애물을 갱신한다. N1의 작은 앱 성공, 경고 억제 publish, 기능 일부만 되는 Testbed를 전체 완료로 표시하지 않는다.

현재 상태: **계획 재검토 완료 / N0~N12 구현 미착수 / G1~G3 미검증 / Mono 제거 미검증**. 다음 실행 지점은 N0의 환경·baseline·최초 publish 및 제거 증거 수집이다. `ref1.md`는 참고 원문으로 보존한다.

## 8. 공식 근거

2026-09-10 열람. 구현 착수 시 실제 고정된 SDK/패키지 버전과 다시 대조한다.

- [MAUI iOS/Mac Catalyst NativeAOT](https://learn.microsoft.com/en-us/dotnet/maui/deployment/nativeaot?view=net-maui-10.0): 실제 publish 검증, runtime/의존성 제한, 배포·진단 기준.
- [.NET NativeAOT 개요와 제한](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/): runtime code generation/dynamic loading, generic 코드 크기와 플랫폼 제약.
- [라이브러리의 AOT 호환성 정비](https://devblogs.microsoft.com/dotnet/creating-aot-compatible-libraries/): 분석 속성, API 소비 테스트 앱, annotation과 실제 publish 검증.
- [MAUI trimming](https://learn.microsoft.com/en-us/dotnet/maui/deployment/trimming?view=net-maui-10.0): NativeAOT의 full trimming 및 기존 보존 설정과의 구분.
- [Mono interpreter](https://learn.microsoft.com/en-us/dotnet/maui/macios/interpreter?view=net-maui-10.0): 현재 `-all` 의미와 NativeAOT 경로의 차이.
- [MAUI runtimes and compilation](https://learn.microsoft.com/en-us/dotnet/maui/deployment/runtimes-compilation?view=net-maui-10.0): Mono AOT와 NativeAOT 구분, Android 패키지 구성. 일반 예제의 크기를 Doroti 실측으로 사용하지 않음.
- [Android App Bundles](https://developer.android.com/guide/app-bundle), [bundletool](https://developer.android.com/tools/bundletool): AAB와 기기별 APK/전송 추정 구분. Android 용량 진단이 필요할 때의 참고이며 Mono 제거 선행 단계는 아님.
