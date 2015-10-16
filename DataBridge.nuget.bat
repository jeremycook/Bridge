mkdir build

tools\nuget.exe pack DataBridge\DataBridge.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory build
tools\nuget.exe pack DataBridge.Db\DataBridge.Db.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory build
