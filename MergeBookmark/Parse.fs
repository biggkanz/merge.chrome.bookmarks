namespace MergeBookmark

open System
open MergeBookmark.Domain

module Parse =
    
    open Domain
    open System.IO
    open System.Text.RegularExpressions
  
    // Parse one line from a NETSCAPE-Bookmark-file-1 and capture 3 groups:
    //  Group 1: Either "H3" meaning Folder or "A" meaning Bookmark
    //  Group 2: Raw text containing Attributes (e.g.: ADD_DATE="1650252539" LAST_MODIFIED="1650252562")
    //  Group 3: Inner text containing Name
    let regexLine = """<D[TL]><([H|A]3?)\s([^>]*)>([^<]*)</[^>]*>"""
    
    // Match attributes of a bookmark folder
    //  Group 1: Date added as UnixTimeSeconds
    //  Group 2: Last modified date as UnixTimeSeconds
    let regexFolderAttr = """ADD_DATE="?([^"\s]*)"?\s*LAST_MODIFIED="?([^"\s]*)"?"""
    
    // Match attributes of a bookmark entry
    //  Group 1: Link
    //  Group 2: Date added as UnixTimeSeconds
    //  Group 3: Icon as png;base64
    let regexBookmarkAttr = """HREF="?([^"\s]*)"?\s*ADD_DATE="?([^"\s]*)"?\s*ICON="?([^"\s]*)"?"""
        
//    let (|Integer|_|) (str: string) =
//       let mutable intvalue = 0L
//       if System.Int64.TryParse(str, &intvalue) then Some(intvalue)
//       else None
//          
//    let (|String|_|) (str: string) =
//       if str.Length > 0 then Some(str)
//       else None
    
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

    let tryParseLine line =
        match line with
        | ParseRegexGroups regexLine [String t; String attr; String name]
            -> Some (t,attr,name)
        | _ -> None
        
    let parseFolderAttr str =
        match str with
        | ParseRegexGroups regexFolderAttr [Integer d; Integer m]
            -> {|date = d; modified = m|}
        | _ -> {|date = 0L; modified = 0L|}
        
    let parseBookmarkAttr str =
        match str with
        | ParseRegexGroups regexFolderAttr [String l; Integer d; String i]
            -> {|link = l; date = d; icon = i|}
        | _ -> {|link = ""; date = 0L; icon = ""|}
                   
    let BuildBookmarkModel input =
        let root = { Parent = None; AddDate = None; LastModified = None; Name = "Bookmarks" }        
        let mutable  _currentParent = root
        
        let buildFolder parent attr text =
            let result = parseFolderAttr attr
            Folder {
                Parent = Some parent
                AddDate = Some result.date
                LastModified = Some result.modified
                Name = text }
            
        let buildBookmark parent attr text =
            let result = parseBookmarkAttr attr
            Bookmark {
                Parent = parent
                Link = result.link
                AddDate = Some result.date
                Icon = result.icon
                Name = text }
                        
        let buildSingleNode (tag, attr, text) =
            let node =
                match tag with
                | "H3" -> buildFolder _currentParent attr text
                | "A" -> buildBookmark _currentParent attr text
                | err -> failwith $"buildSingleNode: Invalid node tag: {err} \n Attributes: {attr} \n Text: {text}"
            match node with
            | Folder f -> do _currentParent <- f
            | _ -> ()            
            node
            
        input
        |> Seq.map buildSingleNode
        
    let testReadBookmarkFile file =
        File.ReadAllLines file
        |> Seq.map tryParseLine
        |> Seq.choose id
        |> BuildBookmarkModel