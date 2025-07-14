# Templates for SAHI
Templates for Smart Automation by Human Intelligence (SAHI).

## What is SAHI?
It is a docker compose like language, where you create a template file to generate, replicate content based on conditions and run commands.
You can chain multiple files so repititive tasks can be isolated in separate files and only the main file is run to create entire application.
It is platform and language agnostic, hence can be used to rapidly build applications in any language of your choice with proper templating, in the first place.

Check out the example `Steps to use SAHI`

## Steps to use SAHI
1. Clone the repo.
2. Extract `sahi.exe` file from `sahi.zip`. This is the main executable file with no dependencies. Note: Currently only Win x64 is supported.
3. Based on the downloaded location of your templates, you need to modify `.sahi` file contents.
4. To generate a complete new solution with API, Blazor Wasm UI and Aspire Hosting, run
   ```
   sahi new-solution.sahi
   ```
5. To add a feature, in this case a Course feature is added, run
   ```
   sahi new-feature.sahi
   ```
6. That's it. You now have fully functional .NET API, Blazor Wasm UI with Aspire hosting.
