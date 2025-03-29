import subprocess
import os

def commit_csproj_changes(version):
    """
    Commit changes to the .csproj files with the version update message.
    """
    print("Checking for .csproj changes...")

    # Check if there are any changes to the tracked files
    result = subprocess.run(['git', 'status', '--porcelain'], capture_output=True, text=True)
    changed_files = [line.split()[1] for line in result.stdout.splitlines()]
    csproj_files = [f for f in changed_files if f.endswith(".csproj")]

    if not csproj_files:
        print("No .csproj files have changed. Skipping commit.")
        return False  # Indicate no commit was made

    print(f"Staging .csproj file : {csproj_files}")
    # Add only the .csproj files to the staging area
    subprocess.run(['git', 'add'] + csproj_files, check=True)

    print("Commiting staging items to git")
    # Commit the changes with a message like "chore(release): Update version to x.y.z"
    result = subprocess.run(['git', 'commit', '-m', f'chore(release): Update version to {version}'], capture_output=True, text=True)
    print(result.stdout.decode())
    if "no changes added to commit" in result.stdout:
        print("No .csproj files changed. Skipping commit.")
        return False

    print("Changes to .csproj files committed successfully.")
    return True  # Indicate a commit was made


def main():
    # Get the version from user input or from a file, environment variable, etc.
    version = input("Enter version (e.g., 1.2.3 or 1.2.3-beta1): ")

    if not commit_csproj_changes(version):
        print("No commit was necessary due to no changes on  .csproj files.")


if __name__ == "__main__":
    main()
