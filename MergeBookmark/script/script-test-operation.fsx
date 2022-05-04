#load "..\Utility.fs"
#load "..\Domain.fs"
#load "..\Parse.fs"
#load "..\Operations.fs"
#load "..\Merge.fs"

open MergeBookmark

let file30 = @"D:\repos\fsharp\merge.chrome.bookmarks\MergeBookmark\test\bookmarks_35_30dup.html"
let file35 = @"D:\repos\fsharp\merge.chrome.bookmarks\MergeBookmark\test\bookmarks_35.html "

let full =
    Utility.IO.ReadAllLines file35
    |> Operations.htmlToItem
    |> Operations.itemToBookmark
    
let dups =
    Utility.IO.ReadAllLines file30
    |> Operations.htmlToItem
    |> Operations.itemToBookmark
    
let unique = Merge.getUniqueBookmarks dups full |> Seq.toList
    
printfn $"Full: {full.Length} Dups: {dups.Length} Unique: {unique.Length}"

unique