```dart
// ignore_for_file: prefer_const_constructors

import 'package:finrir/core-ui/theme.dart';
import 'package:finrir/core/state.dart';
import 'package:flutter/material.dart';

import '../../contexts/logic.dart';

class FiltersBar extends StatelessWidget {
  const FiltersBar({
    Key? key,
    required this.onRefresh,
  }) : super(key: key);

  final Function? onRefresh;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: HorizontalMargins.onlyLeft(),
      child: Align(
        alignment: Alignment.centerLeft,
        child: Wrap(
          alignment: WrapAlignment.start,
          crossAxisAlignment: WrapCrossAlignment.center,
          children: getRowChildren().toList(),
        ),
      ),
    );
  }

  Iterable<Widget> getRowChildren() sync* {
    yield Text('For ', style: TextStyle(fontSize: 16));
    yield PeriodFilterDropdown(onRefresh: onRefresh);
  }
}

class OverviewFiltersBar extends FiltersBar {
  const OverviewFiltersBar({
    Key? key,
    required onFiltersChanged,
  }) : super(key: key, onRefresh: onFiltersChanged);

  @override
  Iterable<Widget> getRowChildren() sync* {
    for (var oldChildren in super.getRowChildren()) {
      yield oldChildren;
    }

    if (Contexts.filter!.contexts.length > 1) {
      yield SizedBox(width: 5);
      yield Text('in ', style: TextStyle(fontSize: 16));
      yield ContextFilterDropdown(onRefresh: onRefresh);
      yield SizedBox(width: 5);
      yield Text('context', style: TextStyle(fontSize: 16));
    }
  }
}

class PeriodFilterDropdown extends StatelessWidget {
  const PeriodFilterDropdown({
    Key? key,
    required this.onRefresh,
  }) : super(key: key);

  final Function? onRefresh;

  @override
  Widget build(BuildContext context) {
    return DropdownButton(
        value: Filtering.state!.periodFilterValue,
        icon: DropdownIcon(),
        items: PeriodFilter.toMenuItems(),
        onChanged: (String? newValue) {
          if (Filtering.state!.periodFilterValue != newValue) {
            Filtering.state!.periodFilterValue = newValue!;
            onRefresh?.call();
          }
        });
  }
}

class ContextFilterDropdown extends StatelessWidget {
  const ContextFilterDropdown({
    Key? key,
    required this.onRefresh,
  }) : super(key: key);

  final Function? onRefresh;

  @override
  Widget build(BuildContext context) {
    return DropdownButton(
        value: Contexts.filter!.current,
        icon: DropdownIcon(),
        items: Contexts.filter!.toMenuItem().toList(),
        onChanged: (String? newValue) {
          if (newValue == null) return;
          Contexts.setFilter(newValue, after: onRefresh);
        });
  }
}

class DropdownIcon extends StatelessWidget {
  const DropdownIcon({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Icon(
      Icons.arrow_drop_down_sharp,
      color: Theme.of(context).colorScheme.primary
    );
  }
}
```