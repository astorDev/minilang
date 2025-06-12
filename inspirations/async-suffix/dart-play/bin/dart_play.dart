import 'dart:convert';
import 'package:http/http.dart' as http;

Future<void> main(List<String> arguments) async {
  final res = await http.get(Uri.parse(
    'https://raw.githubusercontent.com/astorDev/minilang/refs/heads/main/hello.json'
  ));

  final json = const JsonEncoder.withIndent('  ')
    .convert(jsonDecode(res.body));

  print(json);
}
