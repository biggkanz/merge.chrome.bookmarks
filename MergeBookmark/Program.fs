open System
open System.Diagnostics
open FSharp.Data
open MergeBookmark

[<EntryPoint>]
let main args =
    
    //args |> Array.iter (fun arg -> printfn "%s" arg)  
    
    if args.Length > 0 then
        let path = args[0]
        let html = Merge.TestRead path
        html.Descendants ["a"]
        |> Seq.tryHead
        |> Option.iter (fun decendent -> printfn "%O" decendent)
        0
    else
        printfn "no args"
        1