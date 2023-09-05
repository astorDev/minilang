```dart
// ignore_for_file: prefer_const_constructors, use_key_in_widget_constructors, prefer_const_constructors_in_immutables, prefer_const_literals_to_create_immutables, must_be_immutable
import 'package:finrir/core-ui/theme.dart';
import 'package:flutter/material.dart';

class CenteredColumnPage extends StatelessWidget {
  final List<Widget> children;
  EdgeInsetsGeometry? padding;

  CenteredColumnPage({Key? key, 
    required this.children,
    this.padding,
    })
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(body: CenteredColumn(children: children, padding: padding,));
  }
}

class CenteredColumn extends StatelessWidget {
  EdgeInsetsGeometry? padding;
  
  CenteredColumn({
    Key? key,
    required this.children,
    this.padding
  }) : super(key: key);

  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Container(
        padding: padding,
        child: Column(
          children: children,
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.center,
        ),
      ),
    );
  }
}

class Commode extends StatelessWidget {
  bool mindNavbar;
  List<Widget> drawers;
  List<Widget>? topShelfItems;
  bool get hasTopShelfItems => topShelfItems != null && topShelfItems!.isNotEmpty;

  Commode({required this.drawers, this.mindNavbar = false, this.topShelfItems});
 
  @override
  Widget build(BuildContext context) {
    return Align(
      alignment: Alignment.bottomCenter,
      child: Stack(
        children: [
          CommodeBody(hasTopShelfItems: hasTopShelfItems, mindNavbar: mindNavbar, drawers: drawers),
          if (hasTopShelfItems) CommodeTopShelf(children: topShelfItems!)
        ],
      ),
    );
  }
}

class CommodeTopShelf extends StatelessWidget {
  List<Widget> children;
  static double itemSize = 50;
  
  CommodeTopShelf({ required this.children });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: HorizontalMargins.toEdgeInsets(),
      child: Row(
        children: children,
        mainAxisAlignment: MainAxisAlignment.end,
      ),
    );
  }
}

class CommodeBody extends StatelessWidget {
  const CommodeBody({
    Key? key,
    required this.hasTopShelfItems,
    required this.mindNavbar,
    required this.drawers,
  }) : super(key: key);

  final bool hasTopShelfItems;
  final bool mindNavbar;
  final List<Widget> drawers;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: hasTopShelfItems ? EdgeInsets.only(top : CommodeTopShelf.itemSize / 2) : null,
      padding: HorizontalMargins.toEdgeInsets(top: hasTopShelfItems ? 30 : 15),
      height: mindNavbar ? 300 : 400,
      width: 1400000000, //any big number to use whole width
      decoration: BoxDecoration(
        borderRadius: BorderRadius.only(topLeft: Radius.circular(30), topRight: Radius.circular(30)),
        boxShadow: [
          BoxShadow(
            color: Theme.of(context).backgroundColor,
            offset: Offset(0, -1),
            blurRadius: 10
          )
        ],
        color: Theme.of(context).scaffoldBackgroundColor
      ),
      child: SingleChildScrollView(
        child: Column(mainAxisAlignment: MainAxisAlignment.end, children: drawers),
      ),
    );
  }
}

class Underline {
  static BoxDecoration get decoration {
    return BoxDecoration(
      border: Border(bottom: BorderSide(color: Colors.white.withOpacity(0.3))),
    );
  }
}

class TopBar extends StatelessWidget {
  Iterable<Widget>? actions;
  Widget? title;
  Widget? leading;

  TopBar({Key? key, this.actions, this.title, this.leading}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
        margin: HorizontalMargins.toEdgeInsets(top: kToolbarHeight),
        decoration: Underline.decoration,
        child: Row(children: getWidgets().toList()));
  }

  Iterable<Widget> getWidgets() sync* {
    if (leading != null) {
      yield leading!;
    }

    if (title != null) {
      var prepared = prepareTitleWidget(title!);

      yield Expanded(child: prepared);
    }

    if (actions != null) {
      for (var action in actions!) {
        yield action;
      }
    }
  }

  Widget prepareTitleWidget(Widget source) {
    if (source is Text) {
      if (source.style == null) {
        return Text(source.data!, style: TextStyle(fontSize: 20));
      }
    }

    return source;
  }
}

class OrangeBordersBox extends StatelessWidget {
  final Widget? child;
  final double width;

  OrangeBordersBox({this.child, this.width = 30});

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 30,
      width: width,
      decoration: BoxDecoration(border: Border.all(color: Colors.orange)),
      child: child,
    );
  }
}

```