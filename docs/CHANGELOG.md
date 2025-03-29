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
  - Sealed classes currently only being used by SmartEnum inherited classes
- Optimized code imports and improved overall code styling for consistency.
- Enhanced logging configuration with better file-based logging.
- Removed anti-pattern implementations for better modularity.
- Added `.editorconfig` for consistent code styling across the project.
- Addressed almost all warnings reported by Rider's code analysis tools, improving code quality and compliance with recommended practices.
- Standardized naming conventions for `LangKeys` enum and fields across the project.
- Upgraded package versions for improved stability and access to new features.

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
  - Ticket messages are now displayed on their own page instead of a dialog.
- **User Management**:
  - Added a simple account view dialog for users to manage their account details.
  - Fixed account dialog behavior to close on overlay click.
- **Auto Setup**:
  - Implemented auto setup on server startup if the bot is added to the main server.
- **Guild Options**:
  - Restricted access to `GuildOptions` to users with `Owner` permission level.
  - "Always Anonymous" option now affects active tickets.
- **Result Page**:
  - Enhanced the result page with new buttons for authorized users.
- **Metrics and Statistics**:
  - Refactored the statistics entity for consistent naming conventions.
  - Renamed the "Statistic" feature to "Metric" for clearer terminology.
  - Added new charts to the dashboard for better visualization of metrics.
  - Increased caching duration for metrics to improve performance.
  - Improved statistic job performance by using `DefaultIfEmpty`, enabling direct DB average calculations.
  - Added default values to the statistic entity for use in the dashboard.
  - Separated the message line chart into user and moderator messages for more granular insights.
- **SmartEnum Integration**:
  - Converted `DbType` to a smart enum.
  - Enabled SmartEnum mapping for `DbContext`.
  - Set SmartEnum to `sealed` class for better performance and clarity.
- **GuidV7 Implementation**:
  - Replaced all usages of `Guid.NewGuid` with `Guid.CreateVersion7`.
  - Added a new database trigger to automatically set IDs for entities if not provided.
- **Public Discord Transcript Page View:** Implemented a public page for viewing Discord transcripts.

---

### **Bug Fixes**

- Fixed authorization logic to properly handle failed authentication states.
- Added a new error message for unauthorized access.
- Fixed a bug where selecting a ticket type did not update the message correctly.
- Fully fixed average response time calculation to only consider the first responses between moderators and users.
- Removed unnecessary logging to the Discord log channel; only essential information is now logged.
- Changed log level to `Verbose` for warning exceptions in the exception logger.
- Fixed team add and update permission check bug.
- Fixed dashboard UI issues, typos, and separation of average stats.
- Fixed ticket notes dialog width for better usability.
- Fixed unnecessary guild option value assignment on the options page.
- **Concurrency Issue with DB Context:** Fixed a concurrency issue that could occur with the database context.
- **Stack Overflow Error with `GetNow` Custom Date Function:** Resolved a stack overflow error that could occur with the `GetNow` custom date function.
- **Metric Response Mod Message Chart Data Assignment Issue:** Fixed an issue where metric response mod message chart data was not being assigned correctly.

---

### **UI and UX Improvements**

- Updated the option page to make labels more descriptive.
- Updated team management methods and UI to reflect new access control logic.
- Improved UI consistency across the application.
- Changed sidebar order for better navigation.
- Simplified labels in the Options UI.
- Updated UI images for better visual clarity.
- Teams UI default column behavior updated:
  - `RegisterDate` and `UpdateDate` columns are now hidden by default.

---

### **Documentation**

- Improved project documentation:
  - Separated some headers into their own markdown files for better organization.
  - Added direct links to the `img` folder instead of embedding images in markdown.
  - Created `ROADMAP.md` to outline future plans and features.
  - Created `COMMANDS.md` to document available commands.
  - Updated README.md with new details and images.
  - **README.md Update:** Removed `ROADMAP.md` and moved all roadmap elements to GitHub Issues. This centralizes feature planning and tracking within GitHub's issue management system.

### **Build and Automation**

- **Automated Release Workflow:** Added and configured a `.NET Release` GitHub Actions workflow (`dotnet-release.yml`) to automatically create releases on commits to the `master` branch. The workflow uses the version specified in the `.csproj` file to generate the release. This automates the release process and ensures that releases are consistently versioned.
- **Removed `commit cs proj` Script:** Removed the `commit cs proj` script, as the automated release workflow now handles version updates and release creation.