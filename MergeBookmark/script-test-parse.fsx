#load "Domain.fs"
#load "TimestampUtil.fs"
#load "Parse.fs"
open MergeBookmark.Parse

testReadBookmarkFile @"D:\repos\fsharp\merge.chrome.bookmarks\MergeBookmark\test\test.html"
|> Seq.iter (printfn "%A")