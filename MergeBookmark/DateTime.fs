namespace MergeBookmark

module DateTime =
    
    open System

    let toDateTime (timestamp:int64) =
        let offset = DateTimeOffset.FromUnixTimeSeconds timestamp
        offset.DateTime
        
    let toUnixTimeSeconds (datetime:DateTime) =
        let epoch = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
        datetime.Subtract(epoch).TotalSeconds
        |> int64