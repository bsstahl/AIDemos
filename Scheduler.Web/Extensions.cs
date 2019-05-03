using ConferenceScheduler.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Web
{
    public static class Extensions
    {

        public static void WriteSchedule(this IEnumerable<Assignment> assignments, IEnumerable<Session> sessions)
        {
            const int columnWidth = 25;
            const char separatorCharacter = '-';

            var timeslots = assignments.Select(a => a.TimeslotId).Distinct().OrderBy(a => a);
            var rooms = assignments.Select(a => a.RoomId).Distinct().OrderBy(a => a);

            string separator = string.Empty.PadLeft(columnWidth * (rooms.Count() + 1), separatorCharacter);

            var result = new StringBuilder();

            result.Append("T\\R\t|\t");

            foreach (var room in rooms)
                result.Append($"{room.ToString().PadRight(columnWidth, ' ')}\t");

            result.AppendLine();
            result.AppendLine(separator);

            foreach (var timeslot in timeslots)
            {
                result.Append($"{timeslot}\t|\t");
                foreach (var room in rooms)
                {
                    if (assignments.Count(a => a.RoomId == room && a.TimeslotId == timeslot) > 1)
                        throw new ArgumentException($"Multiple assignments to room {room} and timeslot {timeslot}.");
                    else
                    {
                        var assignment = assignments.Where(a => a.RoomId == room && a.TimeslotId == timeslot).SingleOrDefault();
                        if (assignment == null)
                            result.Append("\t".PadLeft(columnWidth, ' '));
                        else
                        {
                            var session = sessions.Single(s => s.Id == assignment.SessionId);
                            result.Append($"{session.ToString(columnWidth)}\t");
                        }
                    }
                }
                result.AppendLine();
            }

            Console.WriteLine(result.ToString());
        }

        public static string ToString(this Session session, int maxWidth)
        {
            if (maxWidth < 6)
            {
                maxWidth = 6;
            }

            if (string.IsNullOrWhiteSpace(session.Name))
            {
                return session.Id.ToString(CultureInfo.CurrentCulture).PadRight(maxWidth);
            }

            string name = session.Name;
            int num = maxWidth - 5;

            return $"{name.Substring(0, Math.Min(name.Length, num)).PadRight(num)} ({session.Id.ToString(CultureInfo.CurrentCulture)})";
        }


        public static Tuple<Presenter, int> Add(this List<Tuple<Presenter, int>> list, Presenter presenter, int timeslotId)
        {
            var presenterUnfavoredTimeslot = new Tuple<Presenter, int>(presenter, timeslotId);
            list.Add(presenterUnfavoredTimeslot);
            return presenterUnfavoredTimeslot;
        }

        public static IEnumerable<int> Add(this IEnumerable<int> list, int item)
        {
            return new List<int>(list) { item };
        }

        public static IEnumerable<int> Remove(this IEnumerable<int> list, int item)
        {
            var result = new List<int>(list);
            result.Remove(item);
            return result;
        }


        public static Assignment Clone(this Assignment assignment)
        {
            return assignment.SessionId.HasValue
                ? new Assignment(assignment.RoomId, assignment.TimeslotId, assignment.SessionId.Value)
                : new Assignment(assignment.RoomId, assignment.TimeslotId);
        }

        public static IEnumerable<Assignment> Clone(this IEnumerable<Assignment> assignments)
        {
            var result = new List<Assignment>();
            foreach (var assignment in assignments)
                result.Add(assignment.Clone());
            return result;
        }
    }
}
