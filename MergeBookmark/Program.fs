open System
open MergeBookmark
open Util
open Operations

[<EntryPoint>]
let main args =
    if args.Length > 1 then        
        let lst =
            GetUniqueMarksEntry @"D:\documents\merge.chrome.bookmarks\bookmark"            
            
        printfn $"count:{lst |> List.length}"
        
        let names =
            lst
            |> List.map (fun (x,y)->(Entry.getName x, Entry.getName y))
            |> List.iter  (fun (x,y)->printfn $"|-%s{x} | %s{y}")
            
        0
    else
        1