# Changelogs

## v2.0
### General Improvements
- Refactored the entire bot code to align with breaking changes and new features introduced in the updated DSharpPlus v5.x.x-nightly version.
- Improved code structure and ensured compatibility with the latest DSharpPlus API.
- Deleted unused and empty files to streamline the codebase.
- Removed unused classes and optimized folder structure by moving classes to appropriate locations.
- Updated coding standards: new classes will no longer be defined as sealed, and existing sealed classes have been converted to non-sealed.
- Optimized code imports and improved overall code styling for consistency.
- Enhanced logging configuration with better file-based logging.
- Removed anti-pattern implementations and unused projects for better modularity.

### Authentication and Authorization
- Added Discord OAuth2 authentication with cookie-based session management.
- Configured access token storage in claims for session-based usage.
- Secured Hangfire Dashboard with a custom authorization filter.
- Introduced policy-based authorization for the Dashboard, leveraging ASP.NET Core policies for dynamic and centralized access control.
- Added support for database-driven dynamic permissions using custom authorization handlers.
- Implemented hierarchical policies like SupportOrHigher and ModeratorOrHigher for flexible role-based access control.
- Improved the authorization system to dynamically evaluate permissions from the database while still supporting attribute-based authorization.
- Implemented web panel access control, allowing only users directly added to teams to access the panel.
- Discord role members, even if roles are added to teams, will not have panel access unless explicitly added as team members.
- Ensured teams with Owner permission level always have access to the web panel.
- Adjusted GetTeamPermissionLevelHandler.cs to make roleList nullable, enabling calls without passing a role list.
- Refactored IPermissionCheck interface and PermissionCheckAttribute for better integration with the pipeline.
- Introduced a MediatR pipeline behavior to enforce AuthorizedUserId checks dynamically for commands and queries.
- Centralized authorization logic in the pipeline for improved logging and data collection.
- Added extension methods to simplify permission validation and improve code reusability.

### Features and Enhancements
- Moved the dashboard to /dashboard URI.
- Added a landing page at / with a login button.
- Enhanced the result page with new buttons for authorized users.
- Added a simple account view dialog for users to manage their account details.
- Implemented auto setup on server startup if the bot is added to the main server.
- GuildOptions access level can now only be changed by users with Owner access level.

### Bug Fixes
- Fixed authorization logic to properly handle failed authentication states.
- Added a new error message for unauthorized access.
- Fixed a bug where selecting a ticket type did not update the message correctly.
- Fully fixed average response time calculation to only consider the first responses between moderators and users.
- Removed unnecessary logging to the Discord log channel; only essential information is now logged.
- Changed log level to Verbose of exception logger for warning exceptions.

### UI and UX Improvements
- Updated the option page to make labels more descriptive.
- Updated team management methods and UI to reflect new access control logic.
- Improved UI consistency across the application.