using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZermeloFunction.Models;

namespace ZermeloFunction
{
    public static class ExtensionMethods
    {
        //public static DateTime FromUnixTime(this long unixTime)
        //{
        //    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        //    return epoch.AddSeconds(unixTime);
        //}
        //public static long ToUnixTime(this DateTime date)
        //{
        //    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        //    return Convert.ToInt64((date - epoch).TotalSeconds);
        //}

        // Local Time --> Unix Time
        public static long ToUnixTime(this DateTime dateTime)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            return dateTimeOffset.ToUnixTimeSeconds();
        }


        // Unix Time --> Local Time
        public static DateTime FromUnixTime(this long unixDateTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixDateTime).DateTime.ToLocalTime().AddHours(2);
        }
    }

    public class ScheduleChanges
    {
        public static Dictionary<string, string> _lessons = new Dictionary<string, string>();

        public ScheduleChanges()
        {
            if (_lessons.Count == 0)
            {
                _lessons.Add("ne", "Nederlands");
                _lessons.Add("netl", "Nederlands");
                _lessons.Add("lv", "Levensbeschouwing");
                _lessons.Add("mu", "Muziek");
                _lessons.Add("en", "Engels");
                _lessons.Add("entl", "Engels");
                _lessons.Add("lo", "Lichamelijke opvoeding");
                _lessons.Add("du", "Duits");
                _lessons.Add("gr", "Grieks");
                _lessons.Add("gs", "Geschiedenis");
                _lessons.Add("la", "Latijn");
                _lessons.Add("ak", "Aardrijkskunde");
                _lessons.Add("na", "Natuurkunde");
                _lessons.Add("nat", "Natuurkunde");
                _lessons.Add("wi", "Wiskunde");
                _lessons.Add("wisb", "Wiskunde");
                _lessons.Add("bi", "Biologie");
                _lessons.Add("ment", "Mentor uur");
                _lessons.Add("fa", "Frans");
                _lessons.Add("te", "Tekenen");
                _lessons.Add("in", "Informatica");
                _lessons.Add("vibalg", "Algemeen VIB uur");

            }
        }

        public Announce GetStartOrEndTime(string leerling, string date, string dayOfWeek, string StartOrEnd)
        {
            var announcement = new Announce();
            var messages = new List<Message>();
            var myZermeloObject = GetSchedule(date, leerling);
            var aantalUur = myZermeloObject.response.totalRows;
            int endTime = 0;
            IOrderedEnumerable<Datum> lesUren;
            string theFinalMessage = string.Empty;

            if (StartOrEnd == "end")
            {
                lesUren = myZermeloObject.response.data.OrderByDescending(t => t.startTimeSlot);
                theFinalMessage = "{0} is {1} om {2} uit.";
            }
            else
            {
                lesUren = myZermeloObject.response.data.OrderBy(t => t.startTimeSlot);
                theFinalMessage = "{0} moet {1} om {2} op school zijn.";
            }

            foreach (var lesUur in lesUren)
            {
                if (!lesUur.cancelled)
                {
                    if (StartOrEnd == "end")
                    {
                        var tmpendTime = lesUur.endTimeSlot;
                        endTime = myZermeloObject.response.data.Where(o => o.endTimeSlot == tmpendTime).First().end;
                        break;
                    }
                    else
                    {
                        var tmpendTime = lesUur.startTimeSlot;
                        endTime = myZermeloObject.response.data.Where(o => o.startTimeSlot == tmpendTime).First().start;
                        break;
                    }
                }
            }

            var endTimeMessage = new Message
            {
                TheMessage = string.Format(theFinalMessage, leerling, dayOfWeek, ExtensionMethods.FromUnixTime(endTime).ToString("HH:mm")),
            };
            messages.Add(endTimeMessage);

            announcement.Messages = messages;

            var jsonREsult = JsonConvert.SerializeObject(announcement);
            Console.Write(jsonREsult);

            return announcement;
        }

        public Announce GetChangesForDay(string leerling, string date)
        {
            //er wordt gevraagd of er wijzigingen zijn voor een datum
            //dus maken we een range voor deze datum

            var announcement = new Announce();
            var messages = new List<Message>();

            var myZermeloObject = GetSchedule(date, leerling);

            foreach (var dataRow in myZermeloObject.response.data.OrderBy(p => p.start)
                .Where((f => f.changeDescription != string.Empty || f.cancelled == true)))
            {
                //Dit geeft de rows met wijzigingen, nu de row opvragen van de gewijzigde rows waar modified niet true is.
                if (dataRow.modified)
                {
                }

                //Als het eerste uur uitvalt:
                var dataRowSubject = dataRow.subjects[0];
                var shortDateString = ExtensionMethods.FromUnixTime(dataRow.start).ToShortDateString();
                var shortTimeString = ExtensionMethods.FromUnixTime(dataRow.start).ToShortTimeString();

                string theLocation = string.Empty;
                string[] datarowLocation = new string[] { };
                if (dataRow.locations.Length > 0)
                {
                    datarowLocation = dataRow.locations;
                    theLocation = datarowLocation[0];
                }
                else
                {
                    theLocation = "NN";
                }

                string theTeacher = string.Empty;
                string[] dataRowTeacher = new string[] { };
                if (dataRow.teachers.Length > 0)
                {
                    dataRowTeacher = dataRow.teachers;
                    theTeacher = dataRowTeacher[0];

                }
                else
                {
                    theTeacher = "NN";
                }

                var dataRowChangeDescription = dataRow.changeDescription;
                var les = dataRow.subjects[0];
                if (_lessons.ContainsKey(dataRow.subjects[0]))
                    les = _lessons[dataRow.subjects[0]];

                if (dataRow.teacherChanged)
                {
                    var teacherMessage = new Message
                    {
                        TheMessage = $"Er is een docent wijziging: {les} wordt nu gegeven door {dataRow.teachers[0]}.",
                        Classroom = dataRow.locations[0]
                    };
                    messages.Add(teacherMessage);
                }

                if (dataRow.moved)
                {
                    var moveMessage = new Message
                    {
                        TheMessage =
                            $"Er is een wijziging in de ruimte: {les} wordt nu gegeven in lokaal {dataRow.locations[0]}.",
                        Classroom = dataRow.locations[0]
                    };
                    messages.Add(moveMessage);
                }

                if (dataRow.cancelled)
                {
                    var cancelledMessage = new Message
                    {
                        TheMessage = $"Het {dataRow.startTimeSlot}e uur {les} valt uit.",
                        Classroom = theLocation,
                        cancelled = true
                    };
                    messages.Add(cancelledMessage);

                }
            }

            announcement.Messages = messages;

            var jsonREsult = JsonConvert.SerializeObject(announcement);
            Console.Write(jsonREsult);

            return announcement;

        }



        public ZermeloResult GetSchedule(string date, string leerling)
        {
            long dateStart = 1552892400;
            long dateEnd = 1553274000;
            var unixDateStart = DateTime.ParseExact(date.ToString(), "dd-MM-yyyy", null);
            var unixDateEnd = DateTime.ParseExact(date.ToString(), "dd-MM-yyyy", null).AddHours(24);

            dateStart = unixDateStart.ToUnixTime();
            dateEnd = unixDateEnd.ToUnixTime();
            try
            {
                string url = string.Empty;
                if (leerling.ToLower() == "wouter")
                    url =
                        $"https://mencia-isbreda.zportal.nl/api/v3/appointments?user=~me&start={dateStart}&end={dateEnd}&access_token=ek8od99iqdaa0phfnsl5nfofl9";
                else if (leerling.ToLower() == "berend")
                    url =
                        $"https://mencia-isbreda.zportal.nl/api/v3/appointments?user=~me&start={dateStart}&end={dateEnd}&access_token=tt7cq8p4lac0rftdp9ddeuc3f4";

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                string content;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
                ZermeloResult myZermeloObject =
                    (ZermeloResult)JsonConvert.DeserializeObject(content, typeof(ZermeloResult));
                return myZermeloObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }



        public DateTime FindDateByWeekday(DayOfWeek weekday)
        {
            string the_temp_date = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);

            // Find the next Friday.
            // Get the number of days between the_date's
            // day of the week and Friday.
            DateTime the_date = DateTime.ParseExact(the_temp_date, "MM/dd/yyyy", null);
            int num_days = weekday - the_date.DayOfWeek;
            if (num_days < 0) num_days += 7;

            // Add the needed number of days.
            DateTime wantedDate = the_date.AddDays(num_days);

            // Display the result.
            return wantedDate;
        }
    }

}

