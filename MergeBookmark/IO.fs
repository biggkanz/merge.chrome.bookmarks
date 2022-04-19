namespace MergeBookmark

open Domain
open FSharp.Data

module IO =
    
    open System.IO
    open FSharp.Data
    
    let ReadHtmlDocument file =  
        File.OpenText file  
        |> HtmlDocument.Load
        
    let GetHtmlNodes path =  
        let html = ReadHtmlDocument path  
        let result =
            html.Elements()
            |> List.last
        result
        //|> Seq.choose (fun n -> n.)
        
//    let GetLinks path =  
//        GetHtmlNodes path  
//        |> Seq.choose (fun node ->  
//            node.TryGetAttribute("href")  
//            |> Option.map (fun att -> att.Value()))
//        
//    let WriteHtmlDocument file (html:HtmlDocument) =  
//        use streamWriter = new StreamWriter(file, false)  
//        HtmlDocument.elements html  
//        |> List.iter (fun node ->              
//            streamWriter.WriteLine(node.ToString()))