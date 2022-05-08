open System
open MergeBookmark
open Util

[<EntryPoint>]
let main args =
    if args.Length > 1 then
        
        let file1 =
            IO.ReadAllLines args[0]
            |> Convert.HtmlToTree
                        
        file1
        |> Tree.flattenLeaf
        |> Seq.toList
        |> List.length
        |> printfn "file1: %d"
            
        let file2 =
            IO.ReadAllLines args[1]
            |> Convert.HtmlToTree
            
        file2
        |> Tree.flattenLeaf
        |> Seq.toList
        |> List.length
        |> printfn "file2: %d"
            
        BookmarkTree.DistinctMarks file1 file2
        |> Convert.markToTree
        |> Build.DocumentFromTree
        |> Seq.iter (printfn "%s")
               
        0
    else
        1