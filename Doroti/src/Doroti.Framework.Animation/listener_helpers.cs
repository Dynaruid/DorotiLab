// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/listener_helpers.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Animation;

public interface AnimationLazyListenerMixin
{
    long _listenerCounter { get; set; }

    public void didRegisterListener();
    public void didUnregisterListener();
    public void didStartListening();
    public void didStopListening();
    public bool isListening { get; }
}

public interface AnimationEagerListenerMixin
{
    public void didRegisterListener();
    public void didUnregisterListener();
    public void dispose();
}

public interface AnimationLocalListenersMixin
{
    HashedObserverList<Action> _listeners { get; }

    public void didRegisterListener();
    public void didUnregisterListener();
    public void addListener(Action listener);
    public void removeListener(Action listener);
    public void clearListeners();
    public void notifyListeners();
}

public interface AnimationLocalStatusListenersMixin
{
    ObserverList<AnimationStatusListener> _statusListeners { get; }

    public void didRegisterListener();
    public void didUnregisterListener();
    public void addStatusListener(AnimationStatusListener listener);
    public void removeStatusListener(AnimationStatusListener listener);
    public void clearStatusListeners();
    public void notifyStatusListeners(AnimationStatus status);
}

