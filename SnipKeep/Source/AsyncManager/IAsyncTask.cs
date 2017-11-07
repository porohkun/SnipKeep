using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnipKeep
{
    public interface IAsyncTask
    {
        string StartStatus { get; }
        string FinalStatus { get; }
        string FailureStatus { get; }

        IEnumerator<TaskStatus> DoWork();
        void ExceptionCatch(Exception e);
        void TaskCompleted();
    }
}