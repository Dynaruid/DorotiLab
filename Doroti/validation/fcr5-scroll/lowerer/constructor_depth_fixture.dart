mixin ViewportDepthMixin {
  int _depth = 0;

  int get depth => _depth;
}

class NotificationSource {
  NotificationSource({required this.source});

  final String source;
}

class UpdateNotification extends NotificationSource with ViewportDepthMixin {
  UpdateNotification({required super.source, int? depth}) {
    if (depth != null) {
      _depth = depth;
    }
  }
}

