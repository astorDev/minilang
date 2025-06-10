# Let's Ditch the Async Suffix in C#

> 3 Arguments Against Using the Async Suffix in Modern .NET Apps!

In 2012, C# received the `async`/`await` keywords and a new way of doing asynchronous programming we still use and love. Along with it came the `Async` suffix that also leaks into our codebases to this day. With the suffix, a programmer could know straight away that a certain method is async and use the syntactic sugar accordingly. Nowadays, the suffix is something very comfy and familiar to every C# developer, and it has even lurked into Microsoft's naming guidelines.

Still, I propose to stop using the suffix. Of course, I'm not the first one to do that. Developers and even companies have already made the move and outlined their reasoning (see NServiceBus "No Async Suffix"). This article lays out all the arguments I've gathered out there for ditching the suffix.

## Hungarian Notation a.k.a. It Is Pure Noise.

> You will get a warning anyway.

## Async Is The New Norm.

> We don't need to differentiate methods anymore.

## No Other Language Does This!

> Even Java, notorious for its verbosity, doesn't use the suffix.

## TL;DR

In short, here are the 3 arguments against the Async suffix:

1. It is a relic of Hungarian Notation. In principle, we shouldn't write something that doesn't bring value, and with modern tooling, we are already able to identify an async method quite easily.
2. Even when there's a non-async version of the method async one should be used. Therefore, it's better to make it harder to use the non-async version by adding the `Sync` suffix.
3. Perhaps for the two reasons above, no other programming language has adopted the convention. C#'s insistence on the antique verbosity seems to contradict its strive for more adoption.

This article is my two cents to inspire more minimalism in modern coding — C# in particular. In the meantime, I'm developing a minimalistic programming language called minilang, and this article serves as an inspiration for it. Check it out [here on GitHub](https://github.com/astorDev/minilang) and don't hesitate to give it a star! 🌟

Claps for this article are also highly appreciated! 😉
