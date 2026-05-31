using System.ComponentModel;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class TimeInterval : ValueObject
{
    public static readonly TimeOnly EarliestStartTime = new(8, 0, 0);
    public static readonly TimeOnly LatestStartTime = new(0, 0, 0);
    public static readonly TimeOnly LatestOvernightEndTime = new(1, 0, 0);
    public static readonly TimeSpan MinimumDuration = TimeSpan.FromHours(1);
    public static readonly TimeSpan MaximumDuration = TimeSpan.FromHours(10);

    public DateTime StartDateTime { get; }
    public DateTime EndDateTime { get; }

    private TimeInterval(DateTime startDateTime, DateTime endDateTime)
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }

    public static Result<TimeInterval> Create(DateTime startDateTime, DateTime endDateTime, DateTime currentTime)
    {
        if (startDateTime < currentTime)
        {
            return Error.Validation("TimeInterval.Validation", "Event start time must be in the future.");
        }

        if (startDateTime > endDateTime)
        {
            return Error.Validation("TimeInterval.Validation", "Start time must be before end time.");
        }

        var dateDifferenceInDays = (endDateTime.Date - startDateTime.Date).Days;

        if (dateDifferenceInDays > 1)
        {
            return Error.Validation("TimeInterval.Validation", "Event can span at most to the next day.");
        }

        if (dateDifferenceInDays < 0)
        {
            return Error.Validation("TimeInterval.Validation", "Start date cannot be after end date.");
        }

        if (TimeOnly.FromDateTime(startDateTime) < EarliestStartTime)
        {
            return Error.Validation("TimeInterval.Validation", "Start time must be at or after 08:00.");
        }

        if (startDateTime > startDateTime.AddDays(1).Subtract(startDateTime.TimeOfDay))
        {
            return Error.Validation("TimeInterval.Validation", "Start time must be before or at 00:00.");
        }

        if (dateDifferenceInDays == 1 && TimeOnly.FromDateTime(endDateTime) > LatestOvernightEndTime)
        {
            return Error.Validation("TimeInterval.Validation",
                "When an event spans overnight, it must end no later than 01:00.");
        }

        var duration = endDateTime - startDateTime;

        if (duration < MinimumDuration)
        {
            return Error.Validation("TimeInterval.Validation", "Duration must be at least 1 hour.");
        }

        if (duration > MaximumDuration)
        {
            return Error.Validation("TimeInterval.Validation", "Duration must be at most 10 hours.");
        }

        return new TimeInterval(startDateTime, endDateTime);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDateTime;
        yield return EndDateTime;
    }
}
