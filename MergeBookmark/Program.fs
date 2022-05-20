open System
open MergeBookmark
open Util
open Operations

[<EntryPoint>]
let main args =
    
    let printEntryList lst =
        lst
        |> List.map Entry.toString
        |> List.iter (printfn "%s")
        
    let printFile file =
        IO.ReadAllLines file
        |> Convert.htmlToEntry
        |> printEntryList
    
    if args.Length > 1 then       
            
        let file1 = @"D:\documents\merge.chrome.bookmarks\test_GetUniqueMarksEntry\bookmarks_0.html"
        let file2 = @"D:\documents\merge.chrome.bookmarks\test_GetUniqueMarksEntry\bookmarks_1.html"
        
        printfn "\nfile1:"
        let pf1 = printFile file1
        printfn "\nfile2:"
        let pf2 = printFile file2
        
        printfn "\nnew file:"    
        GetUniqueMarksEntry_test file1 file2
        |> printEntryList
            
        0
    else
        1