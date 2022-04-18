open System
open System.Diagnostics
open FSharp.Data
open MergeBookmark

[<EntryPoint>]
let main args =
    
    //args |> Array.iter (fun arg -> printfn "%s" arg)  
    
    if args.Length > 0 then
        IO.ReadBookmarkFile args[0]
        0
    else
        printfn "no args"
        1