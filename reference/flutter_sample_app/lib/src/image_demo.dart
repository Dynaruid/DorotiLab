import 'package:flutter/material.dart';

const demoImageAsset = 'assets/images/mae-mu-9002s2VnOAY-unsplash.webp';
const demoImageUrl =
    'https://plus.unsplash.com/premium_photo-1734210255965-0a721514a34e';

/// Displays the same local and remote inputs used by the Doroti image fixture.
class ImageDemo extends StatefulWidget {
  const ImageDemo({
    super.key,
    this.assetImage = const AssetImage(demoImageAsset),
    this.networkImage = const NetworkImage(demoImageUrl),
  });

  final ImageProvider assetImage;
  final ImageProvider networkImage;

  @override
  State<ImageDemo> createState() => _ImageDemoState();
}

class _ImageDemoState extends State<ImageDemo> {
  bool _network = false;
  BoxFit _fit = BoxFit.contain;
  ColorScheme? _light;
  ColorScheme? _dark;
  bool _extracting = false;
  String? _error;
  int _generation = 0;
  int _reload = 0;

  ImageProvider get _provider =>
      _network ? widget.networkImage : widget.assetImage;

  void _select(bool network) {
    setState(() {
      _network = network;
      _generation++;
      _light = _dark = null;
      _error = null;
      _extracting = false;
    });
  }

  Future<void> _extract() async {
    final generation = ++_generation;
    final provider = _provider;
    setState(() {
      _extracting = true;
      _error = null;
    });
    try {
      final schemes = await Future.wait([
        ColorScheme.fromImageProvider(provider: provider),
        ColorScheme.fromImageProvider(
          provider: provider,
          brightness: Brightness.dark,
        ),
      ]).timeout(const Duration(seconds: 30));
      if (!mounted || generation != _generation) return;
      setState(() {
        _light = schemes[0];
        _dark = schemes[1];
        _extracting = false;
      });
    } catch (_) {
      if (!mounted || generation != _generation) return;
      setState(() {
        _extracting = false;
        _error = 'Could not extract colors. Check the image and try again.';
      });
    }
  }

  Future<void> _retryImage() async {
    final generation = _generation;
    await _provider.evict();
    if (!mounted || generation != _generation) return;
    setState(() => _reload++);
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Image demo', style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: 12),
            SegmentedButton<bool>(
              segments: const [
                ButtonSegment(value: false, label: Text('Local asset')),
                ButtonSegment(value: true, label: Text('Image URL')),
              ],
              selected: {_network},
              onSelectionChanged: (selection) => _select(selection.single),
            ),
            const SizedBox(height: 12),
            Text(_network ? 'Unsplash · remote image' : 'Mae Mu · Unsplash'),
            const SizedBox(height: 8),
            ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: ColoredBox(
                color: Theme.of(context).colorScheme.surfaceContainerHighest,
                child: SizedBox(
                  width: double.infinity,
                  height: 240,
                  child: Image(
                    key: ValueKey((_network, _reload)),
                    image: _provider,
                    fit: _fit,
                    semanticLabel: _network
                        ? 'Photo loaded from the Unsplash image URL'
                        : 'Local Unsplash photo by Mae Mu',
                    frameBuilder: (context, child, frame, synchronous) =>
                        frame != null || synchronous
                        ? child
                        : const Center(child: Text('Loading image…')),
                    errorBuilder: (context, error, stack) => Center(
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Text('Image could not be loaded.'),
                          TextButton(
                            onPressed: _retryImage,
                            child: const Text('Retry image'),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ),
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              crossAxisAlignment: WrapCrossAlignment.center,
              children: [
                ChoiceChip(
                  label: const Text('Contain'),
                  selected: _fit == BoxFit.contain,
                  onSelected: (_) => setState(() => _fit = BoxFit.contain),
                ),
                ChoiceChip(
                  label: const Text('Cover'),
                  selected: _fit == BoxFit.cover,
                  onSelected: (_) => setState(() => _fit = BoxFit.cover),
                ),
                FilledButton.tonal(
                  onPressed: _extracting ? null : _extract,
                  child: Text(_extracting ? 'Extracting…' : 'Extract colors'),
                ),
              ],
            ),
            if (_error != null) ...[
              const SizedBox(height: 12),
              Text(_error!, semanticsLabel: _error),
            ],
            if (_light != null && _dark != null) ...[
              const SizedBox(height: 16),
              _Palette(label: 'Light palette', scheme: _light!),
              const SizedBox(height: 12),
              _Palette(label: 'Dark palette', scheme: _dark!),
            ],
          ],
        ),
      ),
    );
  }
}

class _Palette extends StatelessWidget {
  const _Palette({required this.label, required this.scheme});

  final String label;
  final ColorScheme scheme;

  @override
  Widget build(BuildContext context) {
    final roles = [
      ('Primary', scheme.primary, scheme.onPrimary),
      ('Secondary', scheme.secondary, scheme.onSecondary),
      ('Tertiary', scheme.tertiary, scheme.onTertiary),
    ];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: Theme.of(context).textTheme.titleSmall),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            for (final (name, color, foreground) in roles)
              Container(
                width: 108,
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: color,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  '$name\n#${color.toARGB32().toRadixString(16).substring(2).toUpperCase()}',
                  style: TextStyle(color: foreground),
                ),
              ),
          ],
        ),
      ],
    );
  }
}
