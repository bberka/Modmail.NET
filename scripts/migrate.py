import os
import subprocess
import argparse

# --- Configuration ---
CORE_PROJECT = "../src/Modmail.NET"
WEB_PROJECT = "../src/Modmail.NET.Web.Blazor"
MIGRATIONS_PROJECTS = {
    "sqlserver": "../src/Modmail.NET.Migrations.SqlServer",
    "postgresql": "../src/Modmail.NET.Migrations.PostgreSql",
    "mysql": "../src/Modmail.NET.Migrations.MySql",
    "sqlite": "../src/Modmail.NET.Migrations.SQLite",
}
DB_CONTEXT = "ModmailDbContext"


def log(message, level="INFO"):
    """Logs a message with a given level."""
    levels = {
        "INFO": "\033[94m[INFO]\033[0m",
        "SUCCESS": "\033[92m[SUCCESS]\033[0m",
        "ERROR": "\033[91m[ERROR]\033[0m",
        "DEBUG": "\033[93m[DEBUG]\033[0m",
    }
    print(f"{levels.get(level, '[INFO]')} {message}")


def run_command(command, env=None):
    """Runs a shell command and returns the result."""
    log(f"Executing command: {' '.join(command)}", level="DEBUG")
    try:
        result = subprocess.run(
            command,
            check=True,
            capture_output=True,
            text=True,
            env=env,
        )
        log(result.stdout.strip(), level="INFO")
        return result
    except subprocess.CalledProcessError as e:
        log(f"Command failed with error: {e.stderr.strip()}", level="ERROR")
        log(f"Command output: {e.stdout.strip()}", level="DEBUG")
        return None


def run_ef_command(command, project, migration_name, db_provider):
    """Runs the EF Core command for the given project."""
    log(
        f"Running '{command}' for provider '{db_provider}' on project: {project}",
        level="INFO",
    )
    extra_args = []
    if command == "add":
        extra_args = ["--output-dir", "Migrations"]
    elif command == "script":
        extra_args = ["--output", f"{os.path.basename(project)}.sql"]
    elif command == "database":
        command = "update"
    elif command == "remove":
        migration_name = ""

    # Set the DbProvider environment variable
    env = os.environ.copy()
    env["DbProvider"] = db_provider

    ef_command = [
        "dotnet",
        "ef",
        "migrations",
        command,
        migration_name,
        "--project",
        project,
        "--startup-project",
        WEB_PROJECT,
        "--context",
        DB_CONTEXT,
        *extra_args,
    ]
    if command == "update":  # Run database update
        ef_command = [
            "dotnet",
            "ef",
            "database",
            "update",
            "--project",
            WEB_PROJECT,
        ]

    # Run the EF Core command
    result = run_command(ef_command, env=env)
    if result is None:
        log(
            f"Failed to execute EF Core command for provider '{db_provider}'",
            level="ERROR",
        )
        return 1
    log(
        f"EF Core command completed successfully for provider '{db_provider}'",
        level="SUCCESS",
    )
    return 0


if __name__ == "__main__":
    parser = argparse.ArgumentParser(
        description="Manage EF Core migrations across multiple projects."
    )
    parser.add_argument(
        "command",
        choices=["add", "remove", "script", "database"],
        help="The command to execute.",
    )
    parser.add_argument(
        "migration_name",
        nargs="?",
        default="",
        help="The name of the migration (required for 'add').",
    )
    args = parser.parse_args()
    if args.command == "add" and not args.migration_name:
        log("Migration name is required for 'add' command", level="ERROR")
        exit(1)

    for db_provider, project in MIGRATIONS_PROJECTS.items():
        log(
            f"Processing provider '{db_provider}' with project '{project}'",
            level="INFO",
        )
        # Run a build command first to ensure the project builds successfully
        build_command = ["dotnet", "build", project]
        build_result = run_command(build_command)
        if build_result is None:
            log(
                f"Build failed for project '{project}'. Skipping migration.",
                level="ERROR",
            )
            continue

        # Run the EF Core command
        if run_ef_command(args.command, project, args.migration_name, db_provider) != 0:
            log(f"Migration failed for provider '{db_provider}'", level="ERROR")
            exit(1)

    log("Operation completed successfully.", level="SUCCESS")
