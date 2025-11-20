---
description: Add all changes, commit with a concise message, and push to current branch
---

Perform the following git workflow:

1. Run `git status` to see what changes exist
2. Run `git add .` to stage all changes
3. Analyze the staged changes using `git diff --cached` to understand what was changed
4. Create a concise, descriptive 1-sentence commit message that summarizes the changes
5. Commit with the message using `git commit -m "message"`
6. Push to the current branch using `git push`

The commit message should:
- Be a single sentence (no period at the end)
- Start with a verb in imperative mood (e.g., "Add", "Implement", "Fix", "Update", "Refactor")
- Be specific and descriptive but concise
- Focus on WHAT and WHY, not HOW

After completing all steps, confirm the push was successful.
