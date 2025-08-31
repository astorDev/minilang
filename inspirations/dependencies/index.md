I'm trying to figure out a system for developing a frameworks (containing of multiple libraries).

Here's an example situation for dependencies:

```
lib-a
lib-b (depends on lib-a)
lib-c
lib-d (depends on lib-b and lib-c)
```

The question is how to do the dependencies references. In case of C# I typically do a direct reference at first, then when published I move it to the package reference. Although this sort of works it adds an overhead when using package references in the future, since brings a need to use package reference instead of a direct one while developing. Which brings two problems:

a. A need to wait for the publishing of the package
b. Inability to debug code in the dependency.

I'm wondering if there's a more "fluent" approach both in case of C# and JavaScript. I assume a direct reference in case of JavaScript is a `workspace` reference.

## Complication 1: Automated Versioning

Scenario:

- Updates done in `lib-a` and `lib-b` uses the updated right ahead.
- Problem: Newer version of `lib-a` will not be yet published.