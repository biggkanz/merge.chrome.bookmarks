#r @"C:\Users\biggskanz\.nuget\packages\fsharp.data\4.2.8\lib\netstandard2.0\FSharp.Data.dll"
#load "Merge.fs"
open MergeBookmark.Merge

let bookmarks1 =
    GetLinks @"D:\repos\fsharp\merge.chrome.bookmarks\MergeBookmark\test\bookmarks_35.html"
let bookmarks2 =
    GetLinks @"D:\repos\fsharp\merge.chrome.bookmarks\MergeBookmark\test\bookmarks_35_30dup.html"
    
bookmarks1 
|> Seq.length
|> printfn "bookmarks1: %d"

bookmarks2 
|> Seq.length
|> printfn "bookmarks2: %d"

GetDuplicates bookmarks1 bookmarks2
|> Seq.length
|> printfn "GetDuplicates: %d"

GetUnique bookmarks1 bookmarks2
|> Seq.length
|> printfn "GetUnique: %d"