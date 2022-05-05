open System
open MergeBookmark
open MergeBookmark.Utility
open MergeBookmark.Build
open MergeBookmark.Domain

[<EntryPoint>]
let main args =
    if args.Length > 0 then
        let tree =
            IO.ReadAllLines args[0]
            |> Operations.htmlToEntryList
            |> Operations.buildTree
            |> DocumentFromTree
            |> Array.iter (printfn "%s")
            
        //tree |> Tree.iter (printfn "%A") (printfn "%A")
        
        
//        let marks =
//            IO.ReadAllLines args[0]
//            |> Operations.htmlToItem
//            |> Operations.itemToBookmark
//        
//        DocumentFromMarks marks
//        |> Array.iter (printfn "%s")
 
        0
    else
        1