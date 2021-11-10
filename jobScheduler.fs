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

type Queue = {
    mutable Size : int
    mutable Queue : Job []
    mutable Fst_index : int
    mutable Lst_index : int
}

let init_queue (size : int) : Job [] = Array.create size dummyJob
//let mutable QUEUE : Job [] * Job [] = (init_queue SIZE_PRIORITY_Q, init_queue SIZE_Q)
let mutable SIZE_QUEUE  = 4
let mutable QUEUE       = init_queue SIZE_QUEUE
let mutable FST_INDEX_Q = 0
let mutable LST_INDEX_Q = 0

let mutable SIZE_PRIORITY  = 4
let mutable QUEUE_PRIORITY = init_queue SIZE_PRIORITY
let mutable FST_INDEX_PRIORITY  = 0
let mutable LST_INDEX_PRIORITY  = 0

let increaseSizeQueue (queue : outref<Job []>) (size : byref<int>) : unit =
    let extraSpace = init_queue size in
    queue <- Array.append queue extraSpace
    size  <- size * 2

let insertJob (queue : outref<Job []>) (job : Job) (lstIndex : byref<int>)
              (size : byref<int>) : unit =
    queue.[lstIndex] <- job
    if lstIndex >= size - 1 then
        increaseSizeQueue &queue &size
    else ()
    lstIndex <- lstIndex + 1

let scheduleJob (job : Job) : unit =
    if job.HasPriority then
        insertJob &QUEUE_PRIORITY job &LST_INDEX_PRIORITY &SIZE_PRIORITY
    else
        insertJob &QUEUE job &LST_INDEX_Q &SIZE_QUEUE

let runNextJob unit : unit =
// 1. If there is any job older than 5 seconds, it should run.
// 2. If there are priority jobs, the oldest should run before any normal job.
// 3. The oldest submitted job should run.
    ()


