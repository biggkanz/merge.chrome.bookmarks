open System
open MergeBookmark
open MergeBookmark.Util

[<EntryPoint>]
let main args =
    if args.Length > 0 then
        IO.ReadAllLines args[0]
        |> Convert.HtmlToTree
        |> Build.DocumentFromTree
        |> Seq.iter (printfn "%s")
 
        0
    else
        1