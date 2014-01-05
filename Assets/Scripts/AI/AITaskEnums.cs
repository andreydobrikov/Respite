using UnityEngine;
using System.Collections;

public enum AITaskResult 
{
	Idle,
	Pending,
	Suspended,
	Running,
	Complete
}

// TODO: I expect this thing will disappear once I have a handle on how the tasks compare.
public enum AITaskPriority
{
	High,
	Medium,
	Low
}

public enum AITaskSuspendPriority
{
	Normal,
	Urgent,
	Kill
}
