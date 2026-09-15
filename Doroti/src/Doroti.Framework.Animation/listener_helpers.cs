// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/listener_helpers.dart
namespace Doroti.Framework.Animation;

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

