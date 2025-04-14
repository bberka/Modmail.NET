# Installation Guide

This guide will walk you through setting up Modmail.NET.

## Prerequisites

*   Download the latest source code.
*   Install [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or higher.
*   Install [SQL Server](https://learn.microsoft.com/en-us/sql/sql-server/?view=sql-server-ver15) 2019 or higher.
*   [Docker](https://docs.docker.com/get-docker/) (Optional, for containerized deployment)
*   A Discord account with the Developer Portal enabled.

## Installation Steps

1. **Create Discord Application:**
      -   Log in to the [Discord Developer Portal](https://discord.com/developers/applications) and create a new application.

2. **Create Bot User:**
      -   Navigate to the "Bot" tab and create a bot user for your application.
      -   **Note:** Keep the "Public Bot" setting disabled unless you intend for anyone to add your bot to their server.

3. **Invite Bot to Server:**
      -   Navigate to the "OAuth2" tab and scroll below "URL Generator."
      -   Select the `bot` permissions and the following permissions or simply select `Administrator`:
            -   Send Message
            -   Send Messages in Threads
            -   Manage Messages
            -   Read Message History
            -   Add Reactions
            -   View Channels
            -   Manage Channels
            -   Attach Files
            -   Embed Links
            -   Mention Everyone 
            -   Use Slash Commands

      -   Copy the generated URL and paste it into your browser to invite the bot to your server.

4. **Set Redirect URI:**
      -   In the "OAuth2" tab, add `http://your-domain/auth/callback` to the "Redirects" list.
      -   **Important:** Replace `your-domain` with your actual domain or use `http://localhost:5000` for local development.

5. **Configure `appsettings.json`:**
      -   Publish the Blazor web project.
      -   Locate the `appsettings.json` file in the published output.
      -   Copy the "Token," "Client ID," and "Client Secret" from the Discord Developer Portal (Bot and General Information tabs) into the corresponding settings in `appsettings.json`.
      -   Update the following settings:
            -   `Bot:DbConnectionString`: Enter your SQL Server connection string.  **Example:** `Server=localhost;Database=Modmail_NET;User Id=your_user;Password=your_password;`
            -   Any other configuration options
          
6. **Start Application:**
      -   Run the Modmail.NET application.
      -   Open your web browser and navigate to `http://localhost:5000` (if running locally) or `http://your-domain` (if running on a server) to access the web UI.
