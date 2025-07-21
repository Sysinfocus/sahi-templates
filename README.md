# Templates for SAHI
Templates for Smart Automation by Human Intelligence (SAHI).

## What is SAHI?
It is a docker compose like language, where you create a template file to generate, replicate content based on conditions and run commands.
You can chain multiple files so repititive tasks can be isolated in separate files and only the main file is run to create entire application.
It is platform and language agnostic, hence can be used to rapidly build applications in any language of your choice with proper templating, in the first place.

Check out the example `Steps to use SAHI`

## Why SAHI?
Today, when every developer is after Artificial Intelligence (AI) to generate code, SAHI focuses on Human Intelligence (HI) where you craft the template which are repetitive in nature, carefully generate and replicate with placeholders to get deterministic results.

With AI, your output is occasionally correct but you need to verify to be sure it is what you wanted in the first place, but with `SAHI `, you are crafting the output without burning your pocket and the result is deterministic, in no time. You can use AI to generate `SAHI` file with templates and reuse it without paying for your tokens for what you have already are sure about.

## Language Commands, Arguments and Description
Commands are CASE SENSITIVE and should be avoided in arguments. In template `[\n]` will replace with NewLine, `[\t]` will replace with Tab, `''` will replace with double quote, `\\` will replace with single back slash.

| Commands | Arguments | Description 
| - | - | - |
| ADD | &lt;src&gt; &lt;des&gt; &lt;find&gt; &lt;replace&gt; | Copies &lt;src&gt; to &lt;des&gt; after replacing any &lt;find&gt;. | 
| CLEAR | | Clears &lt;condition&gt; for further Replications. | 
| COPY | &lt;from&gt; &lt;to&gt; &lt;src&gt; &lt;des&gt; &lt;ow?&gt; | Copies a file optionally with overwrite. | 
| COPYALL | &lt;from&gt; &lt;to&gt; &lt;src&gt; &lt;des&gt; &lt;ow?&gt; | Copies all files optionally with overwrite. | 
| CREATE | &lt;file&gt; &lt;content&gt; &lt;des?&gt; | Create a new &lt;file&gt; with &lt;content&gt; in the &lt;des?&gt;. | 
| DELETE | &lt;file&gt; &lt;des&gt; | Delete a &lt;file&gt; from the &lt;des&gt; folder. | 
| DELETEALL | &lt;file&gt; &lt;des&gt; | Delete all files recursively from &lt;des&gt; folder. | 
| GET | &lt;repo&gt; &lt;des&gt; | Clones/pulls a public &lt;repo&gt; into &lt;des&gt; folder. | 
| RUN | &lt;command&gt; &lt;args1&gt; &lt;args2&gt; ... | Runs a &lt;command&gt; using specified &lt;args#&gt;. | 
| IF | &lt;condition&gt; &lt;template&gt; | Replicates &lt;template&gt; when &lt;condition&gt; is met. | 
| INFO | | Starts showing messages. | 
| INIT | &lt;src&gt; &lt;des&gt; | Initializes a copy of &lt;src&gt; into &lt;des&gt;. | 
| NEXT | &lt;source_file&gt; | Chain &lt;source_file&gt; to execute. |
| PARAM | &lt;var&gt; &lt;value&gt; | Wherever &lt;var&gt; is declared, replace it with &lt;value&gt;. | 
| PROPS | &lt;comma-separated-prop1\|..prop2\|...&gt; | Pass properties separated by \| and , internally. | 
| RENAME | &lt;find&gt; &lt;replace&gt; &lt;overwrite?&gt; | Renames folders,files,content. Can overwrite. | 
| RENAMEALL | &lt;find&gt; &lt;replace&gt; | Renames all files,content matching with overwrite. | 
| REPLICATE | &lt;src&gt; &lt;find&gt; &lt;replace&gt; | Replicates &lt;props&gt; based on &lt;condition&gt; in all matching &lt;src&gt; files and replaces &lt;find&gt; with the given pattern. | 
| REPLACE | &lt;src&gt; &lt;find&gt; &lt;replace&gt; | Replaces &lt;find&gt; with &lt;replace&gt; in &lt;src&gt; file. | 
| REPLACEALL | &lt;src&gt; &lt;find&gt; &lt;replace&gt; | Replaces in all matching &lt;src&gt; files. | 
| SILENT | | Executes silently except for errors. |
| UNZIP | &lt;zipfile&gt; &lt;des&gt; &lt;overwrite?&gt; | Extracts &lt;zipfile&gt; to the &lt;des&gt; folder. |
| ZIP | &lt;src&gt; &lt;zipfile&gt; &lt;overwrite?&gt; | Creates a &lt;zipfile&gt; from the &lt;src&gt; folder. |

## Steps to use SAHI
1. Clone the repo.
2. Extract `sahi.exe` file from `sahi.zip`. This is the main executable file with no dependencies. Note: Currently only Win x64 is supported.
3. Add path to `sahi.exe` to your `System Environment` so you can run `sahi` from any folder on the system.
4. Based on the downloaded location of your templates, you need to modify `.sahi` file contents.
5. To generate a complete new solution with API, Blazor Wasm UI and Aspire Hosting, run
   ```
   sahi new-solution.sahi
   ```
6. To add a feature, in this case a Course feature is added, run
   ```
   sahi new-feature.sahi
   ```
7. That's it. You now have fully functional .NET API, Blazor Wasm UI with Aspire hosting.

## Release Notes
### 0.0.0.4
- You can now pass additional arguments from command line after the `.sahi` file which can be materialized inside `.sahi` file using `[\1] [\2] [\3]...` as respective arguments.
- GitHub repo link added.

### 0.0.0.3
- Use `ZIP` command to create .zip file from the given `<src>` folder with optional `overwrite`.
- Use `UNZIP` command to extract .zip file to the given `<des>` folder with optional `overwrite`.

### 0.0.0.2
- Use `SILENT` command to suppress information on console.
- Use `INFO` command to show information on console.
- Pass arguments from command line and use them in templates. For eg:
  ```
  sahi file.sahi Test
  ```
  `file.sahi` is the template file and `Test` is an additional arguments which can be used in template as `[\1]` which will be replace with `Test`

### 0.0.0.1
- First release
