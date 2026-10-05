using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Ui;

namespace Doroti.Framework;

/// <summary>Membership of one application's logical tree, updated only on its framework owner.</summary>
public sealed class DorotiApplicationViews : ChangeNotifier
{
    private readonly Dictionary<ulong, DorotiView> _views = [];
    public IReadOnlyList<DorotiView> Views => Array.AsReadOnly(_views.Values.ToArray());
    internal void Attach(DorotiView view)
    {
        if (!_views.TryAdd(view.viewId, view)) throw new InvalidOperationException("A view is already attached to this tree.");
        notifyListeners();
    }
    internal void Detach(DorotiView view)
    {
        if (_views.TryGetValue(view.viewId, out var attached) && ReferenceEquals(attached, view))
        {
            _views.Remove(view.viewId);
            notifyListeners();
        }
    }
}

/// <summary>Put shared State/InheritedWidgets above this collection and view-local UI below it.</summary>
public sealed class DorotiApplicationViewCollection : StatefulWidget
{
    public DorotiApplicationViewCollection(DorotiApplicationViews applicationViews,
        Func<BuildContext, DorotiView, Widget> builder, Key? key = null) : base(key: key)
    {
        ApplicationViews = applicationViews ?? throw new ArgumentNullException(nameof(applicationViews));
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }
    public DorotiApplicationViews ApplicationViews { get; }
    public Func<BuildContext, DorotiView, Widget> Builder { get; }
    public override IState createState() => new CollectionState();

    private sealed class CollectionState : State<DorotiApplicationViewCollection>
    {
        private NativeWindowPresentation.Registry? _presentations;
        public override void initState()
        {
            base.initState();
            widget.ApplicationViews.addListener(MembershipChanged);
            _presentations = NativeWindowPresentation.For(WidgetsBinding.instance.platformDispatcher);
            _presentations.addListener(MembershipChanged);
        }
        private void MembershipChanged() { if (mounted) setState(() => { }); }
        public override void didUpdateWidget(DorotiApplicationViewCollection oldWidget)
        {
            base.didUpdateWidget(oldWidget);
            var previous = oldWidget;
            if (!ReferenceEquals(previous.ApplicationViews, widget.ApplicationViews))
            {
                previous.ApplicationViews.removeListener(MembershipChanged);
                widget.ApplicationViews.addListener(MembershipChanged);
            }
        }
        public override Widget build(BuildContext context)
        {
            var children = new HashSet<Widget>(ReferenceEqualityComparer.Instance);
            var branches = new List<Widget>();
            foreach (var view in widget.ApplicationViews.Views)
            {
                var child = _presentations?.Child(view) ?? widget.Builder(context, view)
                    ?? throw new InvalidOperationException("A view builder returned null.");
                if (!children.Add(child)) throw new InvalidOperationException("One Widget instance cannot be attached to two views.");
                branches.Add(new View(key: new ValueKey<ulong>(view.viewId), view: view, child: child));
            }
            return new ViewCollection(views: branches);
        }
        public override void dispose()
        {
            widget.ApplicationViews.removeListener(MembershipChanged);
            _presentations?.removeListener(MembershipChanged);
            _presentations = null;
            base.dispose();
        }
    }
}
