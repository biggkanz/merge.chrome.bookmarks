open System
open System.Diagnostics
open FSharp.Data
open MergeBookmark

[<EntryPoint>]
let main args =
    
    //args |> Array.iter (fun arg -> printfn "%s" arg)  
    
    if args.Length > 1 then
        Merge.GetLinks args[0]
        |> Seq.iter (fun x -> printfn "File1: %s" x)
        Merge.GetLinks args[1]
        |> Seq.iter (fun x -> printfn "File2: %s" x) 
        0
    else
        printfn "no args"
        1