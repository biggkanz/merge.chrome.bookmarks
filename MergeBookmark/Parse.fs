module MergeBookmark.Parse
    
open Util
open Domain

let hrefRegex = """HREF="?([^"\s]*)"?"""
let addDateRegex = """ADD_DATE="?([^"\s]*)"?"""
let iconRegex = """ICON="?([^"\s]*)"?"""
let nameRegex = """<[HA][^>]*>([^<]*)"""
let modifiedRegex = """LAST_MODIFIED="?([^"\s]*)"?"""

let parseBookmark str =
    let href = Regex.ParseString hrefRegex str 
    let name = Regex.ParseString nameRegex str |> Option.defaultValue ""        
    let date = Regex.ParseInteger64 addDateRegex str |> Option.defaultValue 0L
    let icon = Regex.ParseString iconRegex str |> Option.defaultValue ""
    match href with
    | Some h    -> Some (BookmarkLine.Mark { name=name; href=h; icon="icon" })
    | _         -> None
    
let parseFolder str =
    let name = Regex.ParseString nameRegex str       
    let date = Regex.ParseInteger64 addDateRegex str |> Option.defaultValue 0L
    let modified = Regex.ParseInteger64 modifiedRegex str |> Option.defaultValue 0L
    match name with
    | Some n    -> BookmarkLine.Folder {name=n; date=date.ToString(); modified=modified.ToString()}
    | _         -> BookmarkLine.Folder {name="unnamed folder"; date=date.ToString(); modified=modified.ToString()}

/// determine line type and extract data
let ParseHtmlLine (line:string) =
    if line.Contains("<H3") then Some (parseFolder line)
    elif line.Contains("<A HREF=") then parseBookmark line
    elif line.Contains("<DL>") then Some (BookmarkLine.ListOpen)
    elif line.Contains("</DL>") then Some (BookmarkLine.ListClose)
    else None