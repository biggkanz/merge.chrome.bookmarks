namespace MergeBookmark

module Merge =
    
    open System
    open System.IO
    open FSharp.Data
    
    let TestRead file =
        File.OpenText file
        |> HtmlDocument.Load