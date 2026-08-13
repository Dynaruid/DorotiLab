/// Reserved isolate-worker contract. The default batch session intentionally uses one
/// analyzer-owned context until benchmark evidence justifies more isolated workers.
final class AnalyzerWorker {
  const AnalyzerWorker(this.ordinal);
  final int ordinal;
}
