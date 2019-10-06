using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using ZermeloFunction.Models;

namespace ZermeloFunction
{
    public static class Function1
    {

        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            var eindTijdItems = new List<string>
            {
                "tot hoe laat",
                "tot wanneer",
                "vanaf hoe laat uit",
                "hoe laat uit",
                "wanneer uit"
            };

            var begintijdItems = new List<string>
            {
                "vanaf hoe laat",
                "vanaf wanneer",
                "vanaf",
                "hoe laat zijn"
            };

            string[] tijd = new string[] { };

            log.Info("C# HTTP trigger function processed a request.");

            string eindTijd = String.Empty;
            string inputDayofweek = string.Empty;
            string leerling;
            string tijdVraag = string.Empty;

            var googleHomeRequest = await req.Content.ReadAsAsync<GoogleHomeRequest>();
            var newGoogleHomeRequest = await req.Content.ReadAsAsync<NewGoogleHomeRequest>();

            if (googleHomeRequest != null)
            {
                leerling = googleHomeRequest?.Result.Parameters.leerling;
                inputDayofweek = googleHomeRequest?.Result.Parameters.dayofweek;
                tijd = googleHomeRequest?.Result.Parameters.tijd;
            }
            else
            {
                leerling = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "leerling", StringComparison.OrdinalIgnoreCase) == 0)
                    .Value;                
            }

            if (string.IsNullOrEmpty(leerling))
                leerling = "wouter";

            string actionDayofweek = string.Empty;

            if (inputDayofweek == string.Empty)
            {
                inputDayofweek = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "dayofweek", StringComparison.OrdinalIgnoreCase) == 0)
                    .Value;
            }

            if (tijd.Length == 0)
            {
                tijdVraag = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "tijd", StringComparison.OrdinalIgnoreCase) == 0)
                    .Value;
            }

            switch (inputDayofweek.ToLower())
            {
                case "maandag":
                    actionDayofweek = "Monday";
                    break;
                case "dinsdag":
                    actionDayofweek = "Tuesday";
                    break;
                case "woensdag":
                    actionDayofweek = "Wednesday";
                    break;
                case "donderdag":
                    actionDayofweek = "Thursday";
                    break;
                case "vrijdag":
                    actionDayofweek = "Friday";
                    break;
                case "morgen":
                    actionDayofweek = DateTime.Now.AddDays(1).DayOfWeek.ToString();
                    break;
                default:
                case "vandaag":
                    actionDayofweek = DateTime.Now.DayOfWeek.ToString();
                    break;
            }

            log.Info($"Parameter passed is '{inputDayofweek}'");

            DayOfWeek dow = DayOfWeek.Sunday;

            if (Enum.IsDefined(typeof(DayOfWeek), actionDayofweek))
                dow = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), actionDayofweek, true);

            var scheduleChanges = new ScheduleChanges();
            var dateToCheck = scheduleChanges.FindDateByWeekday(dow).ToString("dd-MM-yyyy");
            var googleResponse = new Response();
            var result = new Announce();
            string fullTime = string.Empty;
            if (tijd != null)
                fullTime = String.Join(" ", tijd);

            if (eindTijdItems.Contains(fullTime.ToLower()))
                result = scheduleChanges.GetStartOrEndTime(leerling, dateToCheck, inputDayofweek.ToLower(), "end");
            else if (begintijdItems.Contains(fullTime.ToLower()))
                result = scheduleChanges.GetStartOrEndTime(leerling, dateToCheck, inputDayofweek.ToLower(), "start");
            else
                result = scheduleChanges.GetChangesForDay(leerling, dateToCheck);

            var i = 0;

            foreach (var msg in result.Messages)
            {
                i++;
                if (i == result.Messages.Count && result.Messages.Count > 1)
                    googleResponse.speech += " en " + msg.TheMessage + ". ";
                else
                    googleResponse.speech += msg.TheMessage;
            }

            if (googleResponse.speech != null)
            {
                googleResponse.displayText = googleResponse.speech;
                googleResponse.source = "webhook";
            }
            else
            {
                googleResponse.speech = $"Er zijn nog geen wijzigingen bekend voor {leerling} voor deze datum. Het rooster is dus ongewijzigd.";
                googleResponse.displayText = googleResponse.speech;
                googleResponse.source = "webhook";
            }
            return inputDayofweek == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, googleResponse);
        }
    }
}
