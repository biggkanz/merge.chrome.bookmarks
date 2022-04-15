#r @"C:\Users\biggskanz\.nuget\packages\fsharp.data\4.2.8\lib\netstandard2.0\FSharp.Data.dll"
//open System
//open FSharp.Data
#load "Merge.fs"
//open MergeBookmark.Merge
#load "IO.fs"
open MergeBookmark.IO
#load "DateTime.fs"
open MergeBookmark.DateTime

//let bookmarks1 =
//    ReadHtmlDocument @"MergeBookmark\test\bookmarks_35.html"
//
//WriteHtmlDocument @"MergeBookmark\test\test.txt" bookmarks1

//ReadBookmarkFile @"MergeBookmark\test\bookmarks_35.html"

//tryParseFolderAddDate """"    <DT><H3 ADD_DATE="1646785324" LAST_MODIFIED="1649547546" PERSONAL_TOOLBAR_FOLDER="true">Bookmarks</H3>"""
let date = tryParseFolderAddDate """        <DT><H3 ADD_DATE="1649547409" LAST_MODIFIED="0">f#</H3>"""
printfn "ParseDate: %O" date.Value
let dateObject = toDateTime date.Value

let dateSeconds = toUnixTimeSeconds dateObject

let originalDateObject = toDateTime dateSeconds