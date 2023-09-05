```dart
class EmailForm extends StatelessWidget {
  const EmailForm({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.all(20),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        boxShadow: [Shadows.button(context)],
        border: Border.all(color: Lamp.purpleEnd, width: 3),
        color: Theme.of(context).colorScheme.surface,
      ),
      child: EmailFormContent(),
    );
  }
}

class EmailFormContent extends StatefulWidget {
  const EmailFormContent({
    Key? key,
  }) : super(key: key);

  @override
  State<EmailFormContent> createState() => _EmailFormContentState();
}

class _EmailFormContentState extends State<EmailFormContent> {
  String input = '';

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Icon(Icons.email, color: Theme.of(context).colorScheme.onSurface,),
            SizedBox(width: 10,),
            Expanded(child: TextField(
              keyboardType: TextInputType.emailAddress,
              onChanged: (value) => setState(() => input = value),
              decoration: InputDecoration(
                hintText: 'Email',
              ),
            ))
          ],
        ),
        SizedBox(height: 20,),
        ActionButton.chip(
          capture: 'Get in', 
          onClick: () {
            Move.of(context).to(() => EmailInputProcessor(input));
          }, 
          isActive: isEmail(input),
        )
      ],
    );
  }
}

class EmailInputProcessor extends Processor {
  String email;

  EmailInputProcessor(this.email);
  
  @override
  Future<Widget> process() async {
    onboarding.User user;

    try { 
      user = await Auth.getIn(email); 
    } 
    on UnsuccessfulResponseException catch (ure) { 
      return ure.response.statusCode == 400 ? BadCodePage() : EmailUnexpectedErrorPage(); 
    }
    catch (ex) { 
      return EmailUnexpectedErrorPage();
    }

    return user.onboardedDate == null ? ExpensesOnboarding() : HomeGate();
  }
}

abstract class Processor extends StatelessWidget {
  
  @override
  Widget build(BuildContext context) {
    navigateInFuture(context);
    return SpinnerPage();
  }

  Future navigateInFuture(BuildContext context) async {
    var targetPage = await process();
    FinalMove.of(context).to(() => targetPage);
  }
  
  Future<Widget> process();
}

class EmailUnexpectedErrorPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return UnexpecteErrorPage(
      actions: [
        EmailForm(),
        BlocksSpace(),
        TelegramLoginActionButton(),
      ],
    );
  }
}
```