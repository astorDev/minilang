# Let's Ditch the Async Suffix in C#

> 3 Arguments Against Using the Async Suffix in Modern .NET Apps!

In 2012, C# received the `async`/`await` keywords and a new way of doing asynchronous programming we still use and love. Along with it came the `Async` suffix that also leaks into our codebases to this day. With the suffix, a programmer could know straight away that a certain method is async and use the syntactic sugar accordingly. Nowadays, the suffix is something very comfy and familiar to every C# developer, and it has even lurked into Microsoft's naming guidelines.

Still, I propose to stop using the suffix. Of course, I'm not the first one to do that. Developers and even companies have already made the move and outlined their reasoning (see NServiceBus "No Async Suffix"). This article lays out all the arguments I've gathered out there for ditching the suffix.

## Hungarian Notation a.k.a. It Is Pure Noise.

Hungarian notation, introduced by Charles Simonyi at Microsoft in the 1970s, is a naming convention where variable names include type information (e.g. `strName` for a string, `bIsReady` for boolean). It became widespread in C, C++ throughout the 1980s and 1990s, helping developers manage type information in weakly-typed environments. 

Sometimes, we can still see relics of the notation like `tbl_TableName` in SQL tables, `m_count` for member (private) variables, or `lstItems` for lists. However, with IDEs getting better, the notation has become increasingly redundant. Following the principle of "Don't do something that doesn't bring any value," the overwhelming majority of programming language guidelines discourage the use of Hungarian Notation nowadays.

In essence, the `Async` suffix is an example of Hungarian notation (or reverse Hungarian notation if you will). Although it looks slightly different from classic examples, it’s comparable to other cases where we could use such notation, but choose not to. For instance, as highlighted in the [NServiceBus article on the topic](https://docs.particular.net/nservicebus/upgrades/5to6/async-suffix#reason-for-no-async-suffix-nservicebus-apis-do-not-follow-hungarian-notation):

- Methods are not suffixed with the name of the type they return.
- Classes are not suffixed with "Instance" or "Static".
- Members are not suffixed with access modifier names such as "Protected" or "Public".

> There's a notable exception: the `I` prefix in interfaces, which even NServiceBus uses. Perhaps that's a topic for another discussion.

To summarize, `Async` suffix, like other examples of Hungarian Notation, adds noise to the code base, instead of relying on an IDE and the compiler. But if it's truly that useless, why are so many proponents of it and why was it used by Microsoft in the first place? Let's discuss it in the next section.

## Async Is The New Norm.

> What if I have a sync version of the same method?

> We don't need to differentiate methods anymore.

> You will get a warning anyway.

## No Other Language Does This!

> Even Java, notorious for its verbosity, doesn't use the suffix.

## TL;DR

In short, here are the 3 arguments against the Async suffix:

1. It is a relic of Hungarian Notation. In principle, we shouldn't write something that doesn't bring value, and with modern tooling, we are already able to identify an async method quite easily.
2. Even when there's a non-async version of the method async one should be used. Therefore, it's better to make it harder to use the non-async version by adding the `Sync` suffix.
3. Perhaps for the two reasons above, no other programming language has adopted the convention. C#'s insistence on the antique verbosity seems to contradict its strive for more adoption.

This article is my two cents to inspire more minimalism in modern coding — C# in particular. In the meantime, I'm developing a minimalistic programming language called minilang, and this article serves as an inspiration for it. Check it out [here on GitHub](https://github.com/astorDev/minilang) and don't hesitate to give it a star! 🌟

Claps for this article are also highly appreciated! 😉
