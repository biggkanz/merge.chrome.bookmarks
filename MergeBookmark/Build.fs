module MergeBookmark.Build

open System
open MergeBookmark.Domain
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
        
//let htmlFromLine (lines:BookmarkLine seq) =
//    let documentLine (line:BookmarkLine) : DocumentLine = 
//        match line with
//        | Folder f -> FolderTag (lineFromFolder f)
//        | Mark m -> MarkTag (lineFromMark m)
//        | ListOpen -> ListOpenTag listBegin
//        | ListClose -> ListCloseTag listEnd
//        
//    let indentLines (lines:DocumentLine seq) =
//        let mutable indent = 0
//        
//        let lineToString line =
//            match line with
//            | FolderTag f -> f
//            | MarkTag m -> m
//            | ListOpenTag o ->
//                do indent <- indent + 1
//                o
//            | ListCloseTag c ->
//                do indent <- indent - 1
//                c
//            
//        let indentLine (i:DocumentLine) =
//            stringBuffer {
//                for _ in 1 .. indent do "    "
//                lineToString i
//            }        
//                
//        seq { for l in lines do indentLine l }    
//        
//    lines
//    |> Seq.map documentLine
//    |> indentLines
       
let DocumentFromMarks (marks:MarkInfo list) =
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
    
let DocumentFromTree (tree:BookmarkTree) =
    let tab depth = String(' ', depth * 4)
    let rec loop depth (tree:BookmarkTree) =
        let spacer = tab depth
        match tree with
        | LeafNode m ->        
            printfn "%s%s" spacer (lineFromMark m)
        | InternalNode (f,subtrees) ->
            printfn "%s%s" spacer (lineFromFolder f)
            printfn "%s%s" spacer listBegin
            subtrees |> Seq.iter (loop (depth + 1))
            printfn "%s%s" spacer listEnd
            
    header |> Array.iter (printfn "%s") 
    printfn "%s%s" (tab 0) listBegin
    loop 1 tree
    printfn "%s%s" (tab 0) listEnd
