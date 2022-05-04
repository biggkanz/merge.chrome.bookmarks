namespace MergeBookmark

open System

module Parse =
    
    open System.Text.RegularExpressions
    open Domain
     
    let (|Integer64|_|) (str: string) =
       let mutable int64value = 0L
       if Int64.TryParse(str, &int64value) then Some(int64value)
       else None
          
    let (|String|_|) (str: string) =
       if str.Length > 0 then Some(str)
       else None
       
    /// parses a regular expression and returns a list of the strings that match
    let (|ParseRegexGroups|_|) regex str =
       let m = Regex(regex, RegexOptions.IgnoreCase).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None
       
    /// parse and return one string match group
    let parseStringGroup regx str =
        match str with
        | ParseRegexGroups regx [String s]
            -> Some s
        | _ -> None
        
    /// return a match group
    let parseInteger64Group regx str =
        match str with
        | ParseRegexGroups regx [Integer64 i]
            -> Some i
        | _ -> None
            
    let hrefRegex = """HREF="?([^"\s]*)"?"""
    let addDateRegex = """ADD_DATE="?([^"\s]*)"?"""
    let iconRegex = """ICON="?([^"\s]*)"?"""
    let nameRegex = """<[HA][^>]*>([^<]*)"""
    let modifiedRegex = """LAST_MODIFIED="?([^"\s]*)"?"""  
      
    /// shorten string length for output in console (debug)
    let ShortenStrings (p:int,f:int,n:string,s1:string,s2:string) =
        let shorten x (str:string) =
            if (str.Length > x)
            then str.Substring(0,x)
            else str
        p, f, shorten 30 n, shorten 15 s1, shorten 15 s2
     
    let parseBookmark str =
        let href = parseStringGroup hrefRegex str 
        let name = parseStringGroup nameRegex str |> Option.defaultValue ""        
        let date = parseInteger64Group addDateRegex str |> Option.defaultValue 0L
        let icon = parseStringGroup iconRegex str |> Option.defaultValue ""
        match href with
        | Some h    -> BookmarkLine.Mark { name=name; url=h; icon="icon" }
        | _         -> Ig
        
    let parseFolder str =
        let name = parseStringGroup nameRegex str       
        let date = parseInteger64Group addDateRegex str |> Option.defaultValue 0L
        let modified = parseInteger64Group modifiedRegex str |> Option.defaultValue 0L
        match name with
        | Some n    -> BookmarkLine.Folder {name=n; date=date.ToString(); modified=modified.ToString()}
        | _         -> BookmarkLine.Folder {name="unnamed folder"; date=date.ToString(); modified=modified.ToString()}

    /// determine line type and extract data
    let ParseLine (line:string) =
        if line.Contains("<H3") then parseFolder line
        elif line.Contains("<A HREF=") then parseBookmark line
        elif line.Contains("</DL>") then BookmarkLine.ListCloseTag
        else Ig