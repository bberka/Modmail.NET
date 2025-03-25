# **Changelogs**

## **v2.0**

### **General Improvements**
- Refactored the entire bot code to align with breaking changes and new features introduced in the updated **DSharpPlus v5.x.x-nightly** version.
- Improved code structure and ensured compatibility with the latest DSharpPlus API.
- Removed unused and empty files, classes, and projects to streamline the codebase.
- Optimized folder structure by moving classes to appropriate locations.
- Updated coding standards:
    - New classes will no longer be defined as `sealed`.
    - Converted existing `sealed` classes to non-sealed.
- Optimized code imports and improved overall code styling for consistency.
- Enhanced logging configuration with better file-based logging.
- Removed anti-pattern implementations for better modularity.
- Added `.editorconfig` for consistent code styling across the project.

---

### **Authentication and Authorization**
- **Discord OAuth2 Authentication**:
    - Added cookie-based session management.
    - Configured access token storage in claims for session-based usage.
- **Authorization Enhancements**:
    - Secured Hangfire Dashboard with a custom authorization filter.
    - Introduced policy-based authorization for the Dashboard, leveraging ASP.NET Core policies for dynamic and centralized access control.
    - Added support for database-driven dynamic permissions using custom authorization handlers.
    - Implemented hierarchical policies like `SupportOrHigher` and `ModeratorOrHigher` for flexible role-based access control.
    - Improved the authorization system to dynamically evaluate permissions from the database while still supporting attribute-based authorization.
- **Web Panel Access Control**:
    - Restricted web panel access to users directly added to teams.
    - Discord role members, even if roles are added to teams, will not have panel access unless explicitly added as team members.
    - Ensured teams with `Owner` permission level always have access to the web panel.
- **Permission Validation**:
    - Refactored `GetTeamPermissionLevelHandler.cs` to make `roleList` nullable, enabling calls without passing a role list.
    - Refactored `IPermissionCheck` interface and `PermissionCheckAttribute` for better integration with the pipeline.
    - Introduced a MediatR pipeline behavior to enforce `AuthorizedUserId` checks dynamically for commands and queries.
    - Centralized authorization logic in the pipeline for improved logging and data collection.
    - Added extension methods to simplify permission validation and improve code reusability.
    - Added query call for authentication permission checks.
    - Refactored and fixed existing authentication permission check logic.
    - Teams can now only be managed by Admins and Owners.
    - Team permissions cannot be set higher than the user's own permission level.
    - Refactored `GetPermissionLevel` query to include roles dynamically instead of requiring a role list.

---

### **Features and Enhancements**
- **Navigation and Pages**:
    - Moved the dashboard to `/dashboard` URI.
    - Added a landing page at `/` with a login button.
- **User Management**:
    - Added a simple account view dialog for users to manage their account details.
- **Auto Setup**:
    - Implemented auto setup on server startup if the bot is added to the main server.
- **Guild Options**:
    - Restricted access to `GuildOptions` to users with `Owner` permission level.
    - "Always Anonymous" option now affects active tickets.
- **Result Page**:
    - Enhanced the result page with new buttons for authorized users.

---

### **Bug Fixes**
- Fixed authorization logic to properly handle failed authentication states.
- Added a new error message for unauthorized access.
- Fixed a bug where selecting a ticket type did not update the message correctly.
- Fully fixed average response time calculation to only consider the first responses between moderators and users.
- Removed unnecessary logging to the Discord log channel; only essential information is now logged.
- Changed log level to `Verbose` for warning exceptions in the exception logger.
- **UtilReadable.cs**: Fixed ordering logic to ensure correct return values.

---

### **UI and UX Improvements**
- Updated the option page to make labels more descriptive.
- Updated team management methods and UI to reflect new access control logic.
- Improved UI consistency across the application.
- Changed sidebar order for better navigation.
- Simplified labels in the Options UI.
