using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Patterns.Observer
{
    public interface IObserver<T>
    {
        Task UpdateAsync(T subjectState, ConsultationSubject.ConsultationEvent consultationEvent);
    }
}
