#load "Domain.fs"
#load "TimestampUtil.fs"
#load "IO.fs"
open MergeBookmark.TimestampUtil
open MergeBookmark.IO

let date = tryParseAddDate """        <DT><H3 ADD_DATE="1649547409" LAST_MODIFIED="0">f#</H3>"""
printfn "ParseDate: %O" date.Value
let dateObject = ToDateTime date.Value

let dateSeconds = ToUnixTimeSeconds dateObject

let originalDateObject = ToDateTime dateSeconds

dateObject = originalDateObject 
    |> printfn "Test passed? %O" 