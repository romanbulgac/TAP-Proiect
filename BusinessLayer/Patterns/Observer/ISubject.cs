using System.Threading.Tasks; // Added
// Assuming ConsultationSubject is in the same namespace or accessible
// using BusinessLayer.Patterns.Observer;

namespace BusinessLayer.Patterns.Observer
{
    public interface ISubject<T>
    {
        void Attach(IObserver<T> observer);
        void Detach(IObserver<T> observer);
        Task NotifyAsync(ConsultationSubject.ConsultationEvent consultationEvent); // Ensure Event enum is accessible
    }
}
