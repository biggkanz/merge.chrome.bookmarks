open System
open MergeBookmark
open Util

[<EntryPoint>]
let main args =
    if args.Length > 1 then        
        //Operations.GetDiffMarks args[0] args[1]
        Operations.GetDiffMarksOfDirectory @"E:\backup\bookmark"
               
        0
    else
        1