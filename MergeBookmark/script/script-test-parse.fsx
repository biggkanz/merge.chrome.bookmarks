#load "Domain.fs"
#load "Parse.fs"
#load "Operations.fs"
open MergeBookmark.Parse

htmlToDomain @"D:\repos\fsharp\merge.chrome.bookmarks\MergeBookmark\test\test.html"
|> Seq.iter (printfn "%A")