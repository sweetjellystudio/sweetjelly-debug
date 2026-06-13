# SWeetJelly Debug

SWeetJelly Debug는 작은 디버그 facade, 라우팅 가능한 디버그 서비스, 메시지 포매팅, Unity 출력 구현을 제공하는 패키지입니다.

일반적인 호출 지점은 단순하게 유지하는 것을 목표로 합니다.

```csharp
using SWeetJelly.Debug.App;

SWJDebug.Log("PlayerHp", hp);
SWJDebug.LogWarning("Inventory", itemCount);
SWJDebug.LogError("Save", saveResult);
SWJDebug.LogException("Save", exception);
```

## 구조

```text
Runtime
  Contract
    IDebugger
    IDebugService
    IDebugFormatter
    DebugEntry
    DebugLabelType
    DebugLevel
  App
    SWJDebug
    DebugService
    DebugFormat
  Framework
    UnityConsoleDebugger
    UnityUIDebugger
  InitialiserIntegration
    DebugInitModule
```

## 개념

`SWJDebug`는 일상적인 호출을 위한 정적 facade입니다.

`IDebugService`는 라우팅, 필터링, 레벨 검사, 포매터 선택, 디버거 등록을 담당합니다.

`IDebugger`는 출력 대상입니다. 기본 구현으로 `UnityConsoleDebugger`와 `UnityUIDebugger`를 제공합니다.

`IDebugFormatter`는 `DebugEntry`를 읽기 쉬운 메시지로 변환합니다. 기본 구현은 `DebugFormat`입니다.

## Initialiser 연동

SWeetJelly Initialiser 모듈 목록에 `DebugInitModule`을 추가합니다.

이 모듈은 `DebugService`를 만들고, `UnityConsoleDebugger`를 등록하고, 이를 `SWJDebug` 서비스로 설정한 뒤 `IDebugService`를 서비스 레지스트리에 등록합니다.

```csharp
using SWeetJelly.Debug.App;

SWJDebug.Log("Debug", "Ready");
```

## 수동 설정

서비스를 직접 구성할 수도 있습니다.

```csharp
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Contract;
using SWeetJelly.Debug.Framework;

var service = new DebugService();
service.MinimumLevel = DebugLevel.Trace;
service.AddDebugger(new UnityConsoleDebugger());

SWJDebug.SetService(service);
```

이전의 단순 설정 방식인 `SWJDebug.Set(IDebugger)`도 계속 사용할 수 있습니다.

```csharp
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Framework;

SWJDebug.Set(new UnityConsoleDebugger());
```

## Trace 로그

Trace 메서드는 호출한 파일, 멤버, 라인 번호를 자동으로 포함합니다.

```csharp
SWJDebug.TraceLog(playerState);
SWJDebug.TraceWarning("Unexpected state");
SWJDebug.TraceError(errorCode);
SWJDebug.TraceException(exception);
```

## 필터링

`MinimumLevel`을 사용하면 낮은 우선순위의 로그를 건너뛸 수 있습니다.

```csharp
SWJDebug.Service.MinimumLevel = DebugLevel.Warning;
```

레벨 순서는 다음과 같습니다.

```text
Trace < Info < Warning < Error < Exception
```

## 커스텀 디버거

새 출력 대상을 추가하려면 `IDebugger`를 구현합니다.

```csharp
using SWeetJelly.Debug.Contract;

public sealed class MyDebugger : IDebugger
{
    public void Publish(DebugEntry entry, string formattedMessage)
    {
        // formattedMessage를 원하는 대상으로 전송합니다.
    }
}
```

그리고 서비스에 등록합니다.

```csharp
SWJDebug.Service.AddDebugger(new MyDebugger());
```

## 커스텀 포매터

메시지 형식을 바꾸려면 `IDebugFormatter`를 구현합니다.

```csharp
using SWeetJelly.Debug.Contract;

public sealed class MyFormatter : IDebugFormatter
{
    public string Format(DebugEntry entry)
    {
        return $"{entry.Level}: {entry.Label} {entry.Value}";
    }
}
```

그리고 서비스에 설정합니다.

```csharp
SWJDebug.Service.SetFormatter(new MyFormatter());
```
