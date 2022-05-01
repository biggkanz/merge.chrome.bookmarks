namespace MergeBookmark.Utility

module Log =
    
    open System
    open System.Threading
    
    /// Print a colored log message
    let message =
        let lockObj = obj()
        fun (color : ConsoleColor) (message : string ) ->
            lock lockObj (fun () ->
                Console.ForegroundColor <- color
                printfn $"%s{message} (thread ID: %i{Thread.CurrentThread.ManagedThreadId})"
                Console.ResetColor())
    
    /// Print a red log message.
    let red = message ConsoleColor.Red
    let green = message ConsoleColor.Green
    let yellow = message ConsoleColor.Yellow
    let cyan  = message ConsoleColor.Cyan

module Time =
    
    open System

    /// convert UnixTimeSeconds to System.DateTime object
    let ToDateTime (timestamp:int64) =
        let offset = DateTimeOffset.FromUnixTimeSeconds timestamp
        offset.DateTime
        
    /// convert System.DateTime object to UnixTimeSeconds
    let ToUnixTimeSeconds (datetime:DateTime) =
        let epoch = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
        datetime.Subtract(epoch).TotalSeconds
        |> int64
        
module IO =
    
    open System.IO
    open FSharp.Data
    
    let ReadHtmlDocument file =  
        File.OpenText file  
        |> HtmlDocument.Load
        
    let GetHtmlNodes path =  
        let html = ReadHtmlDocument path  
        let result =
            html.Elements()
            |> List.last
        result
        
    let ReadAllLines file =
        Log.green (sprintf $"ReadAllLines: {file}")
        try
            File.ReadAllLines file
        with
        | ex -> failwith $"error in MergeBookmark.IO.ReadAllLines: {file}"