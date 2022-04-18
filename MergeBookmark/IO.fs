namespace MergeBookmark

open MergeBookmark.Domain

module IO =
    
    open System.IO
    open System.Text.RegularExpressions
    
    // identify the line type (root folder, folder or bookmark)
    let regexRootFolder = """.*<H3.*PERSONAL_TOOLBAR_FOLDER.*"""
    let regexFolder = """<H3.*</H3>"""    
    let regexBookmark = """(?:<A).*</A>"""
    
    // parse data from line
    let regexName = """<[HA][^>]*>([^<]*)"""
    let regexAddDate = """ADD_DATE="?([^"\s]*)"?"""
    let regexLastModified = """LAST_MODIFIED="?([^"\s]*)"?"""
    let regexBookmarkReference = """HREF="?([^"\s]*)"?"""
    let regexBookmarkIcon = """ICON="?([^"\s]*)"?"""
    
    
    let (|Integer|_|) (str: string) =
       let mutable intvalue = 0L
       if System.Int64.TryParse(str, &intvalue) then Some(intvalue)
       else None
          
    let (|String|_|) (str: string) =
       if str.Length > 0 then Some(str)
       else None
    
    let (|ParseRegex|_|) regex str =
       let m = Regex(regex, RegexOptions.IgnoreCase).Match(str)
       if m.Success
       then Some m.Value 
       else None
       
    // ParseRegex parses a regular expression and returns a list of the strings that match each group in
    // the regular expression.
    // List.tail is called to eliminate the first element in the list, which is the full matched expression,
    // since only the matches for each group are wanted.       
    let (|ParseRegexGroups|_|) regex str =
       let m = Regex(regex, RegexOptions.IgnoreCase).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None
       
    let tryParseAddDate line =
        match line with
        | ParseRegexGroups regexAddDate [Integer x]
            -> Some x
        | _ -> None
        
    let tryParseLastModified line =
        match line with
        | ParseRegexGroups regexLastModified [Integer x]
            -> Some x
        | _ -> None
        
    let tryParseLink line =
        match line with
        | ParseRegexGroups regexBookmarkReference [String x]
            -> Some x
        | _ -> None
        
    let parseName line =
        match line with
        | ParseRegexGroups regexName [String x] -> x
        | _ -> ""
        
    let parseRootFolder line : Folder = {
        Parent = None
        AddDate = tryParseAddDate line
        LastModified = tryParseLastModified line
        Name = parseName line
    }
    
    let rootFolder : Folder = {
        Parent = None
        AddDate = None
        LastModified = None
        Name = "Bookmarks"
    }
    
    let parseFolder parent line : Folder = {
        Parent = Some parent
        AddDate = tryParseAddDate line
        LastModified = tryParseLastModified line
        Name = parseName line
    }
    
    let parseBookmark parent line : Bookmark = {
        Parent = Some parent
        Ref = tryParseLink line |> Option.defaultValue ""
        AddDate = tryParseAddDate line
        Icon = None
        Name = parseName line
    }
    
    let parseBookmarkNode parent line : Option<Node> =
        match line with
        | ParseRegex regexFolder line
            -> Some (Folder (parseFolder parent line))
        | ParseRegex regexBookmark line
            -> Some (Bookmark (parseBookmark parent line))
        | _ -> None
    
    let parseBookmarkRootFolder doc =
        doc
        |> Array.tryPick(
            fun line ->
                match line with
                | ParseRegex regexRootFolder line
                    -> Some (parseRootFolder line)
                | _ -> None)
        |> Option.defaultValue rootFolder
        
    let ReadBookmarkFile file =
        let doc = File.ReadAllLines file
        let root = parseBookmarkRootFolder doc
        printfn "%A" root
  
//    let testReadBookmarkFile file =
//        ReadBookmarkFile file
//        |> Array.filter Option.isSome
//        |> Array.map Option.get
//        |> Array.iter (fun x -> printfn "%A" x)       