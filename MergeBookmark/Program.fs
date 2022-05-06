open System
open Dapper.FSharp.Builders
open MergeBookmark
open MergeBookmark.Utility
open MergeBookmark.Domain

[<EntryPoint>]
let main args =
    if args.Length > 0 then
//        let tree =
//            IO.ReadAllLines args[0]
//            |> Operations.htmlToBookmarkLine
//            |> Build.htmlFromLine
//            |> Seq.iter (printfn "%A")
        
        let marks =
            IO.ReadAllLines args[0]
            |> Operations.HtmlToTree
            |> Build.DocumentFromTree
 
        0
    else
        1