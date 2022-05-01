open MergeBookmark
open MergeBookmark.Utility
open MergeBookmark.Data

[<EntryPoint>]
let main args =
    if args.Length > 0 then
        let t =
            IO.ReadAllLines args[0]
            |> Operations.htmlToIntermediate
            |> Operations.buildTree
        
        t |> Tree.iter (printfn "%A") (printfn "%A")
        0
    else
        1