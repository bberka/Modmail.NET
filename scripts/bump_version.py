import re

core_project_path = "..\\src\\Modmail.NET\\Modmail.NET.csproj"
blazor_project_path = "..\\src\\Modmail.NET.Web.Blazor\\Modmail.NET.Web.Blazor.csproj"


def get_current_version(project_path):
    """
    Fetch the current version from the .csproj file.
    """
    print(f"Fetching current version from {project_path}...")
    try:
        with open(project_path, "r") as file:
            content = file.read()
            match = re.search(r"<Version>(.*?)</Version>", content)
            if match:
                return match.group(1)
    except FileNotFoundError:
        print(f"File not found: {project_path}")
        return "0.0.0"

    print(f"No version found in {project_path}. Defaulting to 0.0.0.")
    return "0.0.0"


def compare_versions(current_version, new_version):
    """
    Compare two versions to ensure the new version is higher.
    Handles prerelease versions correctly (e.g., beta10 > beta9).
    """
    print(f"Comparing current version ({current_version}) with new version ({new_version})...")

    def version_tuple(version):
        """
        Convert version string to a tuple of (major, minor, patch, prerelease_type, prerelease_number)
        """
        match = re.match(r"(\d+)\.(\d+)\.(\d+)(?:-([a-zA-Z]+)(\d+))?", version)
        if not match:
            return None  # Invalid version format
        major, minor, patch, prerelease_type, prerelease_number = match.groups()
        return (
            int(major),
            int(minor),
            int(patch),
            prerelease_type or "",  # Use empty string if no prerelease
            int(prerelease_number) if prerelease_number else 0,  # 0 if no prerelease number
        )

    current_tuple = version_tuple(current_version)
    new_tuple = version_tuple(new_version)

    if current_tuple is None or new_tuple is None:
        print("Invalid version format in one or both versions.")
        return False  # Or raise an exception

    return new_tuple > current_tuple


def update_version(project_path, version):
    """
    Update the version in the .csproj file.
    """
    current_version = get_current_version(project_path)
    if compare_versions(current_version, version):
        print(f"Updating version in {project_path} to {version}...")
        try:
            with open(project_path, "r") as file:
                content = file.read()

            updated_content = re.sub(r"<Version>.*?</Version>", f"<Version>{version}</Version>", content)

            with open(project_path, "w") as file:
                file.write(updated_content)
        except FileNotFoundError:
            print(f"File not found: {project_path}")
    else:
        print(f"Error: New version {version} is not higher than current version {current_version} in {project_path}.")


def main():
    version = input("Enter the version number (e.g., 1.0.0 or 2.0.0-beta1): ")
    if not re.match(r"^\d+\.\d+\.\d+(-[a-zA-Z0-9]+)?$", version):
        print("Invalid version format. Please use the format X.Y.Z or X.Y.Z-prerelease (e.g., 1.0.0 or 2.0.0-beta1).")
        return

    current_core_version = get_current_version(core_project_path)
    current_blazor_version = get_current_version(blazor_project_path)

    if not compare_versions(current_core_version, version):
        print(f"Error: New version {version} is not higher than current version {current_core_version} in {core_project_path}.")
        return

    if not compare_versions(current_blazor_version, version):
        print(f"Error: New version {version} is not higher than current version {current_blazor_version} in {blazor_project_path}.")
        return

    update_version(core_project_path, version)
    update_version(blazor_project_path, version)
    print("Version bump complete!")


if __name__ == "__main__":
    main()