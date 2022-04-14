namespace MergeBookmark

module IO =
    
    open System.IO
    open FSharp.Data
    open System.Text.RegularExpressions
    
    // ParseRegex parses a regular expression and returns a list of the strings that match each group in
    // the regular expression.
    // List.tail is called to eliminate the first element in the list, which is the full matched expression,
    // since only the matches for each group are wanted.
    let (|ParseRegex|_|) regex str =
       let m = Regex(regex).Match(str)
       if m.Success
       then Some m.Value 
       else None
       
    let parseBookmarkLine line =
        match line with
        | ParseRegex "(?:<H3).+(?:PERSONAL_TOOLBAR_FOLDER=\"true\")" line
            -> Some line
        | ParseRegex "(?:<A HREF=\").+" line
            -> Some line
        | _ -> None
    
    let ReadHtmlDocument file =
        File.OpenText file
        |> HtmlDocument.Load
        
    let testReadBookmarkFile file =
        File.ReadAllLines file
            |> Array.iter (fun x -> printfn "%s" x)
       
    let ReadBookmarkFile file =
        File.ReadAllLines file
            |> Array.map (fun x -> parseBookmarkLine x)
    let testWriteHtmlDocument file =
        use streamWriter = new StreamWriter(file, false)
        streamWriter.WriteLine("<startTag>")
        streamWriter.WriteLine("</startTag>")
        
    let WriteHtmlDocument file (html:HtmlDocument) =
        use streamWriter = new StreamWriter(file, false)
        HtmlDocument.elements html
        |> List.iter (fun node ->            
            streamWriter.WriteLine(node.ToString()))
        
    let GetHtmlNodes path =
        let html = ReadHtmlDocument path
        html.Descendants ["a"]
        
    let GetLinks path =
        GetHtmlNodes path
        |> Seq.choose (fun node ->
            node.TryGetAttribute("href")
            |> Option.map (fun att -> att.Value()))