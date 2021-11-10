module jobScheduler
open System

type Seconds = int
type Job = {
    Id : int
    Duration : Seconds
    HasPriority : bool
    Submitted : DateTime
}

let dummyJob = {
    Id          = 0;
    Duration    = 0;
    HasPriority = false;
    Submitted   = DateTime.Now
}

let SIZE_QUEUE = 4
let QUEUE = Array.create SIZE_QUEUE dummyJob
let mutable FST_INDEX  = 0
let mutable LST_INDEX  = 0


let scheduleJob (job : Job) : unit =
    QUEUE.[LST_INDEX] <- job
    (* Check if we need to allocate a bigger queue *)

    LST_INDEX <- LST_INDEX + 1

let runNextJob unit : unit =
// 1. If there is any job older than 5 seconds, it should run.
// 2. If there are priority jobs, the oldest should run before any normal job.
// 3. The oldest submitted job should run.
    ()


