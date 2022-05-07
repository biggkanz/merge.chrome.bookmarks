#load "..\Util.fs"
#load "..\Domain.fs"
#load "..\Parse.fs"
#load "..\Convert.fs"
#load "..\Merge.fs"

open MergeBookmark
open System.IO

let file30 = @"D:\documents\merge.chrome.bookmarks\test\bookmarks_35_30dup.html"
let file35 = @"D:\documents\merge.chrome.bookmarks\test\bookmarks_35.html "

let full =
    File.ReadAllLines file35
    |> Convert.HtmlToTree
    
let dups =
    File.ReadAllLines file30
    |> Convert.HtmlToTree
    
//let unique = Merge.getUniqueBookmarks dups full |> Seq.toList
//    
//printfn $"Full: {full.Length} Dups: {dups.Length} Unique: {unique.Length}"
//
//unique