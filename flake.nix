{
  description = "Prompt Archive development shell";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-25.05";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs { inherit system; };
        dotnetSdk = pkgs.dotnetCorePackages.sdk_10_0-bin;
      in
      {
        devShells.default = pkgs.mkShell {
          packages = with pkgs; [
            nodejs_22
            dotnetSdk
            ];

          DOTNET_CLI_TELEMETRY_OPTOUT = "1";
          DOTNET_NOLOGO = "1";
          ASPNETCORE_ENVIRONMENT = "Development";

          DOTNET_ROOT = "${dotnetSdk}";

          shellHook = ''
            export PATH="$PWD/src/frontend/node_modules/.bin:$PATH"

            echo "Prompt Archive dev shell"
            echo
            echo "Frontend (Cursor):"
            echo "  cd src/frontend && npm install && npm run dev"
            echo
            echo "Backend (Rider/.NET CLI):"
            echo "  cd src/backend/PromptArchive && dotnet restore && dotnet run"
          '';
        };
      }
    );
}
