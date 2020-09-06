# Git

Set your name and email accordingly.

    git config --global user.name "My Name"
    git config --global user.email "My Email"

It is recommended to switch *pull* to *fast-forward only* mode.

    git config --global pull.ff only

Also disable tracking of executable permissions.
This is helpful when running Git from within [WSL](https://en.wikipedia.org/wiki/Windows_Subsystem_for_Linux).

    git config core.filemode false

You can ignore personal files using `.git/info/exclude`.

## Commit Messages

Read through [*How to Write a Git Commit Message*](https://chris.beams.io/posts/git-commit/) and adhere to it where it makes sense.

Put issue references in the body of the commit message.
GitLab may not pick them up correctly from the subject line.

Consider prefixing the subject if the commit only touches a specific part of the codebase (e.g. `Docs:` or `Scripts:`).

## Branches

Keep your work in descriptively named branches, prefixed with your GitLab username, followed by a slash (e.g. `rocky44r/combo-mechanic`).

Alternatively, if there is already an issue present in the tracker, use the issue number and title (e.g. `137-mini-map`).
