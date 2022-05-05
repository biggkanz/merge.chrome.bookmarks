module MergeBookmark.Build

open Utility.StringBuffer
open Domain

let header = [|
    """<!DOCTYPE NETSCAPE-Bookmark-file-1>"""
    """<!-- This is an automatically generated file."""
    """     It will be read and overwritten."""
    """     DO NOT EDIT! -->"""
    """<META HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=UTF-8">"""
    """<TITLE>Bookmarks</TITLE>"""
    """<H1>Bookmarks</H1>"""
|]

let listBegin = """<DL><p>"""    
let listEnd = """</DL><p>"""

let toolbarFolder date modified =
    $"<DT><H3 ADD_DATE=\"{date}\" LAST_MODIFIED=\"{modified}\" PERSONAL_TOOLBAR_FOLDER=\"true\">Bookmarks</H3>"
let folder date modified name =
    $"<DT><H3 ADD_DATE=\"{date}\" LAST_MODIFIED=\"{modified}\">{name}</H3>"
let mark href date icon name =
    $"<DT><A HREF=\"{href}\" ADD_DATE=\"{date}\" ICON=\"{icon}\">\"{name}\"</A>"
    
let lineFromMark m =
    mark m.href "" m.icon m.name
    
let lineFromFolder f =
    folder f.date "" f.name
    
let DocumentFromTree (tree:BookmarkTree) =
        let mutable indent = 0
    
        let indentLine (line:string) =
            stringBuffer {
                for _ in 1 .. indent do "    "
                line
            }
        [|
//            let fMark = indentLine (lineFromMark m)
//            let fFolder = indentLine (lineFromFolder f)
//            
//            Tree.cata fMark  ->
//            
            
            match tree with
            | LeafNode m -> indentLine (lineFromMark m)
            | InternalNode (f,sq) ->
                indentLine (lineFromFolder f)
                listBegin 
        |]


       
let DocumentFromMarks (marks:Mark list) =
    let mutable indent = 0
    
    let indentLine (line:string) =
        stringBuffer {
            for _ in 1 .. indent do "    "
            line
        }  
    
    [|
        indentLine listBegin
        do indent <- indent + 1
        indentLine (toolbarFolder "0" "0")
        indentLine listBegin
        do indent <- indent + 1
        indentLine (folder "0" "0" "import")
        indentLine listBegin
        do indent <- indent + 1
        
        for m in marks do
            indentLine (lineFromMark m)
            
        do indent <- indent - 1
        indentLine listEnd
        do indent <- indent - 1
        indentLine listEnd
        do indent <- indent - 1
        indentLine listEnd
    |] |> Array.append header
