#r @"C:\Users\biggskanz\.nuget\packages\fsharp.data\4.2.8\lib\netstandard2.0\FSharp.Data.dll"
open FSharp.Data
#load "Merge.fs"
open MergeBookmark.Merge
#load "IO.fs"
open MergeBookmark.IO

//let bookmarks1 =
//    ReadHtmlDocument @"MergeBookmark\test\bookmarks_35.html"
//
//WriteHtmlDocument @"MergeBookmark\test\test.txt" bookmarks1

ReadBookmarkFile @"MergeBookmark\test\bookmarks_35.html"

