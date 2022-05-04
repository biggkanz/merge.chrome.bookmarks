open MergeBookmark
open MergeBookmark.Utility

[<EntryPoint>]
let main args =
    if args.Length > 0 then
        let t =
            IO.ReadAllLines args[0]
            |> Operations.htmlToItem
            |> Operations.itemToBookmark
            |> printfn "%A"
 
        0
    else
        1