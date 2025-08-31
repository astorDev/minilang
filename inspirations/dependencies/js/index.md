Here’s how your `package.json` files should look for local development with workspaces, and for publishing:

### 1. Local Development (with workspaces)

In your root `package.json`:
```json
{
  "private": true,
  "workspaces": [
    "lib-a",
    "lib-b",
    "lib-c",
    "lib-d"
  ]
}
```

In `lib-b/package.json`:
```json
{
  "name": "lib-b",
  "version": "1.0.0",
  "dependencies": {
    "lib-a": "workspace:*"
  }
}
```

In `lib-d/package.json`:
```json
{
  "name": "lib-d",
  "version": "1.0.0",
  "dependencies": {
    "lib-b": "workspace:*",
    "lib-c": "workspace:*"
  }
}
```

### 2. When Publishing

When you publish, the `workspace:*` is replaced by the actual version (e.g., `"lib-a": "^1.0.0"`).  
Consumers will see:
```json
{
  "dependencies": {
    "lib-a": "^1.0.0"
  }
}
```

**Summary:**  

- Use `"workspace:*"` for local development in workspaces.
- The dependency is still listed in `dependencies`, but the version is managed by the workspace.
- When publishing, the version is replaced with the actual version, and consumers install from npm as usual.

## How? 

It's replaced automatically by npm (and yarn/pnpm) when you publish.  
When you use `"lib-a": "workspace:*"` in your local `package.json`, npm uses your local workspace version for development.  
When you run `npm publish`, npm replaces `"workspace:*"` with the actual version of `lib-a` (from its `package.json`) in the published package.  
So consumers will see a regular versioned dependency, and everything "just works" without manual changes.