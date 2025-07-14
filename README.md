# Templates for SAHI
Templates for Smart Automation by Human Intelligence (SAHI).

## What is SAHI?
It is a docker compose like language, where you create a template file to generate, replicate content based on conditions and run commands.
You can chain multiple files so repititive tasks can be isolated in separate files and only the main file is run to create entire application.
It is platform and language agnostic, hence can be used to rapidly build applications in any language of your choice with proper templating, in the first place.

Check out the example `Steps to use SAHI`

## Language Commands, Arguments and Description
Commands are CASE SENSITIVE and should be avoided in arguments. In template `[\n]` will replace with NewLine, `[\t]` will replace with Tab, `''` will replace with double quote, `\\` will replace with single back slash.
```
| Commands 	| Arguments 				| Description 					     	|
| ------------- | ------------------------------------- | ----------------------------------------------------- |
| ADD		| <src> <des> <find> <replace>		| Copies <src> to <des> after replacing any <find>.	|
| INIT 		| <src> <des>				| Initializes a copy of <src> into <des>. 		|
| RUN 		| <command> <args1> <args2> ...		| Runs a <command> using specified <args#>. 		|
| IF		| <condition> <template>		| Replicates <template> when <condition> is met. 	|
| CLEAR 	| 					| Clears <condition> for further Replications. 		|
| NEXT 		| <source_file>				| Chain <source_file> to execute. 			|
| PARAM 	| <var> <value>				| Wherever <var> is declared, replace it with <value>. 	|
| PROPS		| <comma-separated-prop1| ..prop2| ...>	| Pass properties separated by | and , internally. 	|
| REPLACE	| <src> <find> <replace>		| Replaces <find> with <replace> in <src> file. 	|
| REPLACEALL	| <src> <find> <replace>		| Replaces in all matching <src> files. 		|
| RENAME	| <find> <replace> <overwrite?>		| Renames folders,files,content. Can overwrite. 	|
| RENAMEALL	| <find> <replace>			| Renames all files,content matching with overwrite. 	|
| COPY		| <from> <to> <src> <des> <ow?>		| Copies a file optionally with overwrite. 		|
| COPYALL	| <from> <to> <src> <des> <ow?>		| Copies all files optionally with overwrite. 		|
| DELETE	| <file> <des>				| Delete a <file> from the <des> folder. 		|
| DELETEALL	| <file> <des>				| Delete all files recursively from <des> folder. 	|
| CREATE	| <file> <content> <des?>		| Create a new <file> with <content> in the <des?>. 	|
| GET		| <repo> <des>				| Clones/pulls a public <repo> into <des> folder. 	|
| REPLICATE	| <src> <find> <replace>		| Replicates <props> based on <condition> in all 	|
| 		| 					| matching <src> files and replaces <find> with 	|
| 		| 					| the given pattern. 					|
```

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
