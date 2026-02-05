import subprocess

publish_directory = "..\\publish"
blazor_project_path = "..\\src\\Modmail.NET.Web.Blazor\\Modmail.NET.Web.Blazor.csproj"


def publish_blazor_app(blazor_project_path, publish_directory):
    """
    Publish the Blazor app using the dotnet CLI.
    """
    print("Publishing Blazor project...")
    try:
        result = subprocess.run(
            ["dotnet", "publish", blazor_project_path, "-c", "Release", "-o", publish_directory],
            check=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
        )
        print(result.stdout.decode())
        print(f"Blazor app published successfully to {publish_directory}!")
    except subprocess.CalledProcessError as e:
        print(f"Failed to publish Blazor app: {e.stderr.decode()}")


def main():
    publish_blazor_app(blazor_project_path, publish_directory)


if __name__ == "__main__":
    main()
