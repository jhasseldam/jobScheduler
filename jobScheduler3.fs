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

type Queue = Job []

//type Queue = {
//    mutable Size : int
//    mutable Queue : Job []
//    mutable Fst_index : int
//    mutable Lst_index : int
//}

let init_queue (size : int) : Job [] = Array.create size dummyJob
//let mutable QUEUE : Job [] * Job [] = (init_queue SIZE_PRIORITY_Q, init_queue SIZE_Q)
let mutable SIZE_QUEUE  = 4
let mutable QUEUE       = init_queue SIZE_QUEUE
let mutable FST_INDEX_Q = 0
let mutable LST_INDEX_Q = 0
let mutable ELEMENTS_Q  = 0

let mutable SIZE_PRIORITY      = 4
let mutable QUEUE_PRIORITY     = init_queue SIZE_PRIORITY
let mutable FST_INDEX_PRIORITY = 0
let mutable LST_INDEX_PRIORITY = 0
let mutable ELEMENTS_PRIORITY  = 0

let FIVE_SECONDS = 5
let SECONDS_TO_MILISECONDS = 1000

let increaseSizeQueue (queue : outref<Job []>) (size : byref<int>) : unit =
    let extraSpace = init_queue size in
    queue <- Array.append queue extraSpace
    size  <- size * 2

let enqueue (queue : outref<Job []>) (job : Job) (lstIndex : byref<int>)
            (size : byref<int>) (numberOfElements : outref<int>) : unit =
    queue.[lstIndex] <- job
    numberOfElements <- numberOfElements + 1
    printfn "pre modulu: last index: %A" lstIndex
    // If the queue is filled we double the size of the queue
    //if lstIndex > size - 1 then
    if numberOfElements >= size then
        increaseSizeQueue &queue &size
    else ()
    lstIndex <- (lstIndex + 1) % size // lstIndex + 1
    printfn "post modulu: last index: %A\n" lstIndex

let scheduleJob (job : Job) : unit =
    if job.HasPriority then
        enqueue &QUEUE_PRIORITY job &LST_INDEX_PRIORITY &SIZE_PRIORITY
                &ELEMENTS_PRIORITY
    else
        enqueue &QUEUE job &LST_INDEX_Q &SIZE_QUEUE &ELEMENTS_Q

let getAge (job : Job) : Seconds = (DateTime.Now - job.Submitted).Seconds

let runJob (job : Job) : unit =
    printfn "Running job %d" job.Id
//    let milisecs = job.Duration * SECONDS_TO_MILISECONDS in
//    Threading.Thread.Sleep milisecs |> ignore // ignore return value from Sleep

let dequeue (queue : outref<Job []>) (fstIndex : byref<int>)
            (noOfElements : outref<int>) : unit =
    let dummyJob2 = {
        Id          = 100;
        Duration    = 0;
        HasPriority = false;
        Submitted   = DateTime.Now
    }
    queue.[fstIndex] <- dummyJob2
    noOfElements <- noOfElements - 1
    fstIndex <- fstIndex + 1

let runNextJob unit : unit =
    let queueIsEmpty : bool = FST_INDEX_Q = LST_INDEX_Q
    let priorityQueueIsEmpty : bool = FST_INDEX_PRIORITY = LST_INDEX_PRIORITY
    let jobPriority = QUEUE_PRIORITY.[FST_INDEX_PRIORITY]
    let job  = QUEUE.[FST_INDEX_Q]

    // Checking if queues are empty
    if queueIsEmpty && priorityQueueIsEmpty then
        printfn "No jobs to run"
    // If the normal queue is empty we run a priority job
    elif queueIsEmpty then
        runJob jobPriority
        dequeue &QUEUE_PRIORITY &FST_INDEX_PRIORITY &ELEMENTS_PRIORITY
    // If the priority queue is empty we run a normal job
    elif priorityQueueIsEmpty then
        runJob job
        dequeue &QUEUE &FST_INDEX_Q &ELEMENTS_Q
    // Otherwise we have both a priority job and a normal job
    else
        let jobAge  : Seconds = getAge(job)
        let jobPAge : Seconds = getAge(jobPriority)

        // If there is any job older than 5 seconds it gets executed
        if jobAge > FIVE_SECONDS || jobPAge > FIVE_SECONDS then
            if jobPAge >= jobAge then
                runJob jobPriority
                dequeue &QUEUE_PRIORITY &FST_INDEX_PRIORITY &ELEMENTS_PRIORITY
            else
                runJob job
                dequeue &QUEUE &FST_INDEX_Q &ELEMENTS_Q
        // If we both have a normal and a priority job younger than 5 seconds we
        // always run the priority job.
        else
            runJob jobPriority
            dequeue &QUEUE_PRIORITY &FST_INDEX_PRIORITY &ELEMENTS_PRIORITY

