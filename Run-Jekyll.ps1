$docsPath = "$PSScriptRoot/docs";
$sitePath = "$PSScriptRoot/_site";

& jekyll build --source $docsPath --destination $sitePath

& jekyll serve --port 4001 --open-url true