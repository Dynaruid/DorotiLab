using Doroti.Framework.Cupertino;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace DorotiSampleApp2;

internal sealed class VariableBlurPage : StatefulWidget
{
    public override IState createState() => new VariableBlurPageState();
}

internal sealed class VariableBlurPageState : State<VariableBlurPage>
{
    private const double OverlayHeight = 180;
    private bool _enabled = true;
    private bool _fast = true;
    private double _sigma = 20;
    private readonly ScrollController _scrollController = new();
    private static readonly Color[] RowColors =
    {
        CupertinoColors.systemBlue, CupertinoColors.systemPurple,
        CupertinoColors.systemOrange, CupertinoColors.systemGreen,
        CupertinoColors.systemPink, CupertinoColors.systemTeal,
    };

    public override void dispose()
    {
        _scrollController.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context) => new CupertinoPageScaffold(
        navigationBar: new CupertinoNavigationBar(middle: new Text("Variable Blur")),
        backgroundColor: CupertinoColors.systemGroupedBackground.resolveFrom(context),
        child: new SafeArea(child: new Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: new List<Widget>
            {
                new Padding(padding: EdgeInsets.CreateAll(16), child: new Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: new List<Widget>
                    {
                        new Row(children: new List<Widget>
                        {
                            new Expanded(child: new Text($"Blur strength: {_sigma:F0}",
                                style: new TextStyle(fontWeight: FontWeight.w600))),
                            new CupertinoSwitch(value: _enabled,
                                onChanged: value => setState(() => _enabled = value)),
                        }),
                        new CupertinoSlider(value: _sigma, min: 0, max: 32,
                            onChanged: value => setState(() => _sigma = value)),
                        new Row(children: new List<Widget>
                        {
                            new Expanded(child: new Text("Adaptive fast blur")),
                            new CupertinoSwitch(value: _fast,
                                onChanged: value => setState(() => _fast = value)),
                        }),
                        new Text("Scroll the list through the blur. Strong at the top, clear below.",
                            style: new TextStyle(fontSize: 14,
                                color: CupertinoColors.secondaryLabel.resolveFrom(context))),
                    })),
                new Expanded(child: new ClipRect(child: new Stack(
                    fit: StackFit.expand,
                    children: new List<Widget>
                    {
                        ListView.CreateBuilder(
                            controller: _scrollController,
                            padding: EdgeInsets.CreateOnly(left: 16, right: 16, bottom: 24),
                            itemCount: 60,
                            itemBuilder: (rowContext, index) => BuildRow(rowContext, index)),
                        // Paint after the list so the filter samples the scrolling content.
                        // Clip only the overlay; let pointer events reach the list beneath it.
                        new Positioned(top: 0, left: 0, right: 0, height: OverlayHeight,
                            child: new IgnorePointer(child: new ClipRect(child: new BackdropFilter(
                                enabled: _enabled && _sigma > 0,
                                filterConfig: ImageFilterConfig.CreateVariableBlur(
                                    startSigma: _sigma, endSigma: 0,
                                    resolutionScale: _fast ? 0.25 : 1),
                                child: SizedBox.CreateExpand())))),
                    }))),
            }))
    );

    private static Widget BuildRow(BuildContext context, long index) => new Padding(
        padding: EdgeInsets.CreateOnly(bottom: 10),
        child: new Container(
            padding: EdgeInsets.CreateAll(16),
            decoration: new BoxDecoration(
                color: CupertinoColors.secondarySystemGroupedBackground.resolveFrom(context),
                borderRadius: BorderRadius.CreateCircular(16)),
            child: new Row(children: new List<Widget>
            {
                new Container(width: 52, height: 52,
                    alignment: Alignment.center,
                    decoration: new BoxDecoration(
                        color: CupertinoDynamicColor.resolve(RowColors[index % RowColors.Length], context),
                        borderRadius: BorderRadius.CreateCircular(12)),
                    child: new Text($"{index + 1:00}", style: new TextStyle(
                        fontSize: 20, fontWeight: FontWeight.bold, color: CupertinoColors.white))),
                new SizedBox(width: 16),
                new Expanded(child: new Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: new List<Widget>
                    {
                        new Text($"List item {index + 1:00}",
                            style: new TextStyle(fontSize: 17, fontWeight: FontWeight.w600)),
                        new SizedBox(height: 4),
                        new Text("Color, text and edges blur together.",
                            style: new TextStyle(fontSize: 13,
                                color: CupertinoColors.secondaryLabel.resolveFrom(context))),
                    })),
            }))
    );
}
