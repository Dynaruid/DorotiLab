{{flutter_js}}
{{flutter_build_config}}
const renderer = new URL(location.href).searchParams.get('renderer') || 'canvaskit';
if (renderer !== 'canvaskit' && renderer !== 'skwasm') throw new Error('Unsupported differential renderer');
// Loader configuration forces the renderer, so an incompatible build fails
// instead of silently combining CanvasKit and skwasm measurements.
_flutter.loader.load({ config: { renderer } });
