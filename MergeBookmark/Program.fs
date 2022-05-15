open System
open MergeBookmark
open Util
open Operations

[<EntryPoint>]
let main args =
    if args.Length > 1 then        
        let count =
            GetUniqueMarksEntry @"D:\documents\merge.chrome.bookmarks\bookmark"
            |> List.length
            
        printfn $"count:{count}"
                 
        0
    else
        1