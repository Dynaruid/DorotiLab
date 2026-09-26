using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using Material = Doroti.Framework.Material;

internal sealed class VariableBlurFixture : StatefulWidget
{
    public override IState createState() => new FixtureState();

    private sealed class FixtureState : State<VariableBlurFixture>
    {
        private bool _enabled = true;

        public override Widget build(BuildContext context) =>
            new Material.Scaffold(
                appBar: new Material.AppBar(title: new Text("Progressive backdrop blur")),
                body: new Center(
                    child: new Column(
                        mainAxisSize: MainAxisSize.min,
                        children:
                        [
                            new Text("Original"),
                            Pattern(),
                            new SizedBox(height: 16),
                            new Text(_enabled ? "Clear top / sigma 12 bottom" : "Effect disabled"),
                            new SizedBox(
                                width: 280,
                                height: 120,
                                child: new ClipRect(
                                    child: new Stack(
                                        children:
                                        [
                                            Pattern(),
                                            new Positioned(
                                                left: 0,
                                                top: 0,
                                                right: 0,
                                                bottom: 0,
                                                child: new BackdropFilter(
                                                    filterConfig: ImageFilterConfig.CreateVariableBlur(
                                                        endSigma: 12
                                                    ),
                                                    enabled: _enabled,
                                                    child: new SizedBox()
                                                )
                                            ),
                                            new Positioned(
                                                left: 12,
                                                bottom: 10,
                                                child: new Text("Foreground stays sharp")
                                            ),
                                        ]
                                    )
                                )
                            ),
                            new SizedBox(height: 16),
                            new Material.ElevatedButton(
                                onPressed: () => setState(() => _enabled = !_enabled),
                                child: new Text("Toggle blur")
                            ),
                        ]
                    )
                )
            );

        private static Widget Pattern() =>
            new SizedBox(
                width: 280,
                height: 120,
                child: new Row(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: Enumerable
                        .Range(0, 20)
                        .Select(i =>
                            (Widget)
                                new Expanded(
                                    child: new ColoredBox(
                                        color: new Color(i % 2 == 0 ? 0xff000000 : 0xffffffff)
                                    )
                                )
                        )
                        .ToList()
                )
            );
    }
}
