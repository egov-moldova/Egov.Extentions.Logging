# AI Agents & Automation

This project is built and maintained with the help of AI agents. We embrace automation to ensure high quality and consistency.

## Guidelines for AI Agents

When working on this repository, AI agents should:
1.  **Follow existing code style**: Maintain consistency with the current codebase.
2.  **Ensure test coverage**: Every new feature or bug fix should include corresponding unit tests.
3.  **Update documentation**: Ensure README and XML comments are up-to-date.
4.  **Use structured logging**: Prefer `LoggerEvent` for all logging operations.
5.  **Respect repository structure**: Keep production code in `src/`.

## Automation Workflows

We use GitHub Actions for:
-   **CI**: Building and testing every PR and commit to the main branch.
-   **CD**: Automatically packing and publishing NuGet packages on release.
