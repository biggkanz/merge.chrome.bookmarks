open System
open MergeBookmark
open Util
open Operations

[<EntryPoint>]
let main args =
    if args.Length > 1 then        
        //Operations.GetDiffMarks args[0] args[1]
        let file1 = @"D:\documents\merge.chrome.bookmarks\test\bookmarks_35.html"
        let file2 = @"D:\documents\merge.chrome.bookmarks\test\bookmarks_35_30dup.html"
         
        let book1 =
            IO.ReadAllLines file2
            |> Convert.htmlToEntry
         
        let book =
            IO.ReadAllLines file1
            |> Convert.htmlToEntry
            
        let unique = Entry.DiffMarksAndParent book1 book 
        
        printfn "book1 %d book2 %d unique %d" book.Length book1.Length unique.Length
        printfn ""
        unique |> List.iter (fun ((_,_,m),(_,_,f)) ->
            printfn $"mark:{m.name} folder:{f.name}")
         
        0
    else
        1