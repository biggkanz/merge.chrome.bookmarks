module MergeBookmark.Build

open System
open Domain
open Util

let listBegin = """<DL><p>"""

let listEnd = """</DL><p>"""

let header = seq {
    """<!DOCTYPE NETSCAPE-Bookmark-file-1>"""
    """<!-- This is an automatically generated file."""
    """     It will be read and overwritten."""
    """     DO NOT EDIT! -->"""
    """<META HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=UTF-8">"""
    """<TITLE>Bookmarks</TITLE>"""
    """<H1>Bookmarks</H1>"""
    listBegin
}

let footer = seq { listEnd }

let toolbarFolder date modified =
    $"<DT><H3 ADD_DATE=\"{date}\" LAST_MODIFIED=\"{modified}\" PERSONAL_TOOLBAR_FOLDER=\"true\">Bookmarks</H3>"

let lineFromMark m = 
    let mark href date icon name =
        $"<DT><A HREF=\"{href}\" ADD_DATE=\"{date}\" ICON=\"{icon}\">\"{name}\"</A>"
    mark m.href "" m.icon m.name

let lineFromFolder f =
    let folder date modified name =
        $"<DT><H3 ADD_DATE=\"{date}\" LAST_MODIFIED=\"{modified}\">{name}</H3>"
    folder f.date "" f.name
    
let DocumentFromTree (tree:BookmarkTree) =
    let body = 
        let tab depth = String(' ', depth * 4)
        
        let rec loop depth tree =
            let spacer = tab depth
            
            seq {
                match tree with
                | LeafNode m ->        
                    yield sprintf "%s%s" spacer (lineFromMark m)
                | InternalNode (f,subtrees) ->
                    yield sprintf "%s%s" spacer (lineFromFolder f)
                    yield sprintf "%s%s" spacer listBegin
                        
                    yield! subtrees
                        |> Seq.collect (loop (depth + 1))
                         
                    yield sprintf "%s%s" spacer listEnd }
        loop 1 tree
    
    footer
        |> Seq.append body
        |> Seq.append header