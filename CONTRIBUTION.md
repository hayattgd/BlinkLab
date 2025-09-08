
Any PRs that don't follow this rule may be rejected or renamed or edited.

# Commit prefix

Commit message should be written in this format:

```
prefix: description
```

| Prefix  | Description                                 |
|:-------:|:-------------------------------------------:|
| docs    | Changes to documents (like README.md)       |
| fix     | Fix / patches for known issues              |
| style   | Code style changes (indents, brackets)      |
| perf    | Performance Improvements                    |
| refactor| Code changes which doesn't affect behavior  |
| feat    | New features to the application             |
| chore   | Others (dependency, shell script, etc...)   |

### Examples

- style: Reorder constructor and fields on NewProjectDialog
- feat: Added templates for new project

# Namespace

| `namespace`                 | Description                               |
|:---------------------------:|:-----------------------------------------:|
| `BlinkLab.Editor`           | Editor specific codes                     |
| `BlinkLab.Runtime`          | Runtime specific codes                    |
| `BlinkLab.Engine`           | Shared codes between editor and runtime   |
| `BlinkLab.Engine.Rendering` | Shared Rendering-related codes            |
