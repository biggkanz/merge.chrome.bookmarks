open MergeBookmark

[<EntryPoint>]
let main args =    
    if args.Length > 0 then        
        let doc = Parse.testReadBookmarkFile args[0]
        doc
        //|> Seq.iter (printfn "%A")
        //|> Seq.iter (fun x -> printfn "%s" (Domain.Node.GetName x))
        //|> Seq.iter (fun x -> printfn "%s" (Domain.Node.GetIndentName x))
        |> Seq.iter (fun x -> printfn "%d" (Domain.Node.GetLevel x))
        0
    else
        printfn "no args"
        1