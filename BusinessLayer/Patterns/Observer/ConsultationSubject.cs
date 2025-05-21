using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// using DataAccess.Models; // Not directly used here if DTO is used
using BusinessLayer.DTOs;

namespace BusinessLayer.Patterns.Observer
{
    public class ConsultationSubject : ISubject<ConsultationDto>
    {
        private readonly List<IObserver<ConsultationDto>> _observers = new();
        private ConsultationDto _state;
        
        public enum ConsultationEvent
        {
            Created,
            Updated,
            Cancelled,
            Completed,
            Booked,
            MaterialAdded
            // Add other relevant events
        }
        
        public ConsultationDto State => _state;
        
        public ConsultationSubject(ConsultationDto initialState)
        {
            _state = initialState ?? throw new ArgumentNullException(nameof(initialState));
        }
        
        public void Attach(IObserver<ConsultationDto> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IObserver<ConsultationDto> observer)
        {
            _observers.Remove(observer);
        }

        public async Task NotifyAsync(ConsultationEvent consultationEvent)
        {
            var observersSnapshot = new List<IObserver<ConsultationDto>>(_observers);
            foreach (var observer in observersSnapshot)
            {
                await observer.UpdateAsync(_state, consultationEvent);
            }
        }
        
        public async Task UpdateStateAndNotifyAsync(ConsultationDto newState, ConsultationEvent eventToNotify)
        {
            _state = newState ?? throw new ArgumentNullException(nameof(newState));
            await NotifyAsync(eventToNotify);
        }
    }
}
