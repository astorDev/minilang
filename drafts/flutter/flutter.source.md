```dart
void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    var theme = ThemeData.dark();

    return MaterialApp(
        title: 'Flutter Demo',
        debugShowCheckedModeBanner: false,
        theme: ThemeCustomizations.apply(theme),
        onGenerateRoute: (settings) {
          var uri = Uri.parse(settings.name!);
          if (uri.pathSegments.length == 2 && uri.pathSegments[0] == 'code'){
            var code = uri.pathSegments[1];
            return MaterialPageRoute(builder: (context) => CodeInputBreaker(code));
          }

          return MaterialPageRoute(builder: (context) => Home());
        },
        home: Home()
    );
  }

  static Future init() async {
    var env = const String.fromEnvironment('Environment');
    App.init(env);
    await Auth.init(App.settings.auth);
    Api.init(App.settings.api);
    if (Auth.tokens != null) {
      await Filtering.refresh();
    }
  }
}

class Home extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
        future: MyApp.init(),
        builder: (context, snapshot) {
          if (!(snapshot.connectionState == ConnectionState.done)) {
            return SpinnerPage();
          } else {
            if (Auth.tokens == null) {
              return UnauthenticatedPage();
            } else {
              return HomeGate();
            }
          }
        });
  }
}
```

```dart
class Settings {
  AuthSettings auth;
  ApiSettings api;
  String telegramDeeplink;

  Settings(this.auth, this.api, this.telegramDeeplink);

  static Settings fromEnvironmentName(String env) {
    switch (env) {
      case 'Mocks':
        return Settings.mocks;
      case 'Development':
        return Settings.development;
      case 'Production':
        return Settings.production;
      default:
        throw Exception('unknown environment');
    }
  }

  static Settings get mocks {
    return Settings(
        AuthSettings(
            "https://30e0953f-ce78-4d74-b559-fc7b2f6bb4dd.mock.pstmn.io"),
        ApiSettings(
            "https://2ff4121f-2b65-448c-8961-a0fe2d359b5b.mock.pstmn.io"),
        "https://t.me/finrirtestbot?start"    
        );
  }

  static Settings get development {
    return Settings(AuthSettings("http://localhost:5220"),
        ApiSettings("http://localhost:5230"),
        "https://t.me/finrirtestbot?start"
      );
  }

  static Settings get production {
    return Settings(AuthSettings("https://finrir.com/auth"),
        ApiSettings("https://finrir.com/mobile"),
        "https://t.me/finrirbot?start"
      );
  }
}
```

```dart
class SpinnerPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(child: Spinner()),
    );
  }
}

class Spinner extends StatefulWidget {
  final double radius;
  final List<Color>? gradientColors;
  final double strokeWidth;

  Spinner({
    Key? key,
    this.radius = 50.0,
    this.gradientColors,
    this.strokeWidth = 10.0,
  }) : super(key: key);

  @override
  _SpinnerState createState() => _SpinnerState();
}

class _SpinnerState extends State<Spinner> with SingleTickerProviderStateMixin {
  late AnimationController animationController;

  @override
  void initState() {
    animationController =
        AnimationController(vsync: this, duration: const Duration(seconds: 1));
    animationController.addListener(() => setState(() {}));
    animationController.repeat();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return RotationTransition(
      turns: Tween(begin: 0.0, end: 1.0).animate(animationController),
      child: CustomPaint(
        size: Size.fromRadius(widget.radius),
        painter: _GradientCircularProgressPainter(
          radius: widget.radius,
          gradientColors: widget.gradientColors ?? getColors(context),
          strokeWidth: widget.strokeWidth,
        ),
      ),
    );
  }

  List<Color> getColors(BuildContext context) {
    var colors = <Color>[Theme.of(context).scaffoldBackgroundColor];
    colors.addAll(ThemeGradient.colorsOf(context));
    return colors;
  }

  @override
  void dispose() {
    animationController.dispose();
    super.dispose();
  }
}

class _GradientCircularProgressPainter extends CustomPainter {
  _GradientCircularProgressPainter({
    required this.radius,
    required this.gradientColors,
    required this.strokeWidth,
  });
  final double radius;
  final List<Color> gradientColors;
  final double strokeWidth;

  @override
  void paint(Canvas canvas, Size size) {
    size = Size.fromRadius(radius);
    double offset = strokeWidth / 2;
    Rect rect = Offset(offset, offset) &
        Size(size.width - strokeWidth, size.height - strokeWidth);
    var paint = Paint()
      ..style = PaintingStyle.stroke
      ..strokeWidth = strokeWidth;
    paint.shader =
        SweepGradient(colors: gradientColors, startAngle: 0.0, endAngle: 2 * pi)
            .createShader(rect);
    canvas.drawArc(rect, 0.0, 2 * pi, false, paint);
  }

  @override
  bool shouldRepaint(CustomPainter oldDelegate) {
    return true;
  }
}
```