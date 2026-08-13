using Doroti.Generated.Framework.Animation;
using Doroti.Generated.Framework.Gestures;
using Doroti.Generated.Framework.Physics;

if (Math.Abs(Curves.linear.transform(0.25) - 0.25) > 1e-12)
{
    throw new InvalidOperationException("Animation package behavior drifted.");
}

var spring = new SpringSimulation(new SpringDescription(1, 100, 20), 0, 1, 0);
if (!spring.isDone(10) || Math.Abs(spring.x(10) - 1) > 1e-6)
{
    throw new InvalidOperationException("Physics package behavior drifted.");
}

var arena = new GestureArenaManager();
var first = new Member();
var second = new Member();
arena.add(1, first);
arena.add(1, second);
arena.close(1);
arena.sweep(1);
if (first.Accepted != 1 || second.Rejected != 1)
{
    throw new InvalidOperationException("Gestures package behavior drifted.");
}

Console.WriteLine("G4-4-PHYSICS-ANIMATION-GESTURES-PACKAGE-CONSUMER-PASS");

sealed class Member : GestureArenaMember
{
    public int Accepted { get; private set; }
    public int Rejected { get; private set; }
    public void acceptGesture(long pointer) => Accepted++;
    public void rejectGesture(long pointer) => Rejected++;
}
