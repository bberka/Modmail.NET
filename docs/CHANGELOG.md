# **Changelogs**

## **v2.3**
### Features

- **Tag System Implementation:** Introduced a tag system, allowing users to trigger pre-defined responses from the bot using a 'tag' slash command.
  - Implemented web UI and database infrastructure for managing tags (create, view, update, delete).
  - Added command handlers for tag retrieval and execution.
  - Implemented a `TagProvider` class to dynamically generate slash commands for available tags.
  - Implemented a `TagBotMessages` helper class to centralize the creation of messages sent by the bot when using tags.

- **Enhanced User Engagement with Mirrored Reactions:** Reactions added by moderators in ticket channels are now automatically mirrored to the corresponding user's direct messages, and vice versa, creating a more interactive communication experience.

- **Implement mirrored message deletion** The bot now removes original user messages from DMs after they are removed to maintain organization and clarity of communications.

-  **Feedback implementation:** The feedback submission system is improved and a data list has been added to improve visibility of user interaction.

- **Automatic Ticket Data Deletion and Optional Timeout**: Added support for automatically deleting timeout tickets.

- **Add preliminary permission policies (currently inactive)** Added the configuration for authorization to be tested and applied by admins once the implementation is finished.

- **Enhance Guild Option UI with visual separators and descriptions:** the visual clarity is improved by seperating groups of guild options through horizontal lines.

### Bug Fixes

- Fixed minor UI text issues
- Fixed incorrect metric calculation
- Fixed bot intents
- Fixed multiple feedback submissions
### Dependency Updates

- Updated DSharpPlus to the latest NuGet nightly version.



## **v2.2**


### Features

- **Attachment Handling Overhaul:**
  - Implemented full support for attachment uploads to tickets. Attachments are now downloaded to the server and then uploaded to Discord.
  - The bot now reacts to mod messages containing attachments instead of deleting and resending the message, improving visual clarity.
  - Attachment URLs are now served from the application's domain instead of Discord's, providing better control and security.
- **Configurable Ticket Data Deletion:**
  - Added the ability to automatically delete ticket data (including attachments) after a configurable period. This feature can be enabled and configured in the options UI.
  - Introduced a daily job to handle the deletion process.
- **Optional Ticket Timeout:**
  - Ticket timeouts can now be disabled entirely. The default timeout value is now -1 (disabled). Existing tickets with a timeout value of -1 will not be automatically closed.
- **Configurable Statistics Calculation Period:**
  - Administrators can now configure the number of days used for calculating statistics. This setting is available in the options UI.
- **Public Attachment Access:**
  - Implemented a public API endpoint to serve attachment files based on their ID, enhancing accessibility.

### Bug Fixes

- **GuildOption Update Issue:** Resolved a bug where changes to `GuildOption` settings in the UI were affecting the cached reference without calling save changes. Caching has been disabled for the options page to ensure data consistency.
- **Invalid Ticket ID Handling:** The web UI now correctly redirects to a 404 page when attempting to view a transcript for an invalid ticket ID.
- **Blacklist Permission Check:** Fixed an issue where the `CheckUserBlacklistStatusQuery` incorrectly required user authorization.
- **Ticket Timeout Logic:** Modified the ticket timeout processing to only consider tickets with valid (non-disabled) timeout values.
- **Dependency Injection Issue:** Addressed a dependency injection issue with `AddHttpClient`.
- **Database Migration Issues:** Corrected database migration issues related to required fields and constraint minimum values.

### Improvements & Refactoring

- **Performance Optimizations:**
  - Optimized permission info retrieval by converting the result type to an array.
  - Improved performance by moving shared code to a common method and using `using` statements for variable scope management.
- **Codebase Cleanup:**
  - Removed an unused encryption key.
  - Removed the unused `MessageSent` color.
- **Queue Handling:** Migrated ticket attachment downloads to a singleton service for better management and dependency injection.
- **SmartEnum Naming:** Fixed the naming of the `HangfireQueueName` SmartEnum to comply with Hangfire's requirements (lowercase letters).
- **Query Simplification:** Inlined the `GetTimedOutTicketListQuery` into the job class, simplifying the codebase.

### Dependency Updates

- Updated NuGet packages to the latest versions.
- Added the `Microsoft.Extensions.Http` NuGet package for managing `HttpClient` instances.
- Added the `EntityFrameworkCore.Exceptions` NuGet package for better exceptions.
- Added the `EFCore.CheckConstraints` NuGet package for data validation.

## **v2.1**

### Features
- Added a new guild option to enable/disable public access to ticket transcripts (`AllowPublicAccessToTranscripts`).
- Added a new guild option to control whether the transcript link is sent to the user upon ticket closure (`SendTranscriptLinkToUser`).

### Bug Fixes
- Fixed a UI issue in the transcript page that caused a gap to appear at the bottom, ensuring the content area uses the available height correctly.

### Improvements & Refactoring
- Removed the character limit for ticket message content in the database, allowing for longer messages without truncation.
- Standardized the return types of response static methods for improved code consistency and predictability.

### Logging
- Added a log message to the configured Discord log channel when a user is successfully removed from the blacklist.

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
- **Ticket Closing Confirmation:**
  - Enforced a confirmation dialog for all ticket closings initiated via button interactions in the initial ticket message.
  - Removed the direct close functionality from the "Close Ticket" button.
  - Renamed the "Close Ticket with Reason" button to "Close Ticket" for clarity.
  - Made the closing reason field in the confirmation modal optional.
  This change ensures users always confirm ticket closures, preventing accidental or unintended closures.

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