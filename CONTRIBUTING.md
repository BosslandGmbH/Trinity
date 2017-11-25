# Contributing to Trinity

## Issues

Feel free to open an issue if you think you have found a bug, or if you have a request
for an enhancement. Please include as much information as possible when opening these.
This includes:
* Log files
* Screenshots. Make sure to blur out names and other uniquely identifying information.
* Locations
* Steps to reproduce

See [here](https://help.github.com/articles/file-attachments-on-issues-and-pull-requests/) for how to attach files to issues.

## Pull requests

Pull requests are always welcome! Please make sure to follow our guidelines when submitting one:
* Coding style: We use the .NET foundation coding style. You can read it [here](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md).  
Note that we will not accept pull requests that do not follow this coding style appropriately.
* Use coroutines instead of behavior trees. This rule applies if you are introducing new logic code.

If the change fixes a bug it is generally advisable to open an issue with details first. Similarly,
if the changeset is large, opening an issue first to discuss the change is a good idea to make
sure everything goes smoothly.

The repo includes a `.editorconfig` file. If you have an IDE that supports this file
it will automatically apply our indentation settings (spaces) when you open the files.
Otherwise you can [download a plugin](http://editorconfig.org/) for your favorite IDE.
This is optional but is very useful if you are switching between projects with different
indentation settings.

## Git Commit Guidelines

### Commit content

Do your best to factor commits appropriately, i.e not too large with unrelated
things in the same commit, and not too small with the same small change applied N
times in N different commits. If there was some accidental reformatting or whitespace
changes during the course of your commits, please rebase them away before submitting
the PR.

### Commit Message Format
Please format commit messages as follows (based on this [excellent post](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)):

```
Summarize change in 50 characters or less

Provide more detail after the first line. Leave one blank line below the
summary and wrap all lines at 72 characters or less.

If the change fixes an issue, leave another blank line after the final
paragraph and indicate which issue is fixed in the specific format
below.

Fix #42
```

Important things you should try to include in commit messages include:
* Motivation for the change
* Difference from previous behaviour
* Whether the change alters the public API, or affects existing behaviour significantly
